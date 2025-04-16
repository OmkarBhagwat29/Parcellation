using Rhino.Geometry;


namespace UrbanDesign.Helper.Geometry
{
    public static class CurveHelper
    {
        public static List<Curve> SplitCurveWithCurves(this Curve curve, List<Curve> otherCurves,double tol = 0.01) {

            var allCurves = new List<Curve> { curve };
            allCurves.AddRange(otherCurves);

            // Boolean split the regions
            CurveBooleanRegions splitRegions = Curve.CreateBooleanRegions(allCurves, Plane.WorldXY, false, tol);

            var closedParcels = new List<Curve>();
            if (splitRegions != null)
            {

                for (int i = 0; i < splitRegions.RegionCount; i++)
                {
                    
                    var regions = splitRegions.RegionCurves(i).ToList();

                    regions.ForEach(r =>
                    {
                        if (r.IsClosed)
                        {
                            var ori = r.ClosedCurveOrientation(new Vector3d(0, 0, 1));
                            if (ori != CurveOrientation.CounterClockwise)
                            {
                                r.Reverse();
                            }
                            closedParcels.Add(r);
                        }
                        //RhinoDoc.ActiveDoc.Objects.AddCurve(r);
                    });
                }
            }

            return closedParcels;

        }

        public static List<Line> GetBoundingEdges(this Curve curve)
        {
            var plane = GetPlane(curve);

            if (!plane.IsValid)
                return null;

            var bbx = curve.GetBoundingBox(plane);


            var edges = bbx.GetEdges().Take(4).Select(l =>
            {

                l.Transform(Transform.PlaneToPlane(Plane.WorldXY, plane));
                return l;
            }).OrderByDescending(e => e.Length).ToList();

            return edges;
        }

        public static List<Curve> SubDivideClosedCurve(this Curve curve,
            double requiredDepth,
            double requiredWidth,
            out double curveWidth, out double curveDepth,
            double tweenTolerance = 0.01,
            bool limitTo2 = false)
        {
            curveWidth = double.NaN;
            curveDepth = double.NaN;

            var edges = curve.GetBoundingEdges();
            if (edges == null)
                return null;

            var vDim = edges[0].Length;
            var uDim = edges[2].Length;

            curveDepth = vDim;
            curveWidth = uDim;

            var vCv_1 = edges[0].ToNurbsCurve();
            var bLn = edges[1];
            bLn.Flip();
            var vCv_2 = bLn.ToNurbsCurve();

            var vCount = (int)Math.Ceiling(uDim / requiredDepth) - 1;
            var uCount = (int)Math.Ceiling(vDim / requiredWidth) - 1;

            if (limitTo2)
            {
                vCount = Math.Min((int)Math.Floor(uDim / requiredDepth), 1);
            }


            var vCvs = Curve.CreateTweenCurves(vCv_1, vCv_2, vCount, tweenTolerance);


            var uCv_1 = edges[2].ToNurbsCurve();
            var vLn = edges[3];
            vLn.Flip();
            var uCv_2 = vLn.ToNurbsCurve();


            var uCvs = Curve.CreateTweenCurves(uCv_1, uCv_2, uCount, tweenTolerance);


            var allCurves = new List<Curve>();
            allCurves.AddRange(uCvs);
            allCurves.AddRange(vCvs);

            return allCurves;
        }


        public static Plane GetPlane(this Curve curve)
        {
            // Get the longest segment
            Line? seg = curve.DuplicateSegments()
                .Select(c => new Line(c.PointAtStart, c.PointAtEnd))
                .OrderByDescending(l => l.Length)
                .FirstOrDefault();

            if (seg == null)
                return Plane.Unset;

            // Define X-axis using the segment's direction
            Vector3d xAxis = seg.Value.Direction;
            xAxis.Unitize();

            // Define Y-axis (perpendicular to X, lying in the XY plane)
            Vector3d yAxis = Vector3d.CrossProduct(Vector3d.ZAxis, xAxis);
            yAxis.Unitize();

            // Create a plane with the segment's start point
            Plane plane = new Plane(seg.Value.From, xAxis, yAxis);

            return plane;
        }
    }
}
