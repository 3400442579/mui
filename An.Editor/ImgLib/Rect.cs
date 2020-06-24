namespace An.Image
{
    public struct Rect
    {
        public Rect(int x,int y,int width, int height) {
            X = x;
            Y = y;
            Height = height;
            Width = width;
        }

        public int Height { get; set; }
        public int Width { get; set; }

        public int X { get; set; }

        public int Y { get; set; }
    }
}
