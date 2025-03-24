using Grasshopper.Kernel;
using Rhino;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using UrbanDesign.Helper.Dimensions;
using UrbanDesign.Logic;
using UrbanDesign.Residential;

namespace UrbanDesign.GH_Components
{
    public class ResidentialLayout_Component : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the ResidentialLayout class.
        /// </summary>
        public ResidentialLayout_Component()
          : base("ResidentialLayout", "residentialLayout",
              "Description",
              "UD", "Compute")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddNumberParameter("PlotWidth", "plotWidth", "", GH_ParamAccess.item, 8.0);
            pManager.AddNumberParameter("PlotLength", "plotLength", "", GH_ParamAccess.item, 16.0);

            pManager.AddNumberParameter("FSI", "fsi", "", GH_ParamAccess.item, 1.5);
            pManager.AddNumberParameter("SiteCoverage", "siteCoverage"," in percentage ", GH_ParamAccess.item, 90.0);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("geoms", "geoms", "", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            double plotWidth = 0;
            double plotLength = 0;
            double Fsi = 0;
            double siteCoverage = 0;

            if (!DA.GetData(0, ref plotWidth)) return;
            if (!DA.GetData(1, ref plotLength)) return;
            if (!DA.GetData(2, ref Fsi)) return;
            if (!DA.GetData(3, ref siteCoverage)) return;

            if (plotWidth > plotLength)
                return;

            if (plotWidth > 8.0 || plotWidth < 6)
                return;

            if (plotLength > 16 || plotLength < 12)
                return;


            RhinoDoc doc = RhinoDoc.ActiveDoc;


            //Room bedroom = new Room("Bedroom",Plane.WorldXY, plotWidth, plotLength);

            ////var fulls = new List<bool>() { false, false, false, false };

            ////bedroom.Set_OutsideInsideBoundary(fulls);


            //var tempData = new List<object>() { };

            ////data.Add(parcel.PlotCurve);

            ////data.Add(rec);
            //tempData.Add(bedroom.RoomBoundary_Center.ToNurbsCurve());
            //tempData.Add(bedroom.RoomBoundary_Outside.ToNurbsCurve());
            //tempData.Add(bedroom.RoomBoundary_inside.ToNurbsCurve());

            //DA.SetDataList(0, tempData);

            //return;




            LandParcel parcel = new LandParcel(plotWidth, plotLength, Fsi, siteCoverage);

            var rec = parcel.GetBuildingRectanlge(out double bWidth, out double bLength);

            //get length and width of footprint outline
            double width = rec.X.Length;
            double length = rec.Y.Length;

            Room houseBoundary = new Room("boundary", Plane.WorldXY, width, length);
            houseBoundary.Set_OutsideInsideBoundary(new List<bool>() { true, true, true, true });


            RoomSystem.BoundingCurve = houseBoundary.RoomBoundary_Center.ToNurbsCurve();

            //doc.Objects.AddCurve(RoomSystem.BoundingCurve);
            //33% of length is for bedrroms
            double bedRoomLen = length * 0.3333;
            double width_Bedroom = width / 2.0;

            //get right side of foot print
            Line rightSide = new Line(rec.Corner(1), rec.Corner(2));
            Line leftSide = new Line(rec.Corner(0), rec.Corner(3));

            //Bedroom divLine 

            Line bedroomDivLn = new Line(leftSide.PointAtLength(leftSide.Length - bedRoomLen),
                rightSide.PointAtLength(rightSide.Length - bedRoomLen));

            Line bedMidLine = new Line(bedroomDivLn.PointAt(0.5),
                new Line(rec.Corner(3), rec.Corner(2)).PointAt(0.5));

            //
            List<Point3d> bdPoints = new List<Point3d>();
            bdPoints.Add(bedMidLine.From);
            bdPoints.Add(bedMidLine.To);
            bdPoints.Add(rec.Corner(2));
            bdPoints.Add(bedroomDivLn.To);

            Point3d bed1_Centroid = AverageOfPoints(bdPoints);

            Plane bdPln = new Plane(rec.Plane);
            bdPln.Translate(bed1_Centroid - rec.Plane.Origin);

            Room bd1 = new Room("BedRoom_1", bdPln, width_Bedroom, bedRoomLen);
            // bd1.Set_OutsideInsideBoundary(new List<bool>() { false, true, true, false });

            //var tempData = new List<object>() { };

            ////data.Add(parcel.PlotCurve);

            ////data.Add(rec);
            //tempData.Add(bd1.RoomBoundary_Center.ToNurbsCurve());
            //tempData.Add(bd1.RoomBoundary_Outside.ToNurbsCurve());
            //tempData.Add(bd1.RoomBoundary_inside.ToNurbsCurve());

            //DA.SetDataList(0, tempData);

            //return;


            bdPoints = new List<Point3d>();
            bdPoints.Add(bedroomDivLn.From);
            bdPoints.Add(rec.Corner(3));
            bdPoints.Add(bedMidLine.To);
            bdPoints.Add(bedMidLine.From);

            Point3d bed2_Centroid = AverageOfPoints(bdPoints);
            bdPln.Translate(bed2_Centroid - bdPln.Origin);




            Room bd2 = new Room("BedRoom_2", bdPln, width_Bedroom, bedRoomLen);
            //bd2.Set_OutsideInsideBoundary(new List<bool>() { false, false, true, true });

            //Make passage
            //create passdivLine at 25%

            double passageLen = length * 0.25;

            double pLen = leftSide.Length - (bedRoomLen + passageLen);

            Line passageDivln = new Line(leftSide.PointAtLength(pLen),
                rightSide.PointAtLength(pLen));

            Line passageMidLn = new Line(passageDivln.PointAt(0.5), bedroomDivLn.PointAt(0.5));

            Point3d passageMidPt = passageMidLn.PointAtLength((passageLen/2.0)-WallInfo.MinThk/2.0);

            Plane passagePln = Plane.WorldXY;
            passagePln.Translate(passageMidPt - Point3d.Origin);

            double passageWidth = 1.2;
            //double passageLen = passageDivln.Length;
            Room passage = new Room("Passage", passagePln, passageWidth, passageLen+2.5);


            //Add Kitchen
            Line kitchenDivLn = passageMidLn;
            kitchenDivLn.Transform(Transform.Translation(-Vector3d.XAxis * (passageWidth / 2.0)));

            Point3d kp0 = leftSide.PointAtLength(leftSide.Length - (bedRoomLen + kitchenDivLn.Length));
            Line kitchenMidLn = new Line(kp0,
                kitchenDivLn.From);

            var kitchenPoints = new List<Point3d>() { };

            kitchenPoints.Add(kp0);
            kitchenPoints.Add(kitchenDivLn.From);
            kitchenPoints.Add(kitchenDivLn.To);
            kitchenPoints.Add(bedroomDivLn.From);

            Point3d ktMid = AverageOfPoints(kitchenPoints);

            Plane kitchenPln = Plane.WorldXY;
            kitchenPln.Translate(ktMid - Point3d.Origin);

            Room kitchen = new Room("Kitchen", kitchenPln, kitchenMidLn.Length, kitchenDivLn.Length);
            //kitchen.Set_OutsideInsideBoundary(new List<bool>() { false, false, false, true });

            //get left sideDimPlane
            //Plane leftDimPln = Plane.WorldXY;
            //leftDimPln.Translate(-Vector3d.XAxis * width / 2);
            //leftDimPln.Rotate(RhinoMath.ToRadians(90), leftDimPln.YAxis);

            //passage.GetLeftSideDimensionPoints(leftDimPln, out Point3d startPt, out Point3d endPt, out Point3d lnPt);


            var data = new List<object>() { };

            //data.Add(parcel.PlotCurve);


            RoomCutOut psg = new RoomCutOut(passage.RoomPlane,passage.RoomBoundary_Center.ToNurbsCurve());

            bd1.AddCutOut(psg);
            bd1.SolveCutOut();


            bd2.AddCutOut(psg);
            bd2.SolveCutOut();

            kitchen.SolveCutOut();

            data.Add(houseBoundary.RoomBoundary_Center.ToNurbsCurve());
            // data.Add(houseBoundary.RoomBoundary_inside.ToNurbsCurve());





            //data.Add(bd1.RoomBoundary_Center.ToNurbsCurve());
            //data.Add(bd1.CutOutBoundary);
            data.Add(bd1.RoomBoundary_Outside);
            data.Add(bd1.RoomBoundary_inside);

            //data.Add(bd2.RoomBoundary_Center.ToNurbsCurve());
            data.Add(bd2.RoomBoundary_Outside);
            data.Add(bd2.RoomBoundary_inside);

            data.Add(passage.RoomBoundary_Center.ToNurbsCurve());

           // data.Add(kitchen.RoomBoundary_Center.ToNurbsCurve());
            data.Add(kitchen.RoomBoundary_Outside);
            data.Add(kitchen.RoomBoundary_inside);

            DA.SetDataList(0, data);


            //Plane dimPln = new Plane(startPt, Vector3d.YAxis, -Vector3d.XAxis);

            //LinearDimension dim = Dimensions.GetLinearDimension( startPt, endPt, lnPt);

            //doc.Objects.AddLinearDimension(dim);






        }

        public static Point3d AverageOfPoints(List<Point3d> pts)
        {

            double x = 0;
            double y = 0;
            double z = 0;
            for (int i = 0; i < pts.Count; i++)
            {

                x += pts[i].X;
                y += pts[i].Y;
                z += pts[i].Z;
            }

            x /= pts.Count;
            y /= pts.Count;
            z /= pts.Count;

            return new Point3d(x, y, z);
        }


        public override void DrawViewportMeshes(IGH_PreviewArgs args)
        {
            //args.Display.
        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                //You can add image files to your project resources and access them like this:
                // return Resources.IconForThisComponent;
                return null;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("23155672-0c9f-4eb2-bfc4-b9ba7a51e312"); }
        }
    }
}