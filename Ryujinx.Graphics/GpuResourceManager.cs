using Ryujinx.Graphics.Gal;
using Ryujinx.Graphics.Memory;
using Ryujinx.Graphics.Texture;
using System.Collections.Generic;
using Ryujinx.Graphics.Gal.OpenGL;

namespace Ryujinx.Graphics
{
    public class GpuResourceManager
    {
        public enum ImageType
        {
            None,
            Texture,
            ColorBuffer,
            ZetaBuffer
        }

        private NvGpu _gpu;

        private HashSet<long>[] _uploadedKeys;

        private Dictionary<TextureKey, ImageType> _imageTypes;

        public GpuResourceManager(NvGpu gpu)
        {
            _gpu = gpu;

            _uploadedKeys = new HashSet<long>[(int)NvGpuBufferType.Count];

            for (int index = 0; index < _uploadedKeys.Length; index++)
            {
                _uploadedKeys[index] = new HashSet<long>();
            }

            _imageTypes = new Dictionary<TextureKey, ImageType>();
        }

        public void SendColorBuffer(NvGpuVmm vmm, TextureKey key, int attachment, GalImage newImage)
        {
            _imageTypes[key] = ImageType.ColorBuffer;

            if (TryReuse(vmm, key, newImage) == null)
            {
                _gpu.Renderer.Texture.Create(key, null, newImage);
            }

            _gpu.Renderer.RenderTarget.BindColor(key, attachment);
        }

        public void SendZetaBuffer(NvGpuVmm vmm, TextureKey key, GalImage newImage)
        {
            _imageTypes[key] = ImageType.ZetaBuffer;

            if (TryReuse(vmm, key, newImage) == null)
            {
                _gpu.Renderer.Texture.Create(key, null, newImage);
            }

            _gpu.Renderer.RenderTarget.BindZeta(key);
        }

        public ImageHandler SendTexture(NvGpuVmm vmm, TextureKey key, GalImage newImage)
        {
            ImageHandler handler = PrepareSendTexture(vmm, key, newImage);

            _imageTypes[key] = ImageType.Texture;

            return handler;
        }

        private ImageHandler PrepareSendTexture(NvGpuVmm vmm, TextureKey key, GalImage newImage)
        {
            bool skipCheck = false;

            if (_imageTypes.TryGetValue(key, out ImageType oldType))
            {
                if (oldType == ImageType.ColorBuffer || oldType == ImageType.ZetaBuffer)
                {
                    //Avoid data destruction
                    MemoryRegionModified(vmm, key.Position, key.Size, NvGpuBufferType.Texture);

                    skipCheck = true;
                }
            }

            if (skipCheck || !MemoryRegionModified(vmm, key.Position, key.Size, NvGpuBufferType.Texture))
            {
                var handler = TryReuse(vmm, key, newImage);
                if (handler != null)
                {
                    return handler;
                }
            }

            byte[][] data = ImageUtils.ReadTexture(vmm, newImage, key.Position);

            return _gpu.Renderer.Texture.Create(key, data, newImage);
        }

        private ImageHandler TryReuse(NvGpuVmm vmm, TextureKey key, GalImage newImage)
        {
            if (_gpu.Renderer.Texture.TryGetImageHandler(key, out ImageHandler cachedImage) && cachedImage.Image.SizeMatches(newImage))
            {
                _gpu.Renderer.RenderTarget.Reinterpret(cachedImage, newImage);

                return cachedImage;
            }

            return null;
        }

        public bool MemoryRegionModified(NvGpuVmm vmm, long position, long size, NvGpuBufferType type)
        {
            HashSet<long> uploaded = _uploadedKeys[(int)type];

            if (!uploaded.Add(position))
            {
                return false;
            }

            return vmm.IsRegionModified(position, size, type);
        }

        public void ClearPbCache()
        {
            for (int index = 0; index < _uploadedKeys.Length; index++)
            {
                _uploadedKeys[index].Clear();
            }
        }

        public void ClearPbCache(NvGpuBufferType type)
        {
            _uploadedKeys[(int)type].Clear();
        }
    }
}
