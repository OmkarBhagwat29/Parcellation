using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UrbanDesign.Logic
{

    public class LandParcel
    {
        public double Width { get; set; }
        public double Length { get; set; }

        public double FSI { get; set; }

        public double PlotArea { get; private set; }

        public double BuilUp_Area { get; set; }

        public Rectangle3d PlotRectangle { get; set; }

        public Curve PlotCurve { get; set; }

        public List<Curve> PlotCurves { get; set; } = new List<Curve>();

        public int FloorCount { get; set; }

        public  double PerFloorArea { get; set; }

        public double SiteCoverage { get; set; }

        public LandParcel(double _plotWidth,double _plotLen,
            double _fsi,double _siteCoverage)
        {
            this.Width = _plotWidth;
            this.Length = _plotLen;
            this.FSI = _fsi;
            this.SiteCoverage = _siteCoverage;

            this.PlotArea = this.Width * this.Length;

            this.PlotRectangle = new Rectangle3d(Plane.WorldXY,
                new Interval(-this.Width / 2, this.Width / 2),
                new Interval(-this.Length / 2, this.Length / 2));

            this.PlotCurve = this.PlotRectangle.ToNurbsCurve();
            this.PlotCurve.Domain = new Interval(0, 1);

            this.PlotCurves = this.PlotCurve.DuplicateSegments().ToList();
            this.PlotCurves.ForEach(c => c.Domain = new Interval(0, 1));

            this.BuilUp_Area = this.PlotArea * this.FSI;

        }

        private void InIt()
        {
            this.SetFloorCount();


            //set per floor area
            this.PerFloorArea = this.BuilUp_Area / this.FloorCount;
        }


        public void SetFloorCount()
        {
            double div = this.BuilUp_Area / this.PlotArea;

            this.FloorCount = (int)Math.Ceiling(div);
        }

        public Curve GetNorthCurve(int northCurveIndex) => this.PlotCurves[northCurveIndex];
        public Curve GetRoadFrontCurve(int frontCurveIndex) => this.PlotCurves[frontCurveIndex];

        public Rectangle3d GetBuildingRectanlge(out double width,out double length)
        {
            double currArea = double.MaxValue;
            double pastArea = currArea;

            double w = this.Width / 2.0;
            double l = this.Length / 2.0;

            double footPrintArea = (this.SiteCoverage / 100.0) * this.PlotArea;

            Rectangle3d rec = Rectangle3d.Unset;

            int ct = 1000;
            while (currArea>= footPrintArea&&ct>0)
            {
                w -= 0.10;
                l -= 0.10;

                rec = new Rectangle3d(Plane.WorldXY,
                    new Interval(-w, w), new Interval(-l, l));

                currArea = rec.Area;

                ct--;

                //if (pastArea < currArea)
                //{
                //    pastArea = currArea;
                //}
            }

            width = w * 2;
            length = l * 2;

            return rec;
        }


    }
}
