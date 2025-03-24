using System;
using System.Linq;
using System.Collections.Generic;
using Rhino;
using Rhino.Commands;
using Rhino.DocObjects;
using Rhino.Geometry;
using Rhino.Input;
using Rhino.Input.Custom;

using System.Drawing;

using Rhino.Display;
using RhinoProjects.UrbanDesign.Logic;

using Grasshopper.GUI.Colours;
using Grasshopper.GUI.Gradient;

namespace RhinoProjects.UrbanDesign.RhinoCommands
{
    public class UrbanDesignCommand : Command
    {
        double tPara = 0;
        Curve curve = null;
        Mover mover;
        List<Brep> breps = new List<Brep>();

        GH_Gradient gradient = GH_Gradient.Traffic();
        RhinoViewport vPort;
        public UrbanDesignCommand()
        {
            // Rhino only creates one instance of each command class defined in a
            // plug-in, so it is safe to store a refence in a static property.

            Instance = this;

            // DisplayPipeline.PreDrawObject += DisplayPipeline_PreDrawObject;


        }

        private void DisplayPipeline_DrawForeground(object sender, DrawEventArgs e)
        {
            //if (simulate)
            //{

            //}

            if (tPara >= 1)
                tPara = 0;

            //curve.ClosestPoint(e.CurrentPoint, out tPara);
            //curve.PerpendicularFrameAt(tPara, out Plane plane);

            mover.Update(curve.PointAt(tPara));
            List<Line> lns = mover.FindIntersectionLines(breps, out List<double> dists);
            int i = 0;
            double min = dists.Min();
            double max = dists.Max();
            foreach (Line line in lns)
            {
                if (!line.IsValid)
                    continue;
                double colorD = Remap(dists[i], min, max, 1, 0);
                if (colorD == double.NaN)
                    continue;
                e.Display.DrawArrow(line, gradient.ColourAt(colorD));
                //e.Display.DrawArrow(line,Color.Red);
                i++;
            }


            e.Display.DrawSphere(new Sphere(curve.PointAt(tPara), 3000), Color.Black);

            tPara += 0.0001;

            Rhino.RhinoDoc.ActiveDoc.Views.ActiveView.Redraw();
        }


        public double Remap(double value, double from1, double to1, double from2, double to2)
        {
            return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
        }

        ///<summary>The only instance of this command.</summary>
        public static UrbanDesignCommand Instance
        {
            get; private set;
        }

        ///<returns>The command name as it appears on the Rhino command line.</returns>
        public override string EnglishName
        {
            get { return "Simulate_IsoVist"; }
        }

        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            this.breps.Clear();

            Rhino.Input.RhinoGet.GetOneObject("select street path", true, Rhino.DocObjects.ObjectType.Curve, out ObjRef oRef);
            if (oRef == null)
                return Result.Cancel;

            curve = oRef.Curve();
            curve.Domain = new Interval(0, 1);
            tPara = 0;

            ObjectEnumeratorSettings settings = new ObjectEnumeratorSettings();
            settings.ObjectTypeFilter = ObjectType.Brep | ObjectType.Extrusion;

            RhinoObject[] rhObjs = doc.Objects.FindByFilter(settings);
            if (rhObjs.Length == 0)
                return Result.Cancel;
            foreach (RhinoObject rhinoObject in rhObjs)
            {
                Brep bb = null;
                if (rhinoObject.Geometry is Brep b)
                {
                    if (b.IsSolid)
                        bb = b;
                }
                else if (rhinoObject.Geometry is Extrusion ext)
                {
                    if (ext.IsSolid)
                        bb = ext.ToBrep();
                }

                breps.Add(bb);
            }

            int count = 100;
            mover = new Mover(curve.PointAtStart, count);


            doc.Objects.UnselectAll();
            doc.Views.ActiveView.Redraw();


            DisplayPipeline.DrawForeground += DisplayPipeline_DrawForeground;

            bool stop = false;

            while (!stop)
            {
                RhinoGet.GetBool("Do you want to stop simulation?", false, "No", "Yes", ref stop);

            }

            DisplayPipeline.DrawForeground -= DisplayPipeline_DrawForeground;


            return Result.Success;

        }

    }

}
