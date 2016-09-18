using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace T3
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private List<ShapePoint> Shape1;
        private List<ShapePoint> Shape2;
        private List<ShapePoint> CurrentShape;

        private Polygon _line;
        private bool _first = true;

        public MainWindow()
        {
            InitializeComponent();
            Shape1 = new List<ShapePoint>();
            Shape2 = new List<ShapePoint>();
            CurrentShape = Shape1;
            DrawCanvas.Background = Brushes.Azure;
            _line = new Polygon
            {
                Stroke = Brushes.Red,
                StrokeThickness = 2
            };
            DrawCanvas.Children.Add(_line);
            var l = new List<int>();
            l.Add(1);
            l.Add(2);
            l.Insert(1,3);
        }

        private void DrawCanvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            CurrentShape.Add(new ShapePoint(e.GetPosition(DrawCanvas)));
            _line.Points.Add(CurrentShape[CurrentShape.Count-1].Coordinates);
            
        }

        private void AddShapeButton_Click(object sender, RoutedEventArgs e)
        {
            if(CurrentShape.Count<3)
                return;
            if (!_first)
            {
                AddShapeButton.IsEnabled = false;
                CurrentShape = null;
                _line = null;
                DrawCanvas.MouseUp -= DrawCanvas_MouseUp;
                FindButton.IsEnabled = true;
                return;
            }
            CurrentShape = Shape2;
            _line = new Polygon
            {
                Stroke = Brushes.Green,
                StrokeThickness = 2
            };
            DrawCanvas.Children.Add(_line);
            AddShapeButton.Content = "Додати другу фігуру";
            _first = false;
        }

        private bool FindIntersection(ShapePoint start1, ShapePoint end1, ShapePoint start2, ShapePoint end2, out ShapePoint inter)
        {
            var A1 = end1.Y - start1.Y;
            var B1 = start1.X - end1.X;
            var A2 = end2.Y - start2.Y;
            var B2 = start2.X - end2.X;
            var C1 = A1*start1.X + B1*start1.Y;
            var C2 = A2 * start2.X + B2 * start2.Y;
            var det = A1*B2 - A2*B1;
            if (Math.Abs(det) < 0.001)
            {
                inter = new ShapePoint(0, 0);
                return false;
            }
            var x = (B2 * C1 - B1 * C2) / det;
            var y = (A1 * C2 - A2 * C1) / det;

            /*if(Shape1.Count==3)
            {
                inter = new ShapePoint(x, y) { Type = ShapePoint.ShapeType.Intersection };
                return true;
            }*/
            var xMin = Math.Min(start2.X, end2.X);
            var xMax = Math.Max(start2.X, end2.X);
            var yMin = Math.Min(start2.Y, end2.Y);
            var yMax = Math.Max(start2.Y, end2.Y);
            var xMin2 = Math.Min(start1.X, end1.X);
            var xMax2 = Math.Max(start1.X, end1.X);
            var yMin2 = Math.Min(start1.Y, end1.Y);
            var yMax2 = Math.Max(start1.Y, end1.Y);


            if (xMin <= x && x <= xMax
                && yMin <= y && y <= yMax
                && xMin2 <= x && x <= xMax2
                && yMin2 <= y && y <= yMax2)//true)
            {
                inter = new ShapePoint(x, y) {Type = ShapePoint.ShapeType.Intersection};
                return true;
            }
            inter = new ShapePoint(0, 0);
            return false;
        }

        private void FindButton_Click(object sender, RoutedEventArgs e)
        {
            //Якщо за годинниковою стрілкою, то сама верхня завжде іде першою
            ShapePoint inter;
            var c = 0;
            var s1Copy = new List<ShapePoint>(Shape1);
            var s1Shift = 1;
            var s2Copy = new List<ShapePoint>(Shape2);
            var s2Shift = 1;
            for (var i = 0; i < Shape1.Count - 1; i++)
            {
                for (var j = 0; j < Shape2.Count - 1; j++)
                {
                    if (FindIntersection(Shape1[i], Shape1[i + 1], Shape2[j], Shape2[j + 1], out inter))
                    {
                        s1Copy.Insert(i+s1Shift,inter);
                        s2Copy.Insert(j+s2Shift,inter);
                        s1Shift++;
                        s2Shift++;//Correct
                    }
                }
                //Todo: fix pasting
                if (FindIntersection(Shape1[i], Shape1[i + 1], Shape2[Shape2.Count-1], Shape2[0], out inter))
                {
                    s1Copy.Insert(i + s1Shift, inter);
                    s1Shift++;
                    s2Copy.Insert(s2Copy.Count,inter);
                }
            }
            for (var j = 0; j < Shape2.Count - 1; j++)
            {
                if (FindIntersection(Shape1[Shape1.Count-1], Shape1[0], Shape2[j], Shape2[j + 1], out inter))
                {
                    s1Copy.Insert(s1Copy.Count,inter);
                    s2Copy.Insert(j + s2Shift, inter);
                    s2Shift++;
                }
            }
            if (FindIntersection(Shape1[Shape1.Count - 1], Shape1[0], Shape2[Shape2.Count - 1], Shape2[0], out inter))
            {
                s1Copy.Insert(s1Copy.Count, inter);
                s2Copy.Insert(s2Copy.Count, inter);
            }
        }

        private void Draw(ShapePoint inter)
        {
            var d = new Ellipse
            {
                Width = 5,
                Height = 5,
                Fill = Brushes.Black
            };
            Canvas.SetTop(d, inter.Y);
            Canvas.SetLeft(d, inter.X);
            DrawCanvas.Children.Add(d);
        }
    }
}
