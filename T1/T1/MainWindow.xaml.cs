using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace T1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Circle _c1;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void DrawCanvas_Loaded(object sender, RoutedEventArgs e)
        {
            var line1 = new Line
            {
                X1 = 0,
                X2 = DrawCanvas.ActualWidth,
                Y1 = DrawCanvas.ActualHeight / 2,
                Y2 = DrawCanvas.ActualHeight / 2,
                StrokeThickness = 2,
                Stroke = Brushes.Black
            };
            DrawCanvas.Children.Add(line1);
            var line2 = new Line
            {
                X1 = DrawCanvas.ActualWidth / 2,
                X2 = DrawCanvas.ActualWidth / 2,
                Y1 = 0,
                Y2 = DrawCanvas.ActualHeight,
                StrokeThickness = 2,
                Stroke = Brushes.Black
            };
            DrawCanvas.Children.Add(line2);
            _c1 = new Circle(DrawCanvas, 50, 20, 20);
            var timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(16.6) };
            timer.Tick += OnTick;
            timer.Start();
        }

        private void OnTick(object sender, EventArgs e)
        {
            _c1.Move(5, 3);
        }
    }
}