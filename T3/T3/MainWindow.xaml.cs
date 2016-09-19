﻿using System;
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
        private List<ShapePoint> Intersections;

        private Polygon _line;
        private bool _first = true;
        private int interCount = 0;


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
            //var l = new List<int>();
            //l.Add(1);
            //l.Add(2);
            //l.Add(4);
            //l.Insert(1,3);
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
            AddIntersections();
            var sCopy = new List<ShapePoint>(Shape1);
            //wrong insert
            foreach (var point in Shape1)
            {
                point.SortIntersections();
                var index = point.FindIndex(sCopy);
                sCopy.InsertRange(index+1,point.Intersections);
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
                var start = FindStart();
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
                _line = new Polygon {Fill = Brushes.Gold};
                foreach (var point in sCopy)
                {
                    _line.Points.Add(point.Coordinates);
                }
                DrawCanvas.Children.Add(_line);
                allVisited = Intersections.All(point => point.Visited);
            }
        }

        private ShapePoint FindStart()
        {
            foreach (ShapePoint t in Shape1)
            {
                if (t.Type == ShapePoint.ShapeType.Intersection && !t.Visited)
                    return t;
            }
            throw  new ArgumentException();
        }

        private void AddIntersections()
        {
            Intersections = new List<ShapePoint>();
            ShapePoint inter;
            for (var i = 0; i < Shape1.Count - 1; i++)
            {
                for (var j = 0; j < Shape2.Count - 1; j++)
                {
                    if (FindIntersection(Shape1[i], Shape1[i + 1], Shape2[j], Shape2[j + 1], out inter))
                    {
                        Draw(inter);
                        Shape1[i].Intersections.Add(inter);
                        Shape2[j].Intersections.Add(inter);
                    }
                }
                if (FindIntersection(Shape1[i], Shape1[i + 1], Shape2[Shape2.Count - 1], Shape2[0], out inter))
                {
                    Draw(inter);
                    Shape1[i].Intersections.Add(inter);
                    Shape2[Shape2.Count - 1].Intersections.Add(inter);
                }
                //Todo: fix pasting
            }
            for (var j = 0; j < Shape2.Count - 1; j++)
            {
                if (FindIntersection(Shape1[Shape1.Count-1], Shape1[0], Shape2[j], Shape2[j + 1], out inter))
                {
                    Draw(inter);
                    Shape1[Shape1.Count - 1].Intersections.Add(inter);
                    Shape2[j].Intersections.Add(inter);
                }
            }
            if (FindIntersection(Shape1[Shape1.Count - 1], Shape1[0], Shape2[Shape2.Count - 1], Shape2[0], out inter))
            {
                Draw(inter);
                Shape1[Shape1.Count - 1].Intersections.Add(inter);
                Shape2[Shape2.Count - 1].Intersections.Add(inter);
            }
        }

        private void Draw(ShapePoint inter)
        {
            interCount++;
            Intersections.Add(inter);
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
