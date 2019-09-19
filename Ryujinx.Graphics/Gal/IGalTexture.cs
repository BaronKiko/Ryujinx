using Ryujinx.Graphics.Gal.OpenGL;

namespace Ryujinx.Graphics.Gal
{
    public interface IGalTexture
    {
        void LockCache();
        void UnlockCache();

        ImageHandler Create(TextureKey key, byte[][] data, GalImage image);

        bool TryGetImageHandler(TextureKey key, out ImageHandler cachedImage);

        void Bind(int index, ImageHandler image);

        void SetSampler(GalImage image, GalTextureSampler sampler);
    }
}