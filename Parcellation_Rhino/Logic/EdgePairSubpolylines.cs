using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UrbanDesign.Logic
{
    class EdgePairSubpolylines
    {
        private Line edgeA;
        private Line edgeB;

        private Polyline triangle1;
        private Polyline trapezoid;
        private Polyline triangle2;
        private double triangle1Area = 0;
        private double trapezoidArea = 0;
        private double triangle2Area = 0;

        public EdgePairSubpolylines(Line edgeA, Line edgeB, Point3d projected0, Point3d projected1)
        {
            this.edgeA = edgeA;
            this.edgeB = edgeB;
            if (projected0.X != double.MaxValue)
            {
                List<Point3d> pts = new List<Point3d>();
                pts.Add(edgeA.To);
                pts.Add(projected0);
                pts.Add(edgeB.From);
                pts.Add(edgeA.To);
                triangle1Area = CalculateTriangleArea(pts[0], pts[1], pts[2]);
                triangle1 = new Polyline(pts);
            }
            if (projected1.X != double.MaxValue)
            {
                List<Point3d> pts = new List<Point3d>();
                pts.Add(edgeA.From);
                pts.Add(projected1);
                pts.Add(edgeB.To);
                pts.Add(edgeA.From);
                triangle2Area = CalculateTriangleArea(pts[0], pts[1], pts[2]);
                triangle2 = new Polyline(pts);
            }
            List<Point3d> ptsTrapezoid = new List<Point3d>();

            double tolerance = 0.0001;
            if (projected1.DistanceTo(edgeA.ClosestPoint(projected1, true)) < tolerance &&
            projected1.X != double.MaxValue) { ptsTrapezoid.Add(projected1); }
            else ptsTrapezoid.Add(edgeA.From);
            if (projected0.DistanceTo(edgeA.ClosestPoint(projected0, true)) < tolerance &&
              projected0.X != double.MaxValue) ptsTrapezoid.Add(projected0);
            else ptsTrapezoid.Add(edgeA.To);
            if (projected0.DistanceTo(edgeB.ClosestPoint(projected0, true)) < tolerance &&
              projected0.X != double.MaxValue) ptsTrapezoid.Add(projected0);
            else ptsTrapezoid.Add(edgeB.From);
            if (projected1.DistanceTo(edgeB.ClosestPoint(projected1, true)) < tolerance &&
              projected1.X != double.MaxValue) ptsTrapezoid.Add(projected1);
            else ptsTrapezoid.Add(edgeB.To);
            ptsTrapezoid.Add(ptsTrapezoid[0]);
            trapezoidArea = CalculateTriangleArea(ptsTrapezoid[0], ptsTrapezoid[1], ptsTrapezoid[2]);
            trapezoidArea += CalculateTriangleArea(ptsTrapezoid[0], ptsTrapezoid[3], ptsTrapezoid[2]);

            trapezoid = new Polyline(ptsTrapezoid);
        }
        public Polyline getTriangle1()
        {
            return triangle1;
        }
        public Polyline getTriangle2()
        {
            return triangle2;
        }
        public Polyline getTrapezoid()
        {
            return trapezoid;
        }
        public double getTotalArea()
        {
            return triangle1Area + triangle2Area + trapezoidArea;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="polyline"></param>
        /// <param name="singlePartArea"></param>
        /// <returns></returns>
        public List<Cut> getCuts(Polyline polyline, double singlePartArea)
        {
            List<Cut> cuts = new List<Cut>();
            List<Line> segments = polyline.GetSegments().ToList();
            int indexOfEdgeA = 0;
            int indexOfEdgeB = 0;
            for (int i = 0; i < segments.Count; i++)
            {
                if (edgeA.PointAt(0.5).DistanceTo(segments[i].PointAt(0.5)) < 0.001)
                {
                    indexOfEdgeA = i;
                }
                else if (edgeB.PointAt(0.5).DistanceTo(segments[i].PointAt(0.5)) < 0.001)
                {
                    indexOfEdgeB = i;
                }
            }

            int segmentsCovered = indexOfEdgeB - indexOfEdgeA + 1;// number of segments covered by a LineRing starting with edgeA and ending with edgeB (including)
                                                                  // Polygon's exterior ring is equal to [edgeA + segmentsBetweenEdgePair + edgeB + segmentsOutsideEdgePair]
            int segmentCountBetweenEdgePair = segmentsCovered - 2;
            int segmentCountOutsideEdgePair = segments.Count - segmentsCovered;
            // if edges are not connected directly, polygon has extra area adjacent to them
            Polyline polylineOutside1 = new Polyline();
            Polyline polylineOutside2 = new Polyline();

            if (segmentCountBetweenEdgePair > 1)
            {
                // calculate extra area bounded by segmentsBetweenEdgePair
                for (int i = 0; i < segmentCountBetweenEdgePair + 1; i++)
                {
                    int ptIdx = indexOfEdgeA + 1 + i;
                    if (ptIdx >= segments.Count) ptIdx -= segments.Count;
                    polylineOutside1.Add(polyline[ptIdx]);
                }
                polylineOutside1.Add(polylineOutside1[0]);
            }
            if (segmentCountOutsideEdgePair > 1)
            {
                // calculate extra area bounded by segmentCountOutsideEdgePair
                for (int i = 0; i < segmentCountOutsideEdgePair + 1; i++)
                {
                    int ptIdx = indexOfEdgeB + 1 + i;
                    if (ptIdx >= segments.Count) ptIdx -= segments.Count;
                    polylineOutside2.Add(polyline[ptIdx]);

                }
                polylineOutside2.Add(polylineOutside2[0]);
            }
            double areaOutside1 = 0, areaOutside2 = 0;
            if (polylineOutside1 != null && polylineOutside1.IsValid && polylineOutside1.Length > 0.01)
            {
                AreaMassProperties amp = AreaMassProperties.Compute(polylineOutside1.ToNurbsCurve());
                if (amp != null) areaOutside1 = amp.Area;
            }
            if (polylineOutside2 != null && polylineOutside2.IsValid && polylineOutside2.Length > 0.01)
            {
                AreaMassProperties amp = AreaMassProperties.Compute(polylineOutside2.ToNurbsCurve());
                if (amp != null) areaOutside2 = amp.Area;
            }

            // check first direction (areaOutside1 + T1 + Trapezoid + T2)
            if (areaOutside1 <= singlePartArea)
            {

                Line lineOfCut = new Line();
                if (areaOutside1 + triangle1Area > singlePartArea)
                {
                    double areaToCutAwayInTriangle = singlePartArea - areaOutside1;
                    double fraction = areaToCutAwayInTriangle / triangle1Area;
                    if (triangle1 != null)
                    {
                        Point3d projected0 = triangle1[1];
                        Line edgeWithPointOfCut = (edgeA.ClosestPoint(projected0, true).DistanceTo(projected0) < 0.000001) ? new Line(edgeA.To, projected0) : new Line(edgeB.From, projected0);
                        Point3d pointOfCut = edgeWithPointOfCut.PointAt(fraction);
                        lineOfCut = (edgeA.ClosestPoint(pointOfCut, true).DistanceTo(pointOfCut) < 0.000001) ? new Line(edgeB.From, pointOfCut) : new Line(pointOfCut, edgeA.To);
                    }

                }
                else if (areaOutside1 + triangle1Area + trapezoidArea >= singlePartArea)
                {

                    Line trapezoidOnEdgeA = trapezoid.SegmentAt(0);
                    Line trapezoidOnEdgeB = trapezoid.SegmentAt(2);
                    trapezoidOnEdgeA.Flip();
                    double areaToCutAway = singlePartArea - (areaOutside1 + triangle1Area);
                    double fraction = areaToCutAway / trapezoidArea;

                    double ia, ib;
                    if (Rhino.Geometry.Intersect.Intersection.LineLine(trapezoidOnEdgeA, trapezoidOnEdgeB, out ia, out ib))
                    {

                        Point3d intersectionPoint = trapezoidOnEdgeA.PointAt(ia);
                        double wholeDistance = intersectionPoint.DistanceTo(trapezoidOnEdgeA.From);
                        double distTo = intersectionPoint.DistanceTo(trapezoidOnEdgeA.To);
                        bool flip = true;
                        if (distTo > wholeDistance) { wholeDistance = distTo; flip = false; fraction = 1 - fraction; }
                        fraction = splitFractionTrapezoid(trapezoidOnEdgeA.Length, wholeDistance, trapezoidOnEdgeA.UnitTangent, fraction);
                        if (!flip) fraction = 1 - fraction;
                    }

                    Point3d pointOfCutOnEdgeA = trapezoidOnEdgeA.PointAt(fraction);
                    Point3d pointOfCutOnEdgeB = trapezoidOnEdgeB.PointAt(fraction);
                    lineOfCut = new Line(pointOfCutOnEdgeB, pointOfCutOnEdgeA);


                }
                else if (areaOutside1 + getTotalArea() >= singlePartArea)
                {

                    double areaToCutAwayInTriangle = singlePartArea - (areaOutside1 + triangle1Area + trapezoidArea);
                    double fraction = areaToCutAwayInTriangle / triangle2Area;
                    if (triangle1 != null)
                    {
                        Point3d projected1 = triangle2[1];
                        Line edgeWithPointOfCut = (edgeA.ClosestPoint(projected1, true).DistanceTo(projected1) < 0.000001) ? new Line(projected1, edgeA.From) : new Line(projected1, edgeB.To);
                        Point3d pointOfCut = edgeWithPointOfCut.PointAt(fraction);
                        lineOfCut = (edgeA.ClosestPoint(pointOfCut, true).DistanceTo(pointOfCut) < 0.000001) ? new Line(edgeB.To, pointOfCut) : new Line(pointOfCut, edgeA.From);
                    }
                }

                Rhino.Geometry.Intersect.CurveIntersections ci = Rhino.Geometry.Intersect.Intersection.CurveLine(polyline.ToNurbsCurve(), lineOfCut, 0.001, 0.001);
                int intCount = 0;
                foreach (Rhino.Geometry.Intersect.IntersectionEvent ie in ci) if (ie.IsPoint) intCount++;
                if (lineOfCut != null && intCount <= 2)
                {

                    PolylineCurve polyCurve = polyline.ToPolylineCurve();
                    double t0, t1;
                    polyCurve.ClosestPoint(lineOfCut.To, out t0);
                    polyCurve.ClosestPoint(lineOfCut.From, out t1);

                    if (t0 != t1)
                    {

                        Curve cutAway = polyCurve.Trim(new Interval(t0, t1));
                        if (cutAway != null)
                        {
                            Polyline plCutAway = cutAway.ToPolyline(0.000001, 0.000001, 0.001, 1000000).ToPolyline();
                            plCutAway.Add(plCutAway[0]);
                            plCutAway.ReduceSegments(0.000001);

                            Curve cutRemain = polyCurve.Trim(new Interval(t1, t0));
                            Polyline plCutRemain = cutRemain.ToPolyline(0.000001, 0.000001, 0.001, 1000000).ToPolyline();
                            plCutRemain.Add(plCutRemain[0]);
                            plCutRemain.ReduceSegments(0.000001);
                            cuts.Add(new Cut(lineOfCut.Length, plCutAway, plCutRemain));
                        }
                    }
                }
            }

            // check first direction (areaOutside1 + T1 + Trapezoid + T2)
            if (areaOutside2 <= singlePartArea)
            {

                Line lineOfCut = new Line();
                if (areaOutside2 + triangle2Area > singlePartArea)
                {
                    double areaToCutAwayInTriangle = singlePartArea - areaOutside2;

                    double fraction = areaToCutAwayInTriangle / triangle2Area;
                    if (triangle2 != null)
                    {
                        Point3d projected1 = triangle2[1];
                        Line edgeWithPointOfCut = (edgeA.ClosestPoint(projected1, true).DistanceTo(projected1) < 0.000001) ? new Line(edgeA.From, projected1) : new Line(edgeB.To, projected1);
                        Point3d pointOfCut = edgeWithPointOfCut.PointAt(fraction);
                        lineOfCut = (edgeA.ClosestPoint(pointOfCut, true).DistanceTo(pointOfCut) < 0.000001) ? new Line(pointOfCut, edgeB.To) : new Line(edgeA.From, pointOfCut);
                    }


                }
                else if (areaOutside2 + triangle2Area + trapezoidArea >= singlePartArea)
                {

                    Line trapezoidOnEdgeA = trapezoid.SegmentAt(0);
                    Line trapezoidOnEdgeB = trapezoid.SegmentAt(2);
                    trapezoidOnEdgeB.Flip();

                    double areaToCutAway = singlePartArea - (areaOutside2 + triangle2Area);
                    double fraction = areaToCutAway / trapezoidArea;
                    double ia, ib;
                    if (Rhino.Geometry.Intersect.Intersection.LineLine(trapezoidOnEdgeA, trapezoidOnEdgeB, out ia, out ib))
                    {

                        Point3d intersectionPoint = trapezoidOnEdgeA.PointAt(ia);
                        double wholeDistance = intersectionPoint.DistanceTo(trapezoidOnEdgeA.From);
                        double distTo = intersectionPoint.DistanceTo(trapezoidOnEdgeA.To);
                        bool flip = true;
                        if (distTo > wholeDistance) { wholeDistance = distTo; flip = false; fraction = 1 - fraction; }
                        fraction = splitFractionTrapezoid(trapezoidOnEdgeA.Length, wholeDistance, trapezoidOnEdgeA.UnitTangent, fraction);
                        if (!flip) fraction = 1 - fraction;
                    }

                    Point3d pointOfCutOnEdgeA = trapezoidOnEdgeA.PointAt(fraction);
                    Point3d pointOfCutOnEdgeB = trapezoidOnEdgeB.PointAt(fraction);
                    lineOfCut = new Line(pointOfCutOnEdgeA, pointOfCutOnEdgeB);

                }
                else if (areaOutside2 + getTotalArea() >= singlePartArea)
                {

                    double areaToCutAwayInTriangle = singlePartArea - (areaOutside2 + triangle2Area + trapezoidArea);
                    double fraction = areaToCutAwayInTriangle / triangle1Area;
                    if (triangle1 != null)
                    {
                        Point3d projected0 = triangle1[1];
                        Line edgeWithPointOfCut = (edgeA.ClosestPoint(projected0, true).DistanceTo(projected0) < 0.000001) ? new Line(projected0, edgeA.To) : new Line(projected0, edgeB.From);
                        Point3d pointOfCut = edgeWithPointOfCut.PointAt(fraction);
                        lineOfCut = (edgeA.ClosestPoint(pointOfCut, true).DistanceTo(pointOfCut) < 0.000001) ? new Line(pointOfCut, edgeB.From) : new Line(edgeA.To, pointOfCut);
                    }
                }
                Rhino.Geometry.Intersect.CurveIntersections ci = Rhino.Geometry.Intersect.Intersection.CurveLine(polyline.ToNurbsCurve(), lineOfCut, 0.001, 0.001);
                int intCount = 0;
                foreach (Rhino.Geometry.Intersect.IntersectionEvent ie in ci) if (ie.IsPoint) intCount++;
                if (lineOfCut != null && intCount <= 2)
                {
                    PolylineCurve polyCurve = polyline.ToPolylineCurve();
                    double t0, t1;
                    polyCurve.ClosestPoint(lineOfCut.To, out t0);
                    polyCurve.ClosestPoint(lineOfCut.From, out t1);

                    if (t0 != t1)
                    {

                        Curve cutAway = polyCurve.Trim(new Interval(t0, t1));
                        if (cutAway != null)
                        {
                            Polyline plCutAway = cutAway.ToPolyline(0.000001, 0.000001, 0.001, 1000000).ToPolyline();
                            plCutAway.Add(plCutAway[0]);
                            plCutAway.ReduceSegments(0.000001);
                            Curve cutRemain = polyCurve.Trim(new Interval(t1, t0));
                            Polyline plCutRemain = cutRemain.ToPolyline(0.000001, 0.000001, 0.001, 1000000).ToPolyline();
                            plCutRemain.Add(plCutRemain[0]);
                            plCutRemain.ReduceSegments(0.000001);
                            cuts.Add(new Cut(lineOfCut.Length, plCutAway, plCutRemain));
                        }
                    }
                }
            }


            return cuts;


        }

        public double splitFractionTrapezoid(double distance, double wholeDistance, Vector3d dir, double fraction)
        {
            dir.Unitize();
            double remainingDistance = wholeDistance - distance;
            Vector3d rotateVec = dir * remainingDistance;
            rotateVec.Rotate(Math.Acos(remainingDistance / wholeDistance), Vector3d.ZAxis);
            double projectedDistance = Math.Abs(Vector3d.Multiply(rotateVec, dir));
            double ankathete = wholeDistance - (fraction * (wholeDistance - projectedDistance));
            double gegenkathete = Math.Sqrt(Math.Pow(wholeDistance / 2.0, 2) - Math.Pow((wholeDistance / 2.0 - ankathete), 2));
            double hypothenuse = Math.Sqrt(Math.Pow(ankathete, 2) + Math.Pow((gegenkathete), 2));

            return (wholeDistance - hypothenuse) / distance;

        }
        private double CalculateTriangleArea(Point3d pt0, Point3d pt1, Point3d pt2)
        {
            return Math.Abs(Vector3d.CrossProduct(new Vector3d(pt0 - pt2), new Vector3d(pt1 - pt2)).Length / 2);

        }
    }

}
