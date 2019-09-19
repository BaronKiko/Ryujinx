using Ryujinx.Graphics.Texture;

namespace Ryujinx.Graphics.Gal
{
    public struct GalImage
    {
        public int Width;
        public int Height;

        // FIXME: separate layer and depth
        public int Depth;
        public int LayerCount;
        public int TileWidth;
        public int GobBlockHeight;
        public int GobBlockDepth;
        public int Pitch;
        public int MaxMipmapLevel;

        public GalImageFormat   Format;
        public GalMemoryLayout  Layout;
        public GalTextureSource XSource;
        public GalTextureSource YSource;
        public GalTextureSource ZSource;
        public GalTextureSource WSource;
        public GalTextureTarget TextureTarget;

        public int Size
        {
            get
            {
                if (size == 0)
                {
                    size = LayerStride * LayerCount;
                }

                return size;
            }

            set
            {
                size = value;
                LayerStride = value / LayerCount;
            }
        }

        public int LayerStride
        {
            get
            {
                if (layerStride == 0)
                {
                    layerStride = ImageUtils.GetLayerStride(this);
                }

                return layerStride;
            }

            private set { layerStride = value; }
        }

        // Private backing variables
        private int size, layerStride;

        public GalImage(
            int              width,
            int              height,
            int              depth,
            int              layerCount,
            int              tileWidth,
            int              gobBlockHeight,
            int              gobBlockDepth,
            GalMemoryLayout  layout,
            GalImageFormat   format,
            GalTextureTarget textureTarget,
            int              maxMipmapLevel = 1,
            GalTextureSource xSource        = GalTextureSource.Red,
            GalTextureSource ySource        = GalTextureSource.Green,
            GalTextureSource zSource        = GalTextureSource.Blue,
            GalTextureSource wSource        = GalTextureSource.Alpha)
        {
            Width          = width;
            Height         = height;
            LayerCount     = layerCount;
            Depth          = depth;
            TileWidth      = tileWidth;
            GobBlockHeight = gobBlockHeight;
            GobBlockDepth  = gobBlockDepth;
            Layout         = layout;
            Format         = format;
            MaxMipmapLevel = maxMipmapLevel;
            XSource        = xSource;
            YSource        = ySource;
            ZSource        = zSource;
            WSource        = wSource;
            TextureTarget  = textureTarget;
            size           = 0;
            layerStride    = 0;

            Pitch = ImageUtils.GetPitch(format, width);
        }

        public bool SizeMatches(GalImage image, bool ignoreLayer = false)
        {
            if (ImageUtils.GetBytesPerPixel(Format) !=
                ImageUtils.GetBytesPerPixel(image.Format))
            {
                return false;
            }

            if (ImageUtils.GetAlignedWidth(this) !=
                ImageUtils.GetAlignedWidth(image))
            {
                return false;
            }

            bool result = Height == image.Height && Depth == image.Depth;

            if (!ignoreLayer)
            {
                result = result && LayerCount == image.LayerCount;
            }

            return result;
        }
    }
}