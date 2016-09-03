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
        private Circle _c2;
        private double _c1MoveSpeed;
        private double _c2MoveSpeed;
        private int _direction;
        private int _c1Multiplier;
        private int _c2Multiplier;
        private DispatcherTimer _timer1;
        private DispatcherTimer _timer2;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void DrawCanvas_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private void OnTick1(object sender, EventArgs e)
        {
            _c1.Move(_c1Multiplier);
            if (Circle.CheckCollision(_c1, _c2))
            {
                _timer1.Stop();
                _timer2.Stop();
            }
        }

        private void OnTick2(object sender, EventArgs e)
        {
            _c2.Move(_c2Multiplier * _direction);
            if (Circle.CheckCollision(_c1, _c2))
            {
                _timer1.Stop();
                _timer2.Stop();
            }
        }

        private void BeginButton_Click(object sender, RoutedEventArgs e)
        {
            DrawCanvas.Children.Clear();
            var line1 = new Line
            {
                X1 = 0,
                X2 = DrawCanvas.ActualWidth,
                Y1 = DrawCanvas.ActualHeight * 2 / 3,
                Y2 = DrawCanvas.ActualHeight * 2 / 3,
                StrokeThickness = 2,
                Stroke = Brushes.Black
            };
            DrawCanvas.Children.Add(line1);
            _c1Multiplier = _c2Multiplier = 1;
            _c1MoveSpeed = int.Parse(C1Box.Text);
            while (_c1MoveSpeed > 101)
            {
                _c1Multiplier *= 2;
                _c1MoveSpeed /= 2;
            }
            _c2MoveSpeed = int.Parse(C2Box.Text);
            while (_c2MoveSpeed > 101)
            {
                _c2Multiplier *= 2;
                _c2MoveSpeed /= 2;
            }
            if (DirectionBox.SelectedIndex == 1)
            {
                _direction = -1;
                var c2Radius = double.Parse(C2RadiusBox.Text);
                _c2 = new Circle(DrawCanvas, c2Radius, (int)(DrawCanvas.ActualWidth / 2 - c2Radius),
                    (int)(-DrawCanvas.ActualHeight / 6 + c2Radius));
            }
            else
            {
                _direction = 1;
                var c2Radius = double.Parse(C2RadiusBox.Text);
                _c2 = new Circle(DrawCanvas, c2Radius, 0, (int)(-DrawCanvas.ActualHeight / 6 + c2Radius));
            }
            var c1Radius = double.Parse(C1RadiusBox.Text);
            _c1 = new Circle(DrawCanvas, c1Radius, (int)(-DrawCanvas.ActualWidth / 2 + c1Radius), (int)(-DrawCanvas.ActualHeight / 6 + c1Radius));
            _timer1 = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(1000.0 / _c1MoveSpeed) };
            _timer1.Tick += OnTick1;
            _timer2 = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(1000.0 / _c2MoveSpeed) };
            _timer2.Tick += OnTick2;
            _timer2.Start();
            _timer1.Start();
        }
    }
}