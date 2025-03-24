using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using RhinoProjects.UrbanDesign.Logic;

namespace UrbanDesign.GH_Components
{
    public class IsoVist_Component : GH_Component
    {
        Mover mover;
        double tSpeed = 0;
        /// <summary>
        /// Initializes a new instance of the IsoVist_Component class.
        /// </summary>
        public IsoVist_Component()
          : base("IsoVist", "isovist",
              "",
              "UD", "Compute")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddBooleanParameter("reset", "reset", "", GH_ParamAccess.item);
            pManager.AddBooleanParameter("run", "run", "", GH_ParamAccess.item);
            pManager.AddCurveParameter("path", "path", "", GH_ParamAccess.item);
            pManager.AddBrepParameter("Geoms", "Geoms", "", GH_ParamAccess.list);
            pManager.AddNumberParameter("speed", "speed", "", GH_ParamAccess.item,0.001);
            pManager.AddIntegerParameter("rayCount", "rayCount", "", GH_ParamAccess.item, 50);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddLineParameter("lines", "lns", "", GH_ParamAccess.list);
            pManager.AddNumberParameter("distances", "dists", "", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            bool reset = false;
            bool run = false;
            Curve path = null;
            List<Brep> brps = new List<Brep>();
            double speed = 0;
            int rayCount = 0;

            if (!DA.GetData(0, ref reset)) return;
            if (!DA.GetData(1, ref run)) return;
            if (!DA.GetData(2, ref path)) return;
            if (!DA.GetDataList(3, brps)) return;
            DA.GetData(4, ref speed);
            DA.GetData(5, ref rayCount);

            if (reset)
            {
                path.Domain = new Interval(0, 1);
                mover = new Mover(path.PointAtStart, (int)360/rayCount);
                tSpeed = 0;

            }

            if (mover == null)
                return;

            if (run)
                this.ExpireSolution(true);

            mover.Count = (int)(360/rayCount);

            mover.Update(path.PointAt(tSpeed));

            List<Line> lns = mover.FindIntersectionLines(brps, out List<double> dists);

       

            tSpeed += speed;

            if (tSpeed >= 1)
                tSpeed = 0;

            DA.SetDataList(0, lns);
            DA.SetDataList(1, dists);
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
            get { return new Guid("61c1575b-3da8-4eb5-a91b-c61f56bf65ce"); }
        }
    }
}