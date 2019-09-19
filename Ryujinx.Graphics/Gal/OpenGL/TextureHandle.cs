using System;

namespace Ryujinx.Graphics.Gal.OpenGL
{
    public struct TextureHandle : IEquatable<TextureHandle>
    {
        public int ParentHandle;
        public int ViewHandle;

        // -1 means all layers/levels
        public int Layer;
        public int Level;
        
        public TextureHandle(int parentHandle)
        {
            ParentHandle = parentHandle;
            Layer        = -1;
            Level        = 0;
            ViewHandle   = -1;
        }

        public TextureHandle(int parentHandle, int viewHandle, int layer, int level)
        {
            ParentHandle = parentHandle;
            Layer        = layer;
            Level        = level;
            ViewHandle   = viewHandle;
        }

        public bool Equals(TextureHandle other)
        {
            return (ParentHandle == other.ParentHandle &&
                    Layer  == other.Layer  &&
                    Level  == other.Level);
        }
    }
}
