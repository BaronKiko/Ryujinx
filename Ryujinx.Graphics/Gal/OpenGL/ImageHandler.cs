using System;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL;
using Ryujinx.Common.Logging;
using Ryujinx.Graphics.Texture;
using static Ryujinx.Graphics.Gal.OpenGL.OglSizedCachedResource;

namespace Ryujinx.Graphics.Gal.OpenGL
{
    public class ImageHandler
    {
        public GalImage Image;

        public TextureKey Key;

        public TextureHandle Handle { get; set; }

        public ImageHandler Parent;

        public CacheBucket Bucket;

        public bool HasColor   => ImageUtils.HasColor(TopLevelImage.Format);
        public bool HasDepth   => ImageUtils.HasDepth(TopLevelImage.Format);
        public bool HasStencil => ImageUtils.HasStencil(TopLevelImage.Format);

        public GalImage TopLevelImage => TopLevelHandler.Image;
        public ImageHandler TopLevelHandler => (Parent == null) ? this : Parent.TopLevelHandler;

        public TextureMapHandle[] Map
        {
            get
            {
                if (map == null)
                {
                    map = ImageUtils.GetMap(Image);
                }

                return map;
            }

            private set { }
        }

        // Private backing variables
        private TextureMapHandle[] map = null;
        private TextureHandle handle;

        public ImageHandler(int handle, TextureKey key, GalImage image)
        {
            Handle = new TextureHandle(handle);
            Key    = key;
            Image  = image;
        }

        public List<int> SetHandle(TextureHandle handle, PixelInternalFormat format)
        {
            List<int> deadTextures = new List<int>();

            if (handle.ParentHandle == Handle.ParentHandle)
            {
                // Already set, we can return early
                return deadTextures;
            }

            Logger.PrintInfo(LogClass.Gpu, $"Changing {Image.TextureTarget} - {Image.LayerCount} handle from {Handle.ParentHandle}:{Handle.Layer}:{Handle.Level} to {handle.ParentHandle}:{handle.Layer}:{handle.Level}");

            if (Handle.ViewHandle != -1)
            {
                deadTextures.Add(Handle.ViewHandle);
            }

            Handle = handle;

            bool isLayered = ImageUtils.IsLayered(Image.TextureTarget);
            foreach (TextureMapHandle mapHandle in Map)
            {
                if (mapHandle.Image == null)
                {
                    continue;
                }

                int layer = mapHandle.Layer;

                if (!isLayered)
                {
                    layer = -1;
                }
                else if (Handle.Layer != -1)
                {
                    layer += Handle.Layer;
                }

                int view = GL.GenTexture();
                GL.TextureView(view, TextureTarget.ProxyTexture2D, Handle.ParentHandle, format, Handle.Level + mapHandle.Level, 1, layer, 1);

                deadTextures.AddRange(mapHandle.Image.SetHandle(new TextureHandle(Handle.ParentHandle, view, layer, Handle.Level + mapHandle.Level), format));
            }

            return deadTextures;
        }
    }
}
