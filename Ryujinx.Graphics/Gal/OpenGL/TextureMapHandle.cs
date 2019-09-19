namespace Ryujinx.Graphics.Gal.OpenGL
{
    public struct TextureMapHandle
    {
        public long Position;
        public long Size;

        // -1 means all layers/levels
        public int Layer;
        public int Level;

        public ImageHandler Image;
    }
}
