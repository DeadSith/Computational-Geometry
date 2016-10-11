using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using static System.Math;

namespace T4
{
    public class Triangle
    {
        public Vector3D Edge1;
        public Vector3D Edge2;
        public Vector3D Edge3;
        public bool Visible = false;
        public readonly Brush Color;
        private static int count = 0;
        private int c;
        private double checkKoef = 1;

        public Triangle(Vector3D edge1, Vector3D edge2, Vector3D edge3, Brush color)
        {
            Edge1 = edge1;
            Edge2 = edge2;
            Edge3 = edge3;
            Color = color;
            c = count++;
        }

        public Point3D GetNormal()
        {
            var x = (Edge2.Y - Edge1.Y)*(Edge3.Z - Edge1.Z) - (Edge2.Z - Edge1.Z)*(Edge3.Y - Edge1.Y);
            var y = (Edge2.Z - Edge1.Z)*(Edge3.X - Edge1.X) - (Edge2.X - Edge1.X)*(Edge3.Z - Edge1.Z);
            var z = (Edge2.X - Edge1.X)*(Edge3.Y - Edge1.Y) - (Edge2.Y - Edge1.Y)*(Edge3.X - Edge1.X);
            return new Point3D(x,y,z);
        }

        public void Check(double c)
        {
            var centre = new Point3D((Edge1.X+Edge2.X+Edge3.X)/3, (Edge1.Y + Edge2.Y + Edge3.Y) / 3, (Edge1.Z + Edge2.Z + Edge3.Z) / 3);
            var l = new Point3D(centre.X,centre.Y,centre.Z-c);
            var n = GetNormal();
            var res = l.X*n.X + l.Y*n.Y + l.Z*n.Z;
            if (res < checkKoef)
                Visible = true;
        }

        private void Transform(Matrix3D transformMatrix)
        {
            Visible = false;
                Edge1 *= transformMatrix;
                Edge2 *= transformMatrix;
                Edge3 *= transformMatrix;
        }

        public void ShiftZ(double angle)
        {
            var matrix = new Matrix3D
            {
                M11 = Cos(angle),
                M12 = -Sin(angle),
                M21 = Sin(angle),
                M22 = Cos(angle),
                M33 = 1,
                M44 = 1
            };
            Transform(matrix);
        }

        public void ShiftY(double angle)
        {
            var matrix = new Matrix3D
            {
                M11 = Cos(angle),
                M13 = -Sin(angle),
                M22 = 1,
                M31 = Sin(angle),
                M33 = Cos(angle),
                M44 = 1
            };
            Transform(matrix);
        }

        public void ShiftX(double angle)
        {
            var matrix = new Matrix3D
            {
                M11 = 1,
                M22 = Cos(angle),
                M23 = Sin(angle),
                M32 = -Sin(angle),
                M33 = Cos(angle),
                M44 = 1
            };
            Transform(matrix);
        }

        public void Resize(double koef)
        {
            checkKoef /= koef;
            var matrix = new Matrix3D
            {
                M11 = koef,
                M22 = koef,
                M33 = Cos(koef),
                M44 = 1
            };
            Transform(matrix);
        }
    }
}
