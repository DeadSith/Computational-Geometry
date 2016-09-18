using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace T3
{
    class ShapePoint
    {
        public enum ShapeType
        {
            Strandard,
            Intersection
        }

        public readonly Point Coordinates;
        public double X => Coordinates.X;
        public double Y => Coordinates.Y;
        public ShapeType Type;

        public ShapePoint(Point p)
        {
            Coordinates = p;
            Type = ShapeType.Strandard;
        }

        public ShapePoint(double x, double y)
        {
            Coordinates = new Point(x,y);
            Type = ShapeType.Strandard;
        }

        public int FindIndex(List<ShapePoint> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i] == this)
                    return i;
            }
            throw new ArgumentException();
        }
    }
}
