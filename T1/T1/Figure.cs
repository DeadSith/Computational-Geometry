using System.Windows.Controls;

namespace T1
{
    public struct Point
    {
        public int X;
        public int Y;

        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }

        public static Point operator +(Point p1, Point p2)
        {
            return new Point(p1.X + p2.X, p1.Y + p2.Y);
        }

        public static Point operator -(Point p1, Point p2)
        {
            return new Point(p1.X - p2.X, p1.Y - p2.Y);
        }
    }

    public abstract class Figure
    {
        protected readonly Point CanvasCentre;
        protected readonly Canvas ParentCanvas;

        protected Figure(Canvas parent)
        {
            ParentCanvas = parent;
            CanvasCentre = new Point((int)parent.ActualWidth / 2, (int)parent.ActualHeight / 2);
        }

        public abstract void Move(int xChange, int yChange = 0);
    }
}