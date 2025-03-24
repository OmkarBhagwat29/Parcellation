using Rhino;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RhinoProjects.UrbanDesign.Logic
{
    public class Mover
    {
        public static List<Brep> breps = new List<Brep>();
        public Point3d Origin { get; private set; }
        public int Count { get;  set; }

        public List<Line> Lines { get; private set; }

        const double rad = int.MaxValue;

        public Mover(Point3d pt, int armCounts)
        {
            this.Origin = pt;
            this.Count = armCounts;
            this.Lines = new List<Line>();

            this.TransfomrRays();

            //rad = 56;
        }



        public void Update(Point3d newPt)
        {
            this.Origin = newPt;
            this.Lines.Clear();

            TransfomrRays();

        }

        private void TransfomrRays()
        {

            //Vector3d dir = Vector3d.XAxis;
            //double angle = 360 / Count;

            //for (int i = 0; i < 360; i += density)
            //{
            //    double angle = i;
            //    rays.Add(new Ray(pos, angle));
            //}

            for (int i = 0; i < 360; i += this.Count)
            {

                Vector3d dir = new Vector3d(Math.Cos(RhinoMath.ToRadians(i)), Math.Sin(RhinoMath.ToRadians(i)), 0);

                Line ln = new Line(Origin, dir, rad);
                Lines.Add(ln);


            }
        }


        public List<Line> FindIntersectionLines(List<Brep> breps, out List<double> dists)
        {
            List<Line> lns = new List<Line>();
            dists = new List<double>();

            foreach (Line line in Lines)
            {
                Point3d closePt = Point3d.Unset;
                double d = double.MaxValue;

                List<Point3d> pts = new List<Point3d>();
                foreach (Brep brep in breps)
                {
                    Curve c = line.ToNurbsCurve();
                    c.Domain = new Interval(0, 1);

                    if (Rhino.Geometry.Intersect.Intersection.CurveBrep(c, brep,
                        RhinoDoc.ActiveDoc.ModelAbsoluteTolerance,
                        out Curve[] overlapCvs,
                        out Point3d[] ts))
                    {

                        pts.AddRange(ts);


                        foreach (Point3d p in pts)
                        {

                            double mD = this.Origin.DistanceToSquared(p);
                            if (mD < d)
                            {
                                d = mD;
                                closePt = p;
                            }
                        }



                    }

                }

                Line ln = new Line(this.Origin, closePt);
                lns.Add(ln);
                dists.Add(ln.Length);
            }


            return lns;

        }
    }
}
