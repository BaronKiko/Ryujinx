using System;

namespace Ryujinx.Graphics.Texture
{
    class LinearSwizzle : ISwizzle
    {
        private int _pitch;
        private int _bpp;

        private int _sliceSize;

        private int _width;
        private int _height;

        public LinearSwizzle(int pitch, int bpp, int width, int height)
        {
            _pitch     = pitch;
            _bpp       = bpp;
            _sliceSize = width * height * bpp;
            _width = width;
            _height = height;
        }

        public void SetMipLevel(int level)
        {
            if (level == 0)
                return;

            throw new NotImplementedException();
        }

        public int GetMipOffset(int level)
        {
            if (level == 1)
                return _sliceSize;
            throw new NotImplementedException();
        }

        public int[] GetMipOffsets(int maxLevel)
        {
            if (maxLevel == 1)
                return new []{ 0, _sliceSize };
            throw new NotImplementedException();
        }

        public (int, int, int)[] GetMipDimensions(int maxLevel)
        {
            if (maxLevel == 1)
                return new[] {(_width, _height, 1)};
            throw new NotImplementedException();
        }

        public int GetImageSize(int mipsCount)
        {
            int size = GetMipOffset(mipsCount);

            size = (size + 0x1fff) & ~0x1fff;

            return size;
        }

        public int GetSwizzleOffset(int x, int y, int z)
        {
            return z * _sliceSize + x * _bpp + y * _pitch;
        }
    }
}