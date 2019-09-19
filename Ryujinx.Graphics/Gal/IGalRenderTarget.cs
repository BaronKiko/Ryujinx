using Ryujinx.Graphics.Gal.OpenGL;

namespace Ryujinx.Graphics.Gal
{
    public interface IGalRenderTarget
    {
        void Bind();

        void BindColor(TextureKey key, int attachment);

        void UnbindColor(int attachment);

        void BindZeta(TextureKey key);

        void UnbindZeta();

        void Present(TextureKey key);

        void SetMap(int[] map);

        void SetTransform(bool flipX, bool flipY, int top, int left, int right, int bottom);

        void SetWindowSize(int width, int height);

        void SetViewport(int attachment, int x, int y, int width, int height);

        void Render();

        void Copy(
            GalImage srcImage,
            GalImage dstImage,
            long     srcKey,
            int      srcSize,
            long     dstKey,
            int      dstSize,
            int      srcX0,
            int      srcY0,
            int      srcX1,
            int      srcY1,
            int      dstX0,
            int      dstY0,
            int      dstX1,
            int      dstY1);

        void Reinterpret(ImageHandler handler, GalImage newImage);
    }
}