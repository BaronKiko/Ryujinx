using System;

namespace Ryujinx.Graphics.Gal.OpenGL
{
    public struct TextureKey : IEquatable<TextureKey>
    {
        public long Position;
        public int  Size;

        public GalTextureTarget Target;
        
        public TextureKey(long position, int size, GalTextureTarget target)
        {
            Position = position;
            Size     = size;
            Target   = target;
        }

        public bool Equals(TextureKey other)
        {
            return (Position == other.Position &&
                    Size     == other.Size &&
                    Target   == other.Target);
        }
    }
}
