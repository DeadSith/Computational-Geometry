using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;


namespace T4
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public double C=0.01;
        public double Zoom = 100;
        public Triangle[] Triangles;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void MakeTriangles()
        {
            var c = 0;
            var points = new List<Vector3D>
            {
                new Vector3D(c, c+1, c),
                new Vector3D(c+1, c, c),
                new Vector3D(c, c, c-1),
                new Vector3D(c-1, c, c),
                new Vector3D(c, c, c+1),
                new Vector3D(c, c-1, c)
            };
            var triangles = new List<Triangle>
            {
                new Triangle(points[0], points[1], points[2],Brushes.Yellow),
                new Triangle(points[0], points[2], points[3],Brushes.Black),
                new Triangle(points[0], points[3], points[4],Brushes.BlueViolet),
                new Triangle(points[0], points[4], points[1],Brushes.Maroon),
                new Triangle(points[5], points[4], points[3],Brushes.DarkMagenta),
                new Triangle(points[5], points[3], points[2],Brushes.DarkGoldenrod),
                new Triangle(points[5], points[2], points[1],Brushes.DarkGreen),
                new Triangle(points[5], points[1], points[4],Brushes.Firebrick)
            };
            Triangles = triangles.ToArray();
        }

        private void Draw()
        {
            foreach (var triangle in Triangles)
            {
                triangle.Check(C);
                if (triangle.Visible)
                {
                    var p1 = To2D(triangle.Edge1);
                    var p2 = To2D(triangle.Edge2);
                    var p3 = To2D(triangle.Edge3);
                    var pol = new Polygon
                    {
                        Fill = triangle.Color,
                        Stroke = Brushes.DarkSeaGreen,
                        StrokeThickness = 2
                    };
                    pol.Points.Add(new Point(p1.X,p1.Y));
                    pol.Points.Add(new Point(p2.X, p2.Y));
                    pol.Points.Add(new Point(p3.X, p3.Y));
                    DrawCanvas.Children.Add(pol);
                }
            }
        }

        private Vector To2D(Vector3D point)
        {
            var b = 1.0 - point.Z*C;
            return new Vector(point.X/b*Zoom+DrawCanvas.ActualWidth/2,point.Y/b*Zoom+DrawCanvas.ActualHeight/2);
        }
        
        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            MakeTriangles();
            Draw();
        }

        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Up || e.Key == Key.W)
            {
                foreach (var triangle in Triangles)
                {
                    triangle.ShiftX(0.16);
                }
                DrawCanvas.Children.Clear();
                Draw();
            }
            else if (e.Key == Key.Down || e.Key == Key.S)
            {
                foreach (var triangle in Triangles)
                {
                    triangle.ShiftX(-0.16);
                }
                DrawCanvas.Children.Clear();
                Draw();
            }
            else if (e.Key == Key.Left || e.Key == Key.A)
            {
                foreach (var triangle in Triangles)
                {
                    triangle.ShiftY(0.16);
                }
                DrawCanvas.Children.Clear();
                Draw();
            }
            else if (e.Key == Key.Right || e.Key == Key.D)
            {
                foreach (var triangle in Triangles)
                {
                    triangle.ShiftY(-0.16);
                }
                DrawCanvas.Children.Clear();
                Draw();
            }
            else if (e.Key == Key.Q)
            {
                foreach (var triangle in Triangles)
                {
                    triangle.ShiftZ(0.16);
                }
                DrawCanvas.Children.Clear();
                Draw();
            }
            else if (e.Key == Key.E)
            {
                foreach (var triangle in Triangles)
                {
                    triangle.ShiftZ(-0.16);
                }
                DrawCanvas.Children.Clear();
                Draw();
            }
            else if (e.Key == Key.OemPlus || e.Key == Key.Add)
            {
                Zoom *= 1.05;
                DrawCanvas.Children.Clear();
                Draw();
            }
            else if (e.Key == Key.OemMinus || e.Key == Key.Subtract)
            {
                Zoom *= 0.95;
                DrawCanvas.Children.Clear();
                Draw();
            }
        }
    }
}
