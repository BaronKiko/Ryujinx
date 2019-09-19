using RangeTree;
using Ryujinx.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ryujinx.Graphics.Texture;

namespace Ryujinx.Graphics.Gal.OpenGL
{
    public class OglSizedCachedResource
    {
        public delegate void DeleteValue(ImageHandler value);

        private const int MinTimeDelta = 5 * 60000;
        private const int MaxRemovalsPerRun = 10;

        public class CacheBucket
        {
            public ImageHandler Value { get; set; }

            private LinkedListNode<CacheBucket> _node;
            public LinkedListNode<CacheBucket> Node
            {
                get => _node;
                set
                {
                    _node = value;
                    Timestamp = PerformanceCounter.ElapsedMilliseconds;
                }
            }

            public long DataKey { get; private set; }

            public long DataSize { get; private set; }

            public long Timestamp { get; private set; }

            public CacheBucket(ImageHandler value, long dataKey, long dataSize, LinkedListNode<CacheBucket> node)
            {
                Value = value;
                DataKey = dataKey;
                DataSize = dataSize;
                Node = node;
            }
        }

        private RangeTree<long, CacheBucket> _cacheTree;

        private LinkedList<CacheBucket> _sortedCache;

        private DeleteValue _deleteValueCallback;

        private Queue<ImageHandler> _deletePending;

        private bool _locked;

        private long _maxSize;
        private long _totalSize;

        public OglSizedCachedResource(DeleteValue deleteValueCallback, long maxSize)
        {
            _maxSize = maxSize;
            _deleteValueCallback = deleteValueCallback ?? throw new ArgumentNullException(nameof(deleteValueCallback));

            _cacheTree = new RangeTree<long, CacheBucket>();

            _sortedCache = new LinkedList<CacheBucket>();

            _deletePending = new Queue<ImageHandler>();
        }

        public void Lock()
        {
            _locked = true;
        }

        public void Unlock()
        {
            _locked = false;

            while (_deletePending.TryDequeue(out ImageHandler value))
            {
                _deleteValueCallback(value);
            }

            ClearCacheIfNeeded();
        }

        public IEnumerable<ImageHandler> GetInRange(long key, long size, GalTextureTarget avoidTarget)
        {
            long limit = key + size;
            return _cacheTree.Query(key, limit)
                .Where((i) =>
                {
                    return ((i.DataSize != size) || (i.Value.Image.TextureTarget != avoidTarget)) && (i.DataKey >= key) && ((i.DataKey + i.DataSize) <= limit);
                })
                .Select((i) => i.Value);
        }

        public int[] TryGetParentTexture(long key, long size, out ImageHandler parent)
        {
            var overlapped = _cacheTree.Query(key, key + size);
            
            foreach (var parentCandidiate in overlapped)
            {
                if (parentCandidiate.DataSize > size)
                {
                    int[] indexes = ImageUtils.GetMapIndex(parentCandidiate.Value.Map, (key - parentCandidiate.Value.Key.Position), size, parentCandidiate.Value.Image.Size);
                    if (indexes.Length > 0)
                    {
                        parent = parentCandidiate.Value;
                        return indexes;
                    }
                }
            }

            parent = null;
            return new int[0];
        }

        public ImageHandler AddOrUpdate(TextureKey key, ImageHandler value)
        {
            ImageHandler oldHandler = null;

            if (!_locked)
            {
                ClearCacheIfNeeded();
            }

            CacheBucket bucket = GetBucket(key);

            if (bucket == null)
            {
                bucket = new CacheBucket(value, key.Position, key.Size, null);
                _cacheTree.Add(key.Position, key.Position + key.Size, bucket);
                _totalSize += key.Size;
            }
            else
            {
                if (bucket.Value.Image.TextureTarget != value.Image.TextureTarget)
                {
                    Console.WriteLine("Blah");
                }

                oldHandler = bucket.Value;
                bucket.Value = value;
                _sortedCache.Remove(bucket.Node);
            }

            var n = _sortedCache.AddLast(bucket);
            bucket.Node = n;

            value.Bucket = bucket;

            return oldHandler;

            /*if (bucket != null)
            {
                if (_locked)
                {
                    _deletePending.Enqueue(bucket.Value);
                }
                else
                {
                    _deleteValueCallback(bucket.Value);
                }

                _sortedCache.Remove(bucket.Node);
                _cacheTree.Remove(bucket);

                _totalSize -= bucket.DataSize;

                bucket.
            }

            bucket = new CacheBucket(value, key.Position, key.Size, null);
            _cacheTree.Add(key.Position, key.Position + key.Size, bucket);
            var n = _sortedCache.AddLast(bucket);
            bucket.Node = n;

            _totalSize += key.Size;*/
        }

        public bool TryGetValue(TextureKey key, out ImageHandler value)
        {
            var a = _cacheTree.Query(key.Position, key.Position + key.Size);

            CacheBucket bucket = GetBucket(key);
            if (bucket != null)
            {
                value = bucket.Value;

                _sortedCache.Remove(bucket.Node);

                LinkedListNode<CacheBucket> node = _sortedCache.AddLast(bucket);
                bucket.Node = node;

                return true;
            }

            value = default(ImageHandler);

            return false;
        }

        public bool IsCached(TextureKey key) => TryGetValue(key, out ImageHandler v);

        private CacheBucket GetBucket(TextureKey key)
        {
            return _cacheTree.Query(key.Position, key.Position + key.Size).FirstOrDefault((b) => b.Value.Key.Equals(key));

            /*IEnumerable<CacheBucket> overlap = _cacheTree.Query(key.Position, key.Position + key.Size);

            foreach (CacheBucket bucket in overlap)
            {
                if (key.Target == GalTextureTarget.Unknown)
                {
                    if ((key.Position == bucket.Value.Key.Position) && (key.Size == bucket.Value.Key.Size))
                    {
                        ImageHandler topHandler = bucket.Value;

                        while (topHandler.Parent != null)
                        {
                            if ((key.Position == topHandler.Parent.Key.Position) && (key.Size == topHandler.Parent.Key.Size))
                            {
                                topHandler = topHandler.Parent;
                            }
                            else
                            {
                                break;
                            }
                        }

                        return topHandler.Bucket;
                    }
                }
                else if (key.Equals(bucket.Value.Key))
                {
                    return bucket;
                }
            }

            return null;*/
        }

        private void ClearCacheIfNeeded()
        {
            return;

            long timestamp = PerformanceCounter.ElapsedMilliseconds;

            int count = 0;

            while (count++ < MaxRemovalsPerRun)
            {
                LinkedListNode<CacheBucket> node = _sortedCache.First;

                if (node == null)
                {
                    break;
                }

                CacheBucket bucket = node.Value;

                long timeDelta = timestamp - bucket.Timestamp;

                if (timeDelta <= MinTimeDelta && !UnderMemoryPressure())
                {
                    break;
                }

                _sortedCache.Remove(node);

                _cacheTree.Remove(bucket);

                _deleteValueCallback(bucket.Value);

                _totalSize -= bucket.DataSize;
            }
        }

        private bool UnderMemoryPressure()
        {
            return _totalSize >= _maxSize;
        }
    }
}
