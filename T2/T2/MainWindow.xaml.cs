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

namespace T2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    public struct Complex
    {
        public double Re, Im;

        public Complex(Complex c)
        {
            Re = c.Re;
            Im = c.Im;
        }

        public Complex(double re, double im)
        {
            Re = re;
            Im = im;
        }
    }

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private int _xCentre, _yCentre, _maxIterations;
        private double _endValue;
        private WriteableBitmap _wb;

        private void DrawButton_Click(object sender, RoutedEventArgs e)
        {
            DrawButton.IsEnabled = false;
            _xCentre = int.Parse(WidthText.Text)/2;
            _yCentre = int.Parse(HeightText.Text)/2;
            _maxIterations = int.Parse(IterText.Text);
            _endValue = 5;
            _wb = BitmapFactory.New(_xCentre*2, _yCentre*2);
            _wb.Clear(Colors.White);
            FractalCanvas.Source = _wb;
            var re = double.Parse(ReText.Text);
            var im = double.Parse(ImText.Text);
            DrawFractal(new Complex(re,im), new Complex(1,3));
        }

        private void DrawFractal(Complex z, Complex lambdaInput = default(Complex))
        {
            var zOriginal = new Complex(z);
            for (var y = 0; y < _yCentre*2; y++)
            {
                for (var x = 0; x < _xCentre*2; x++)
                {
                    var n = 0;
                    var lambda = new Complex();
                    lambda.Re = (x-_xCentre) * 0.01 + 1;
                    lambda.Im = (y-_yCentre) * 0.01;
                    /*lambda.Re = (x - _xCentre) * (lambdaInput.Re/100)+1;
                    lambda.Im = (y - _yCentre) * (lambdaInput.Im/100);*/
                    z = new Complex(zOriginal);
                    while ((z.Re*z.Re+z.Im*z.Im<_endValue)&&(n<_maxIterations))
                    {
                        var p = z.Re - z.Re * z.Re + z.Im * z.Im;
                        var q = z.Im - 2 * z.Re * z.Im;
                        z.Re = lambda.Re * p - lambda.Im * q;
                        z.Im = lambda.Re * q + lambda.Im * p;
                        n++;
                    }
                    if (n < _maxIterations)
                    {
                        DrawPixel(x,y,Color.FromArgb(255, 0, (byte)((n * 15) % 255), (byte)((n * 20) % 255)));
                    }
                }
            }
            DrawButton.IsEnabled = true;
        }

        private void DrawPixel(int x, int y, Color color)
        {
            _wb.SetPixel(x,y,color);
        }
    }
}
