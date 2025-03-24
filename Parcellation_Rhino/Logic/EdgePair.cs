using Rhino.Geometry;
using Rhino.Geometry.Intersect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UrbanDesign.Logic
{
    /**
     * Represents a pair of edges on polygon's exterior ring.
     * Warning: direction of edges is assumed to be the same as in the polygon's exterior ring.
     *
     * Possible lines of cut are located in one of:
     *
     * T1 - First triangle, may not exist in some cases
     * Trapezoid - Trapezoid, always present
     * T2 - Second triangle, may not exist in some cases
     *
     *
     *                                edgeA
     *            edgeA.p0 .____________________________. edgeA.p1
     *                    /|                            |\
     *                   /                                \
     *   outsideEdge2   /  |                            |  \   outsideEdge1
     *                 /                                    \
     *                / T2 |        Trapezoid           | T1 \
     *               /                                        \
     *              .______.____________________________|______.
     *        edgeB.p1                edgeB                    edgeB.p0
     *                     ^                            ^
     *                 projected1                  projected0
     *
     */

    class EdgePair
    {

        private Line edgeA;
        private Line edgeB;

        private Point3d projected0;
        private Point3d projected1;

        public EdgePair(Line edgeA, Line edgeB)
        {

            double ia, ib;
            Point3d interesectionPoint = new Point3d(double.MaxValue, double.MaxValue, double.MaxValue);
            if (Rhino.Geometry.Intersect.Intersection.LineLine(edgeA, edgeB, out ia, out ib))
                interesectionPoint = edgeA.PointAt(ia);
            bool intersectionOnEdge = false;
            if (ia >= 0 && ia <= 1) intersectionOnEdge = true;
            if (ib >= 0 && ib <= 1) intersectionOnEdge = true;

            this.edgeA = edgeA;
            this.edgeB = edgeB;

            projected0 = getProjectedPoint(edgeA.To, edgeB, interesectionPoint, intersectionOnEdge);
            if (projected0.X == double.MaxValue)
            {
                projected0 = getProjectedPoint(edgeB.From, edgeA, interesectionPoint, intersectionOnEdge);
            }

            projected1 = getProjectedPoint(edgeB.To, edgeA, interesectionPoint, intersectionOnEdge);

            if (projected1.X == double.MaxValue)
            {
                projected1 = getProjectedPoint(edgeA.From, edgeB, interesectionPoint, intersectionOnEdge);
            }
        }
        public EdgePairSubpolylines getSubpolylines()
        {
            return new EdgePairSubpolylines(edgeA, edgeB, projected0, projected1);
        }


        private Point3d getProjectedPoint(Point3d point, Line opposingEdge, Point3d intPt, bool intersectionOnEdge)
        {
            if (intPt.X != double.MaxValue)
            {
                if (intersectionOnEdge)
                {
                    Vector3d vecPerpendicular = Vector3d.CrossProduct(opposingEdge.UnitTangent, Vector3d.ZAxis);
                    Line perpendicularLine = new Line(intPt, vecPerpendicular);
                    int orientationIndexOfPoint = OrientationOfPoint(perpendicularLine, point);
                    double p = opposingEdge.ClosestParameter(intPt);
                    if (p >= 0 && p <= 1)
                    {
                        int orientationIndexOfStart = OrientationOfPoint(perpendicularLine, opposingEdge.From);

                        if (orientationIndexOfPoint == orientationIndexOfStart)
                        {
                            // Start of opposingEdge is on the same side as the vertex (thus we shorten the segment discarding p1)
                            opposingEdge = new Line(opposingEdge.From, intPt);

                        }
                        else
                        {
                            // End of opposingEdge is on the same side as the vertex (thus we shorten the segment discarding p0)
                            opposingEdge = new Line(intPt, opposingEdge.To);
                        }
                    }
                    else
                    {
                        // the intersection point is outside of the edge
                        int orientationIndexOfEdge = OrientationOfPoint(perpendicularLine, opposingEdge.To);   // -1 or +1

                        // TODO: need to handle edge pairs better
                        //                    boolean vertexIsOnPerpendicularLine = orientationIndexOfVertex == 0;             // this is needed to handle cases when both edges are perpendicular to each other
                        bool vertexIsOnPerpendicularLine = false;
                        if (!vertexIsOnPerpendicularLine && orientationIndexOfPoint != orientationIndexOfEdge)
                        {
                            // projection of vertex is located somewhere on the opposite side of the intersection point (not on the edge)
                            return new Point3d(double.MaxValue, double.MaxValue, double.MaxValue);

                        }
                        // otherwise it is on the same side - proceed as usual
                    }
                }
                double distanceOfPoint = point.DistanceTo(intPt);


                double distOfOpEdgePoint1 = intPt.DistanceTo(opposingEdge.From);
                double distOfOpEdgePoint2 = intPt.DistanceTo(opposingEdge.To);
                if (distanceOfPoint >= Math.Max(distOfOpEdgePoint1, distOfOpEdgePoint2) || distanceOfPoint <= Math.Min(distOfOpEdgePoint1, distOfOpEdgePoint2))
                {
                    return new Point3d(double.MaxValue, double.MaxValue, double.MaxValue);
                }

                Point3d furtherPoint = getFurtherEnd(intPt, opposingEdge);
                Line extendedOpposingEdge = new Line(intPt, furtherPoint);
                return extendedOpposingEdge.PointAt(distanceOfPoint / extendedOpposingEdge.Length);
            }
            else
            {
                // In case of parallel lines, we do not have an intersection point
                double p = opposingEdge.ClosestParameter(point);       // a projection onto opposingEdge (extending to infinity)
                if (p >= 0 && p <= 1)
                {
                    return opposingEdge.PointAt(p);
                }
                else
                {
                    return new Point3d(double.MaxValue, double.MaxValue, double.MaxValue);
                }

            }
        }
        private int OrientationOfPoint(Line line, Point3d point)
        {
            Point3d testPt;
            Plane pl = new Plane(line.From, line.UnitTangent, Vector3d.ZAxis);
            pl.RemapToPlaneSpace(point, out testPt);
            if (testPt.Z < 0)
                return -1;
            else if (testPt.Z > 0) return 1;
            else return 0;

        }
        public Point3d getFurtherEnd(Point3d point, Line line)
        {
            return point.DistanceTo(line.From) > point.DistanceTo(line.To) ? line.From : line.To;
        }


    }
}
