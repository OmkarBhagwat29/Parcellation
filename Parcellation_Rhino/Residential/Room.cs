using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UrbanDesign.Residential
{
    public class Room
    {
        public List<Window> Windows { get; set; } = new List<Window>();
        public List<Door> Doors { get; set; } = new List<Door>();

        public double Width { get; set; }
        public double Length{ get; set; }

        List<RoomCutOut> CutOuts = new List<RoomCutOut>();

        public List<Curve> CenterCurves { get; set; } = new List<Curve>();
        public List<double> WallThickness { get; set; } = new List<double>();

        public Plane RoomPlane { get; set; }

        public Curve CutOutBoundary { get; set; } = null;
        public List<Curve> CutOutCurves { get; set; } = new List<Curve>();

        public List<bool> ExteriorInteriorDecision { get; set; } = new List<bool>();

        public Curve RoomBoundary_inside { get; set; }

        public Curve RoomBoundary_Outside { get; set; }

        public Rectangle3d RoomBoundary_Center { get; set; }

        public string RoomName { get; set; }

        public Room(string roomName,
            Plane _pln,double _width,double _length,bool solveCutOuts = false)
        {
            this.RoomPlane = _pln;
            this.Width = _width;
            this.Length = _length;

            double w = this.Width / 2.0;
            double l = this.Length / 2.0;

            this.RoomBoundary_Center = new Rectangle3d(this.RoomPlane,
                new Interval(-w, w), new Interval(-l, l));

            this.CenterCurves = this.RoomBoundary_Center.ToNurbsCurve().DuplicateSegments().ToList();
            this.CenterCurves.ForEach(c => c.Domain = new Interval(0, 1));


            if (solveCutOuts)
            {
                this.SolveCutOut();

            }


        }

        public void Set_OutsideInsideBoundary(List<bool> isFullWall)
        {


            bool id0 = isFullWall[0]; //Bottom wall
            bool id1 = isFullWall[1]; //right wall
            bool id2 = isFullWall[2]; //top wall
            bool id3 = isFullWall[3]; //left wall


            if (id0 == false && id1 == true && id2 == true && id3 == true) // bottomWall
            {
                this.RoomBoundary_Outside = this.BottomSide_HalfWall(true).ToNurbsCurve();
                this.RoomBoundary_inside = this.BottomSide_HalfWall(false).ToNurbsCurve();
            }
            else if (id0 == true && id1 == false && id2 == true && id3 == true) // rightWall
            {
                this.RoomBoundary_Outside = this.RightSide_HalfWall(true).ToNurbsCurve();
                this.RoomBoundary_inside = this.RightSide_HalfWall(false).ToNurbsCurve();
            }
            else if (id0 == true && id1 == true && id2 == false && id3 == true) //top wall
            {
                this.RoomBoundary_Outside = this.TopSide_HalfWall(true).ToNurbsCurve();
                this.RoomBoundary_inside = this.TopSide_HalfWall(false).ToNurbsCurve();
            }
            else if (id0 == true && id1 == true && id2 == true && id3 == false) //left wall
            {
                this.RoomBoundary_Outside = this.LeftSide_HalfWall(true).ToNurbsCurve();
                this.RoomBoundary_inside = this.LeftSide_HalfWall(false).ToNurbsCurve();
            }
            else if (id0 == false && id1 == false && id2 == true && id3 == true) //bottom and right wall half
            {
                this.RoomBoundary_Outside = this.BottomRightSide_HalfWall(true).ToNurbsCurve();
                this.RoomBoundary_inside = this.BottomRightSide_HalfWall(false).ToNurbsCurve();
            }
            else if (id0 == false && id1 == true && id2 == true && id3 == false) //bottom and left wall half
            {
                this.RoomBoundary_Outside = this.BottomLeftSide_HalfWall(true).ToNurbsCurve();
                this.RoomBoundary_inside = this.BottomLeftSide_HalfWall(false).ToNurbsCurve();
            }
            else if (id0 == false && id1 == false && id2 == false && id3 == true) //bottom - left - Top  wall half
            {
                this.RoomBoundary_Outside = this.Bottom_Left_Top_Side_HalfWall(true).ToNurbsCurve();
                this.RoomBoundary_inside = this.Bottom_Left_Top_Side_HalfWall(false).ToNurbsCurve();
            }
            else if (id0 == false && id1 == false && id2 == false && id3 == false) //all half walls
            {
                this.RoomBoundary_Outside = this.AllSides_HalfWall(true).ToNurbsCurve();
                this.RoomBoundary_inside = this.AllSides_HalfWall(false).ToNurbsCurve();
            }
            else if (id0 == true && id1 == true && id2 == true && id3 == true) // all sides full walls
            {
                this.RoomBoundary_Outside = this.AllSides_FullWall(true).ToNurbsCurve();
                this.RoomBoundary_inside = this.AllSides_FullWall(false).ToNurbsCurve();
            }


        }


        #region Half and full Wall algorithm
        private Rectangle3d BottomSide_HalfWall(bool isOutsideWall)
        {

            double recWidthPos = 0;
            double recWidthNeg = 0;

            double recLegthPos = 0;
            double recLengthNeg = 0;

            if (isOutsideWall)
            {
                recWidthPos = (this.Width / 2.0) + (WallInfo.MaxThk / 2.0);
                recWidthNeg = -recWidthPos;

                recLegthPos = (this.Length / 2.0) + (WallInfo.MaxThk / 2.0);
                recLengthNeg = -((this.Length / 2) + (WallInfo.MinThk / 2.0));
            }
            else {
                recWidthPos = (this.Width / 2.0) - (WallInfo.MaxThk / 2.0);
                recWidthNeg = -recWidthPos;

                recLegthPos = (this.Length / 2.0) - (WallInfo.MaxThk / 2.0);
                recLengthNeg = -((this.Length / 2) - (WallInfo.MinThk / 2.0));
            }

            Interval xInterval = new Interval(recWidthNeg, recWidthPos);
            Interval yInterval = new Interval(recLengthNeg, recLegthPos);

            var rec = new Rectangle3d(this.RoomPlane, xInterval,yInterval);

            return rec;

        }

        private Rectangle3d BottomLeftSide_HalfWall(bool isOutsideWall)
        {

            double recWidthPos = 0;
            double recWidthNeg = 0;

            double recLegthPos = 0;
            double recLengthNeg = 0;

            if (isOutsideWall)
            {
                recWidthPos = (this.Width / 2.0) + (WallInfo.MaxThk / 2.0);
                recWidthNeg = -((this.Width / 2.0) + (WallInfo.MinThk / 2.0));

                recLegthPos = (this.Length / 2.0) + (WallInfo.MaxThk / 2.0);
                recLengthNeg = -((this.Length / 2) + (WallInfo.MinThk / 2.0));
            }
            else
            {
                recWidthPos = (this.Width / 2.0) - (WallInfo.MaxThk / 2.0);
                recWidthNeg = -((this.Width / 2.0) - (WallInfo.MinThk / 2.0));

                recLegthPos = (this.Length / 2.0) - (WallInfo.MaxThk / 2.0);
                recLengthNeg = -((this.Length / 2) - (WallInfo.MinThk / 2.0));
            }

            Interval xInterval = new Interval(recWidthNeg, recWidthPos);
            Interval yInterval = new Interval(recLengthNeg, recLegthPos);

            var rec = new Rectangle3d(this.RoomPlane, xInterval, yInterval);

            return rec;

        }

        private Rectangle3d BottomRightSide_HalfWall(bool isOutsideWall)
        {

            double recWidthPos = 0;
            double recWidthNeg = 0;

            double recLegthPos = 0;
            double recLengthNeg = 0;

            if (isOutsideWall)
            {
                recWidthPos = (this.Width / 2.0) + (WallInfo.MinThk / 2.0);
                recWidthNeg = -((this.Width / 2.0) + (WallInfo.MaxThk / 2.0));

                recLegthPos = (this.Length / 2.0) + (WallInfo.MaxThk / 2.0);
                recLengthNeg = -((this.Length / 2) + (WallInfo.MinThk / 2.0));
            }
            else
            {
                recWidthPos = (this.Width / 2.0) - (WallInfo.MinThk / 2.0);
                recWidthNeg = -((this.Width / 2.0) - (WallInfo.MaxThk / 2.0));

                recLegthPos = (this.Length / 2.0) - (WallInfo.MaxThk / 2.0);
                recLengthNeg = -((this.Length / 2) - (WallInfo.MinThk / 2.0));
            }

            Interval xInterval = new Interval(recWidthNeg, recWidthPos);
            Interval yInterval = new Interval(recLengthNeg, recLegthPos);

            var rec = new Rectangle3d(this.RoomPlane, xInterval, yInterval);

            return rec;

        }

        private Rectangle3d Bottom_Left_Top_Side_HalfWall(bool isOutsideWall)
        {

            double recWidthPos = 0;
            double recWidthNeg = 0;

            double recLegthPos = 0;
            double recLengthNeg = 0;

            if (isOutsideWall)
            {
                recWidthPos = (this.Width / 2.0) + (WallInfo.MinThk / 2.0);
                recWidthNeg = -((this.Width / 2.0) + (WallInfo.MaxThk / 2.0));

                recLegthPos = (this.Length / 2.0) + (WallInfo.MinThk / 2.0);
                recLengthNeg = -((this.Length / 2) + (WallInfo.MinThk / 2.0));
            }
            else
            {
                recWidthPos = (this.Width / 2.0) - (WallInfo.MinThk / 2.0);
                recWidthNeg = -((this.Width / 2.0) - (WallInfo.MaxThk / 2.0));

                recLegthPos = (this.Length / 2.0) - (WallInfo.MinThk / 2.0);
                recLengthNeg = -((this.Length / 2) - (WallInfo.MinThk / 2.0));
            }

            Interval xInterval = new Interval(recWidthNeg, recWidthPos);
            Interval yInterval = new Interval(recLengthNeg, recLegthPos);

            var rec = new Rectangle3d(this.RoomPlane, xInterval, yInterval);

            return rec;

        }

        private Rectangle3d RightSide_HalfWall(bool isOutsideWall)
        {

            double recWidthPos = 0;
            double recWidthNeg = 0;

            double recLegthPos = 0;
            double recLengthNeg = 0;

            if (isOutsideWall)
            {
                recWidthPos = (this.Width / 2.0) + (WallInfo.MinThk / 2.0);
                recWidthNeg = -((this.Width / 2.0) + (WallInfo.MaxThk / 2.0));

                recLegthPos = (this.Length / 2.0) + (WallInfo.MaxThk / 2.0);
                recLengthNeg = -(recLegthPos);
            }
            else
            {
                recWidthPos = (this.Width / 2.0) - (WallInfo.MinThk / 2.0);
                recWidthNeg = -((this.Width / 2.0) - (WallInfo.MaxThk / 2.0)); ;

                recLegthPos = (this.Length / 2.0) - (WallInfo.MaxThk / 2.0);
                recLengthNeg = -(recLegthPos);
            }


            var rec = new Rectangle3d(this.RoomPlane, new Interval(recWidthNeg, recWidthPos),
                new Interval(recLengthNeg, recLegthPos));

            return rec;

        }

        private Rectangle3d TopSide_HalfWall(bool isOutsideWall)
        {

            double recWidthPos = 0;
            double recWidthNeg = 0;

            double recLegthPos = 0;
            double recLengthNeg = 0;

            if (isOutsideWall)
            {
                recWidthPos = (this.Width / 2.0) + (WallInfo.MaxThk / 2.0);
                recWidthNeg = -(recWidthPos);

                recLegthPos = (this.Length / 2.0) + (WallInfo.MinThk / 2.0);
                recLengthNeg = -((this.Length / 2.0) + (WallInfo.MaxThk / 2.0));
            }
            else
            {
                recWidthPos = (this.Width / 2.0) - (WallInfo.MaxThk / 2.0);
                recWidthNeg = -(recWidthPos);

                recLegthPos = (this.Length / 2.0) - (WallInfo.MinThk / 2.0);
                recLengthNeg = -((this.Length / 2.0) - (WallInfo.MaxThk / 2.0));
            }


            var rec = new Rectangle3d(this.RoomPlane, new Interval(recWidthNeg, recWidthPos),
                new Interval(recLengthNeg, recLegthPos));

            return rec;

        }

        private Rectangle3d LeftSide_HalfWall(bool isOutsideWall)
        {

            double recWidthPos = 0;
            double recWidthNeg = 0;

            double recLegthPos = 0;
            double recLengthNeg = 0;

            if (isOutsideWall)
            {
                recWidthPos = (this.Width / 2.0) + (WallInfo.MaxThk / 2.0);
                recWidthNeg = -((this.Width / 2.0) + (WallInfo.MinThk / 2.0));

                recLegthPos = (this.Length / 2.0) + (WallInfo.MaxThk / 2.0);
                recLengthNeg = -(recLegthPos);
            }
            else
            {
                recWidthPos = (this.Width / 2.0) - (WallInfo.MaxThk / 2.0);
                recWidthNeg = -((this.Width / 2.0) - (WallInfo.MinThk / 2.0));

                recLegthPos = (this.Length / 2.0) - (WallInfo.MaxThk / 2.0);
                recLengthNeg = -(recLegthPos);
            }

            var rec = new Rectangle3d(this.RoomPlane, new Interval(recWidthNeg, recWidthPos),
                new Interval(recLengthNeg, recLegthPos));

            return rec;

        }

        private Rectangle3d AllSides_HalfWall(bool isOutsideWall)
        {

            double recWidthPos = 0;
            double recWidthNeg = 0;

            double recLegthPos = 0;
            double recLengthNeg = 0;

            if (isOutsideWall)
            {
                recWidthPos = (this.Width / 2.0) + (WallInfo.MinThk / 2.0);
                recWidthNeg = -(recWidthPos);

                recLegthPos = (this.Length / 2.0) + (WallInfo.MinThk / 2.0);
                recLengthNeg = -(recLegthPos);
            }
            else
            {
                recWidthPos = (this.Width / 2.0) - (WallInfo.MinThk / 2.0);
                recWidthNeg = -(recWidthPos);

                recLegthPos = (this.Length / 2.0) - (WallInfo.MinThk / 2.0);
                recLengthNeg = -(recLegthPos);
            }

            var rec = new Rectangle3d(this.RoomPlane, new Interval(recWidthNeg, recWidthPos),
                new Interval(recLengthNeg, recLegthPos));

            return rec;

        }

        private Rectangle3d AllSides_FullWall(bool isOutsideWall)
        {

            double recWidthPos = 0;
            double recWidthNeg = 0;

            double recLegthPos = 0;
            double recLengthNeg = 0;

            if (isOutsideWall)
            {
                recWidthPos = (this.Width / 2.0) + (WallInfo.MaxThk / 2.0);
                recWidthNeg = -(recWidthPos);

                recLegthPos = (this.Length / 2.0) + (WallInfo.MaxThk / 2.0);
                recLengthNeg = -(recLegthPos);
            }
            else
            {
                recWidthPos = (this.Width / 2.0) - (WallInfo.MaxThk / 2.0);
                recWidthNeg = -(recWidthPos);

                recLegthPos = (this.Length / 2.0) - (WallInfo.MaxThk / 2.0);
                recLengthNeg = -(recLegthPos);
            }

            var rec = new Rectangle3d(this.RoomPlane, new Interval(recWidthNeg, recWidthPos),
                new Interval(recLengthNeg, recLegthPos));

            //Rhino.RhinoDoc.ActiveDoc.Objects.AddRectangle(rec);
            //Rhino.RhinoDoc.ActiveDoc.Objects.AddRectangle(this.RoomBoundary_Center);

            return rec;

        }

        #endregion


        public void AddCutOut(RoomCutOut cutout)
        {
            this.CutOuts.Add(cutout);
        }


        public void SolveCutOut()
        {
            Curve centerCv = this.RoomBoundary_Center.ToNurbsCurve();

            if (this.CutOuts.Count == 0)
            {

                this.CutOutCurves = this.CenterCurves;

               // Rhino.RhinoDoc.ActiveDoc.Objects.AddCurve(this.CutOutCurves[3]);

                this.Set_WallExterioirInterior(RoomSystem.BoundingCurve);


                this.SetBoundaryWithInterSection(true);
                this.SetBoundaryWithInterSection(false);

                return;
            }

            for (int i = 0; i < this.CutOuts.Count; i++)
            {
                RoomCutOut cutOut = this.CutOuts[i];

                Curve[] cvs = Curve.CreateBooleanDifference(centerCv, cutOut.RoomCutBoundary, 0.001);

                if (cvs !=null)
                {
                    if (cvs.Length!=1)
                    {
                        continue;
                    }


                    centerCv = cvs[0];
                }

            }

            this.CutOutBoundary = centerCv;
            this.CutOutBoundary.Domain = new Interval(0, 1);

            this.CutOutCurves = this.CutOutBoundary.DuplicateSegments().ToList();
            this.CutOutCurves.ForEach(c => c.Domain = new Interval(0, 1));

            this.Set_WallExterioirInterior(RoomSystem.BoundingCurve);

            this.SetBoundaryWithInterSection(true);
            this.SetBoundaryWithInterSection(false);
            
        }


        private void Set_WallExterioirInterior(Curve boundingCv)
        {

            double wallMinThk = WallInfo.MinThk / 2.0;
            double wallMaxThk = WallInfo.MaxThk / 2.0;

            if (boundingCv == null)
            {
                //all walls are full wall


                for (int i = 0; i < this.CutOutCurves.Count; i++)
                {
                    this.WallThickness.Add(wallMaxThk);
                }

                return;
            }



            for (int i = 0; i < this.CutOutCurves.Count; i++)
            {
                Curve c = this.CutOutCurves[i];

                var wallInterior = this.IsWallInterior(boundingCv, c);

                this.ExteriorInteriorDecision.Add(wallInterior);

                if (wallInterior)
                {
                    this.WallThickness.Add(wallMinThk);
                }
                else
                {
                    this.WallThickness.Add(wallMaxThk);
                }

            }

        }

        private void SetBoundary()
        {
            //set first outside
            List<Curve> outsides = new List<Curve>();
            List<Curve> insides = new List<Curve>();

            int prvIndex = this.CutOutCurves.Count-1;
            int nextIndex = 1;

            for (int i = 0; i < this.CutOutCurves.Count; i++)
            {

                Curve c = this.CutOutCurves[i];

                double thk = this.WallThickness[i];

                double prevThk = this.WallThickness[prvIndex];
                double nextThk = this.WallThickness[nextIndex];

                Curve cO = c.Offset(this.RoomPlane.Origin, this.RoomPlane.Normal,
                    -thk, 0.001, CurveOffsetCornerStyle.Sharp)[0];

                Curve cI = c.Offset(this.RoomPlane.Origin, this.RoomPlane.Normal,
                        thk, 0.001, CurveOffsetCornerStyle.Sharp)[0];

                Line lnOut = new Line(cO.PointAtStart, cO.PointAtEnd);

                Line lnIn = new Line(cI.PointAtStart, cI.PointAtEnd);

               // thk *= 2;

                lnOut.Extend(prevThk, nextThk);

                lnIn.Extend(-prevThk, -nextThk);

               //Rhino.RhinoDoc.ActiveDoc.Objects.AddLine(lnOut);
                Rhino.RhinoDoc.ActiveDoc.Objects.AddLine(lnIn);

                outsides.Add(lnOut.ToNurbsCurve());
                insides.Add(lnIn.ToNurbsCurve());

                if (i == 0)
                {
                    prvIndex = 0;

                }
                else
                {
                    prvIndex++;
                }

                if (i == this.CutOutCurves.Count - 2)
                {
                    nextIndex = 0;
                }
                else
                {
                    nextIndex++;
                }
       
            }

            //join all curves
            Curve cJ = Curve.JoinCurves(outsides)[0];
            cJ.Domain = new Interval(0, 1);
            this.RoomBoundary_Outside = cJ.DuplicateCurve();

            cJ = Curve.JoinCurves(insides)[0];
            cJ.Domain = new Interval(0, 1);
            this.RoomBoundary_inside = cJ.DuplicateCurve();
        }

        private void SetBoundaryWithInterSection(bool isInside)
        {
            //set first outside
            List<Curve> offsetsCvs = new List<Curve>();

            int prvIndex = this.CutOutCurves.Count - 1;
            int nextIndex = 1;

            double thk = 1;

            if (!isInside)
            {
                thk = -thk;
            }

           double extendVal = WallInfo.MaxThk;

            for (int i = 0; i < this.CutOutCurves.Count; i++)
            {

                Curve c = this.CutOutCurves[i];

                Curve prevCv = this.CutOutCurves[prvIndex];
                Curve nextCv = this.CutOutCurves[nextIndex];

                double wallThk = this.WallThickness[i]*thk;
                double prevThk = this.WallThickness[prvIndex] * thk;
                double nextThk = this.WallThickness[nextIndex] * thk;


                Curve cO = c.Offset(this.RoomPlane.Origin, this.RoomPlane.Normal,
                            wallThk, 0.001, CurveOffsetCornerStyle.Sharp)[0];

                Curve cPrv = prevCv.Offset(this.RoomPlane.Origin, this.RoomPlane.Normal,
                            prevThk, 0.001, CurveOffsetCornerStyle.Sharp)[0];

                Curve cNext = nextCv.Offset(this.RoomPlane.Origin, this.RoomPlane.Normal,
                                    nextThk, 0.001, CurveOffsetCornerStyle.Sharp)[0];


                Line lnOut = new Line(cO.PointAtStart, cO.PointAtEnd);
                Line lnPrv = new Line(cPrv.PointAtStart, cPrv.PointAtEnd);
                Line lnNext = new Line(cNext.PointAtStart, cNext.PointAtEnd);


                lnOut.Extend(extendVal, extendVal);
                lnPrv.Extend(extendVal, extendVal);
                lnNext.Extend(extendVal, extendVal);


                var cx = Rhino.Geometry.Intersect.Intersection.LineLine(lnOut, lnPrv, out double a, out double b);
                Point3d startPt = lnOut.PointAt(a);
                
                cx = Rhino.Geometry.Intersect.Intersection.LineLine(lnOut, lnNext, out a, out b);
                Point3d endPt = lnOut.PointAt(a);

                Line fLn = new Line(startPt, endPt);


                offsetsCvs.Add(fLn.ToNurbsCurve());

                if (i == 0)
                {
                    prvIndex = 0;

                }
                else
                {
                    prvIndex++;
                }

                if (i == this.CutOutCurves.Count - 2)
                {
                    nextIndex = 0;
                }
                else
                {
                    nextIndex++;
                }

            }

            var cJs = Curve.JoinCurves(offsetsCvs);
            Curve cJ = cJs[0];
            cJ.Domain = new Interval(0, 1);


            if (!isInside)
            {
                this.RoomBoundary_Outside = cJ.DuplicateCurve();
            }
            else
            {
                this.RoomBoundary_inside = cJ.DuplicateCurve();
            }

        }

        private bool IsWallInterior(Curve boundingCv,Curve roomcv)
        {
            bool isInterior = true;

            double someOffsetVal = 1;

                var cOut = roomcv.Offset(Plane.WorldXY, someOffsetVal, 0.001, CurveOffsetCornerStyle.Sharp)[0];
                var cIn = roomcv.Offset(Plane.WorldXY, -someOffsetVal, 0.001, CurveOffsetCornerStyle.Sharp)[0];

            if (boundingCv.Contains(cOut.PointAtStart, Plane.WorldXY, 0.001)
            == PointContainment.Outside)
            {
                isInterior = false;
            }
            else if (boundingCv.Contains(cIn.PointAtStart, Plane.WorldXY, 0.001) ==
            PointContainment.Outside)
            {
                isInterior = false;
            }

            return isInterior;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="lefSidePlane">it is plane place outside building foot print for dimension purpose</param>
        /// <param name="startPt"></param>
        /// <param name="endPt"></param>
        /// <param name="lnPt"></param>
        public void GetLeftSideDimensionPoints(Plane lefSidePlane,
            out Point3d startPt,out Point3d endPt,out Point3d lnPt)
        {
            Transform xForm = Transform.ProjectAlong(lefSidePlane, -Vector3d.XAxis);

            startPt = this.RoomBoundary_Center.Corner(0);
            endPt = this.RoomBoundary_Center.Corner(3);

            lnPt = new Line(startPt, endPt).PointAt(0.5);

            startPt.Transform(xForm);
            endPt.Transform(xForm);
            lnPt.Transform(xForm);

            lnPt -= Vector3d.XAxis * 0.25;


        }
    }
}
