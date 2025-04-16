using Rhino;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UrbanDesign.Helper.Dimensions
{
    public static class Dimensions
    {

        public static LinearDimension GetLinearDimension( Point3d startPt, Point3d endPt,
            Point3d pointOnLine, double dimValue = 0)
        {
            Plane pln = Plane.WorldXY;

            //pln.Origin = startPt;
            Vector3d xDir = endPt - startPt;
            Vector3d yDir = new Vector3d(xDir);
            yDir.Rotate(RhinoMath.ToRadians(90), Vector3d.ZAxis);

            pln = new Plane(startPt, xDir, yDir);

            double u, v;
            pln.ClosestParameter(startPt, out u, out v);
            Point2d start = new Point2d(u, v);

            pln.ClosestParameter(endPt, out u, out v);
            Point2d end = new Point2d(u, v);

            pln.ClosestParameter(pointOnLine, out u, out v);
            Point2d lnPt = new Point2d(u, v);

            LinearDimension dim = new LinearDimension(pln, start, end, lnPt);
      
            if (dimValue != 0)
            {
                dim.PlainText = dimValue.ToString();
            }

            return dim;
        }

       
    }

}
