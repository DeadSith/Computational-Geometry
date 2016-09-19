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
        public List<ShapePoint> Intersections = new List<ShapePoint>();
        public bool Visited = false;

        public int Index1;
        public int Index2;

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

        public void SortIntersections()
        {
            if (Intersections.Count <= 1) return;
            var sortByY = (this.X - Intersections[0].X) < (this.Y - Intersections[0].Y);
            int koef = 1;
            if (sortByY)
            {
                if (Y > Intersections[0].Y)
                    koef = -1;
            }
            else
            {
                if (X > Intersections[0].X)
                    koef = -1;
            }
            //sort by closest to point = first
            Intersections.Sort((p1, p2) =>
            {
                if (sortByY)
                {
                    if (p1.Y < p2.Y)
                        return -1*koef;
                    else if (p1.Y > p2.Y)
                        return 1*koef;
                }
                else
                {
                    if (p1.X<p2.X)
                        return -1*koef;
                    if (p1.X > p2.X)
                        return 1*koef;
                }
                return 0;
                //var d1 = DistanceSquared(p1, this);
                //var d2 = DistanceSquared(p2, this);
                //if (d1 - d2 < 0.01) return 0;
                //if (d1 < d2) return -1;
                //if (d1 > d2) return 1;
                //return 0;
            });
        }

        private static double DistanceSquared(ShapePoint p1, ShapePoint p2)
        {
            return (p2.X - p1.X) * (p2.X - p1.X) + (p2.Y - p1.Y) * (p2.Y - p1.Y);
        }
    }
}
