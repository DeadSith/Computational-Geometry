using System;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace T1
{
    internal class Circle : Figure
    {
        private Ellipse _ellipse;
        private Ellipse _clone;
        private Point Centre;

        public Circle(Canvas parent, double radius, int x, int y) : base(parent)
        {
            _ellipse = new Ellipse
            {
                Height = radius * 2,
                Width = radius * 2,
                Stroke = Brushes.Black
            };
            Centre = new Point(x, y);
            Canvas.SetTop(_ellipse, CanvasCentre.Y - y - radius);
            Canvas.SetLeft(_ellipse, CanvasCentre.X + x - radius);
            ParentCanvas.Children.Add(_ellipse);
        }

        public override void Move(int xChange, int yChange = 0)
        {
            Centre += new Point(xChange, yChange);
            Canvas.SetTop(_ellipse, Canvas.GetTop(_ellipse) - yChange);
            Canvas.SetLeft(_ellipse, Canvas.GetLeft(_ellipse) + xChange);
            var diff = ParentCanvas.ActualWidth / 2 - Math.Abs(Centre.X) + _ellipse.Width;
            if (diff < 0)
            {
                Canvas.SetLeft(_ellipse, Math.Abs(diff));
                Centre = new Point((int)(-ParentCanvas.ActualWidth / 2 + _ellipse.Width / 2), Centre.Y);
            }
        }

        public static bool CheckCollision(Circle c1, Circle c2)
        {
            var d = c1.Centre - c2.Centre;
            return Math.Sqrt(d.X * d.X + d.Y * d.Y) < (c1._ellipse.Width + c2._ellipse.Width) / 2;
        }
    }
}