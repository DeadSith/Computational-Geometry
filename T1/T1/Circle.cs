using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace T1
{
    internal class Circle : Figure
    {
        private readonly Ellipse _ellipse;

        public Circle(Canvas parent, double radius, int x, int y) : base(parent)
        {
            _ellipse = new Ellipse
            {
                Height = radius,
                Width = radius,
                Stroke = Brushes.Black
            };
            Canvas.SetTop(_ellipse, CanvasCentre.Y - y - radius / 2);
            Canvas.SetLeft(_ellipse, CanvasCentre.X + x - radius / 2);
            ParentCanvas.Children.Add(_ellipse);
        }

        public override void Move(int xChange, int yChange = 0)
        {
            Canvas.SetTop(_ellipse, Canvas.GetTop(_ellipse) - yChange);
            Canvas.SetLeft(_ellipse, Canvas.GetLeft(_ellipse) + xChange);
        }
    }
}