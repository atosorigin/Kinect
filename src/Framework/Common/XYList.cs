using System.Collections.Generic;

namespace Common
{
    public class XYList<T> : List<T>
    {
        public int Width { get; private set; }
        public int Height { get; private set; }

        public XYList(int width, int height)
            : base(width * height)
        {
            Width = width;
            Height = height;
        }

        public T this[int x, int y]
        {
            get { return this[x + y * Width]; }
            set { this[x + y * Width] = value; }
        }
    }
}
