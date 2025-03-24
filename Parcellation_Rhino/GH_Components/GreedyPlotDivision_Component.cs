using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using UrbanDesign.Logic;

using System.Linq; 


namespace UrbanDesign.GH_Components
{
    public class GreedyPlotDivision_Component : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the GreedyPlotDivision_Component class.
        /// </summary>
        public GreedyPlotDivision_Component()
          : base("GreedyPlotDivision", "plotDivision",
              "",
              "UD", "Compute")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter("boundary", "boundary", "", GH_ParamAccess.item);
            pManager.AddIntegerParameter("plotCount", "plotCount", "", GH_ParamAccess.item,2);

        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddCurveParameter("plots", "plots", "", GH_ParamAccess.list);
            pManager.AddNumberParameter("Area", "area", "", GH_ParamAccess.list);
            pManager.AddPointParameter("centers", "centers", "", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Curve plot = null;
            int plotCount = 1;

            if (!DA.GetData(0, ref plot))return;
            DA.GetData(1, ref plotCount);

            if (!plot.IsClosed || !plot.IsValid)
                return;

            AreaMassProperties amp = AreaMassProperties.Compute(plot);

            double singlePlotArea = amp.Area / (double)plotCount;

            List<Polyline> plotParts = new List<Polyline>();

            if (!plot.TryGetPolyline(out Polyline pl))
                return;

            Polyline remPoly = pl;
            for (int i = 0; i < plotCount-1; i++)
            {
                remPoly = Cut.split(remPoly, plotParts, singlePlotArea);
            }
            plotParts.Add(remPoly);

            DA.SetDataList(0, plotParts);
            DA.SetDataList(1, plotParts.Select(p => Math.Round(AreaMassProperties.Compute(p.ToNurbsCurve()).Area, 2)));
            DA.SetDataList(2, plotParts.Select(p => AreaMassProperties.Compute(p.ToNurbsCurve()).Centroid));
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
            get { return new Guid("bc9ec33d-752b-4451-be5d-ff53eea7b456"); }
        }
    }
}