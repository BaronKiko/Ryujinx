namespace Ryujinx.Graphics.Texture
{
    interface ISwizzle
    {
        int GetSwizzleOffset(int x, int y, int z);

        void SetMipLevel(int level);

        int GetMipOffset(int level);

        int[] GetMipOffsets(int maxLevel);

        (int, int, int)[] GetMipDimensions(int maxLevel);

        int GetImageSize(int mipsCount);
    }
}