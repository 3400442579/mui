namespace Ani.IMG
{
    public struct Rect
    {
        public Rect(int x,int y,int width, int height) {
            X = x;
            Y = y;
            Height = height;
            Width = width;
        }
        public Rect(uint x, uint y, uint width, uint height)
        {
            X = (int)x;
            Y = (int)y;
            Height = (int)height;
            Width = (int)width;
        }

        public int Height { get; set; }
        public int Width { get; set; }

        public int X { get; set; }

        public int Y { get; set; }
    }
}
