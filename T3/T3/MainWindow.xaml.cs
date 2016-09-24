using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
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
        private List<ShapePoint> InnerShape1;
        private List<ShapePoint> CurrentShape;
        private List<ShapePoint> Intersections;
        private List<List<ShapePoint>> Results = new List<List<ShapePoint>>();

        private Polygon _line;
        private bool _first = true;


        public MainWindow()
        {
            InitializeComponent();
            Shape1 = new List<ShapePoint>();
            Shape2 = new List<ShapePoint>();
            InnerShape1 = new List<ShapePoint>();
            CurrentShape = Shape1;
            DrawCanvas.Background = Brushes.Azure;
            _line = new Polygon
            {
                Stroke = Brushes.Red,
                StrokeThickness = 2
            };
            DrawCanvas.Children.Add(_line);
            //var l = new List<int>();
            //l.Add(1);
            //l.Add(2);
            //l.Add(4);
            //l.Insert(1,3);
        }

        #region Events
        
        private void DrawCanvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            CurrentShape.Add(new ShapePoint(e.GetPosition(DrawCanvas)));
            _line.Points.Add(CurrentShape[CurrentShape.Count - 1].Coordinates);

        }

        private void AddShapeButton_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentShape.Count < 3)
                return;
            if (!_first)
            {
                AddShapeButton.IsEnabled = false;
                _line = new Polygon
                {
                    Stroke = Brushes.DarkViolet,
                    StrokeThickness = 2
                };
                DrawCanvas.Children.Add(_line);
                //DrawCanvas.MouseUp -= DrawCanvas_MouseUp;
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

        private void FindButton_Click(object sender, RoutedEventArgs e)
        {
            //Якщо за годинниковою стрілкою, то сама верхня завжде іде першою
            AddIntersections(ref Shape1, ref Shape2);
            var sCopy = new List<ShapePoint>(Shape1);
            foreach (var point in Shape1)
            {
                point.SortIntersections();
                var index = point.FindIndex(sCopy);
                sCopy.InsertRange(index + 1, point.Intersections);
            }
            Shape1 = sCopy;
            sCopy = new List<ShapePoint>(Shape2);
            foreach (var point in Shape2)
            {
                point.SortIntersections();
                var index = point.FindIndex(sCopy);
                sCopy.InsertRange(index + 1, point.Intersections);
            }
            for (var i = 0; i < Shape1.Count; i++)
                Shape1[i].Index1 = i;
            Shape2 = sCopy;
            for (var i = 0; i < Shape2.Count; i++)
                Shape2[i].Index2 = i;
            var allVisited = false;
            while (!allVisited)
            {
                sCopy = new List<ShapePoint>();
                var currentShape = Shape1;
                var start = FindStart(Shape1);
                start.Visited = true;
                var ind = start.FindIndex(currentShape) + 1 != currentShape.Count ? start.FindIndex(currentShape) + 1 : 0;
                var current = currentShape[ind];
                sCopy.Add(start);
                do
                {
                    sCopy.Add(current);
                    if (current.Type == ShapePoint.ShapeType.Intersection)
                    {
                        current.Visited = true;
                        currentShape = currentShape == Shape1 ? Shape2 : Shape1;
                    }
                    var index = currentShape == Shape1 ? current.Index1 + 1 : current.Index2 + 1;
                    if (index == currentShape.Count)
                        index = 0;
                    current = currentShape[index];
                } while (current != start);

                _line = new Polygon { Fill = Brushes.Gold };
                foreach (var point in sCopy)
                {
                    _line.Points.Add(point.Coordinates);
                }
                DrawCanvas.Children.Add(_line);
                Results.Add(sCopy);
                allVisited = Intersections.All(point => point.Visited);
            }
            if (InnerShape1?.Count != 0)
            {
                var res = new List<List<ShapePoint>>();
                foreach (var t in Results)
                {
                    var k = t;
                    Cut(ref k, InnerShape1, ref res);
                }
                Results = res;
            }
            foreach (var shape in Results)
            {
                _line = new Polygon { Fill = Brushes.Gold };
                foreach (var point in shape)
                {
                    _line.Points.Add(point.Coordinates);
                }
                DrawCanvas.Children.Add(_line);
            }
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            DrawCanvas.Children.Clear();
            Shape1 = new List<ShapePoint>();
            Shape2 = new List<ShapePoint>();
            InnerShape1 = new List<ShapePoint>();
            CurrentShape = Shape1;
            DrawCanvas.Background = Brushes.Azure;
            _line = new Polygon
            {
                Stroke = Brushes.Red,
                StrokeThickness = 2
            };
            DrawCanvas.Children.Add(_line);
            _first = true;
            AddShapeButton.IsEnabled = true;
            AddInner.IsEnabled = true;
            FindButton.IsEnabled = false;
            //DrawCanvas.MouseUp += DrawCanvas_MouseUp;
            AddShapeButton.Content = "Додати першу фігуру";
            Results = new List<List<ShapePoint>>();
        }

        private void AddInner_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentShape.Count < 3)
                return;
            InnerShape1 = CurrentShape;
            Shape2 = new List<ShapePoint>();
            CurrentShape = Shape2;
            _line = new Polygon
            {
                Stroke = Brushes.Brown,
                StrokeThickness = 2
            };
            DrawCanvas.Children.Add(_line);
            AddInner.IsEnabled = false;
        }

        #endregion

        #region Help Methods
       
        private bool FindIntersection(ShapePoint start1, ShapePoint end1, ShapePoint start2, ShapePoint end2, out ShapePoint inter)
        {
            var A1 = end1.Y - start1.Y;
            var B1 = start1.X - end1.X;
            var A2 = end2.Y - start2.Y;
            var B2 = start2.X - end2.X;
            var C1 = A1 * start1.X + B1 * start1.Y;
            var C2 = A2 * start2.X + B2 * start2.Y;
            var det = A1 * B2 - A2 * B1;
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
                inter = new ShapePoint(x, y) { Type = ShapePoint.ShapeType.Intersection };
                return true;
            }
            inter = new ShapePoint(0, 0);
            return false;
        }

        private ShapePoint FindStart(List<ShapePoint> shapeToSearch)
        {
            foreach (ShapePoint t in shapeToSearch)
            {
                if (t.Type == ShapePoint.ShapeType.Intersection && !t.Visited)
                    return t;
            }
            throw new ArgumentException();
        }

        private void AddIntersections(ref List<ShapePoint> shape1, ref List<ShapePoint> shape2)
        {
            Intersections = new List<ShapePoint>();
            ShapePoint inter;
            for (var i = 0; i < shape1.Count - 1; i++)
            {
                for (var j = 0; j < shape2.Count - 1; j++)
                {
                    if (FindIntersection(shape1[i], shape1[i + 1], shape2[j], shape2[j + 1], out inter))
                    {
                        Draw(ref inter);
                        shape1[i].Intersections.Add(inter);
                        shape2[j].Intersections.Add(inter);
                    }
                }
                if (FindIntersection(shape1[i], shape1[i + 1], shape2[shape2.Count - 1], shape2[0], out inter))
                {
                    Draw(ref inter);
                    shape1[i].Intersections.Add(inter);
                    shape2[shape2.Count - 1].Intersections.Add(inter);
                }
            }
            for (var j = 0; j < shape2.Count - 1; j++)
            {
                if (FindIntersection(shape1[shape1.Count - 1], shape1[0], shape2[j], shape2[j + 1], out inter))
                {
                    Draw(ref inter);
                    shape1[shape1.Count - 1].Intersections.Add(inter);
                    shape2[j].Intersections.Add(inter);
                }
            }
            if (FindIntersection(shape1[shape1.Count - 1], shape1[0], shape2[shape2.Count - 1], shape2[0], out inter))
            {
                Draw(ref inter);
                shape1[shape1.Count - 1].Intersections.Add(inter);
                shape2[shape2.Count - 1].Intersections.Add(inter);
            }
        }

        private void Draw(ref ShapePoint inter)
        {
            inter.Type = ShapePoint.ShapeType.Intersection;
            Intersections.Add(inter);
            /*var d = new Ellipse
            {
                Width = 5,
                Height = 5,
                Fill = Brushes.Black
            };
            Canvas.SetTop(d, inter.Y);
            Canvas.SetLeft(d, inter.X);
            DrawCanvas.Children.Add(d);*/
        }

        private void Cut(ref List<ShapePoint> shapeToCut, List<ShapePoint> cutter, ref List<List<ShapePoint>> resList)
        {
            foreach (var point in shapeToCut)
            {
                point.Intersections.Clear();
                point.Type = ShapePoint.ShapeType.Strandard;
            }
            foreach (var point in cutter)
            {
                point.Intersections.Clear();
                point.Type = ShapePoint.ShapeType.Strandard;
            }
            AddIntersections(ref shapeToCut, ref cutter);
            var sCopy = new List<ShapePoint>(shapeToCut);
            foreach (var point in shapeToCut)
            {
                point.SortIntersections();
                var index = point.FindIndex(sCopy);
                sCopy.InsertRange(index + 1, point.Intersections);
            }
            shapeToCut = sCopy;
            sCopy = new List<ShapePoint>(cutter);
            foreach (var point in cutter)
            {
                point.SortIntersections();
                var index = point.FindIndex(sCopy);
                sCopy.InsertRange(index + 1, point.Intersections);
            }
            cutter = sCopy;
            for (var i = 0; i < shapeToCut.Count; i++)
                shapeToCut[i].Index1 = i;
            for (var i = 0; i < cutter.Count; i++)
                cutter[i].Index2 = i;
            var allVisited = false;
            while (!allVisited)
            {
                sCopy = new List<ShapePoint>();
                var currentShape = cutter;
                var start = FindStart(shapeToCut);
                start.Visited = true;
                var ind = start.FindIndex(currentShape) != 0 ? start.FindIndex(currentShape) - 1 : currentShape.Count - 1;
                var current = currentShape[ind];
                sCopy.Add(start);
                do
                {
                    sCopy.Add(current);
                    if (current.Type == ShapePoint.ShapeType.Intersection)
                    {
                        current.Visited = true;
                        currentShape = currentShape == shapeToCut ? cutter : shapeToCut;
                    }
                    var index = currentShape == shapeToCut ? current.Index1 - 1 : current.Index2 - 1;
                    if (index < 0)
                        index = currentShape.Count - 1;
                    current = currentShape[index];
                } while (current != start);
                //resList.Add(sCopy);
                //костиль
                _line = new Polygon { Fill = Brushes.Azure };
                foreach (var point in sCopy)
                {
                    _line.Points.Add(point.Coordinates);
                }
                DrawCanvas.Children.Add(_line);
                allVisited = Intersections.All(point => point.Visited);
            }
        }

        #endregion
    }
}
