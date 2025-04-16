using Parcellation.Helper.Geometry;
using UD.Simulation.Constraints;
using Rhino.Geometry;

using UrbanDesign.Helper.Inputs;
using UD.Flooding;
using Particle = UD.Simulation.Particle;
using Rhino;

namespace Parcellation.UD.Flooding
{
    public static class FloodingHelper
    {
        public static FloodingSystem System;

        public static RhinoDoc Doc = RhinoDoc.ActiveDoc;




        public static void InitializeSystem()
        {
            System = new FloodingSystem();
        }



        #region input
        public static void SetFloodingTerrain()
        {

            var srf = RhinoSelect.SelectObject("Select Terrain", Rhino.DocObjects.ObjectType.Brep);

            if (srf == null)
                return;

            System.SetTerrain(srf.Brep());

          //  PrepareMockup(srf.Surface());

        }
        #endregion


        #region Functionality

        public static void Evaluate()
        {
            //Doc.Objects.AddMesh(System.TerrainMesh);
            if (System.Terrain is null)
                return;

            System.Enabled = true;
            var points = System.Terrain.Faces[0].IsoTrimSurface(20, 25)
                    .Select(s => AreaMassProperties.Compute(s))
                     .Where(a => a is not null)
                     .Select(a => a.Centroid)
                    .ToList();

            System._solver.Particles.Clear();
            System._solver.Constraints.Clear();

            foreach (var point in points)
            {
                var particle = new Particle(point);

                System._solver.Particles.Add(particle);
            }

            System._solver.Particles.ForEach(p => System._solver.Constraints.Add(new GravityConstraint(p)));
            // System._solver.Particles.ForEach(p => System._solver.Constraints.Add(new TerrainSlideConstraint(p, System.TerrainMesh)));

            System._solver.Constraints.Add(new ParticleRepulsionConstraint(System._solver.Particles));

            System.SolveAsync();
        }

        public static void Reset()
        {

            if (System != null & System.Terrain != null)
            {
                System.Stop();
                System.Enabled = false;

                var run = System.Run;
                System = null;

                System = new FloodingSystem();

                Doc.Views.Redraw();
            }

        }
        #endregion
    }
}
