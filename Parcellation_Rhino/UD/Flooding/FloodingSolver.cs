

using Rhino.Geometry;
using UD.Simulation;

namespace UD.Flooding
{
    public class FloodingSolver
    {
        public List<Simulation.Particle> Particles = new();
        public List<IConstraint> Constraints = new();
        public Mesh Terrain;
        //public double TimeStep = 0.05;
        //public int Iterations = 5;

        public bool Run(Func<bool> isPaused)
        {
            if (isPaused()) return true;

            foreach (var c in Constraints)
            {
                c.Solve();
            }

            foreach (var particle in Particles)
            {
                particle.Update();


                var result = Terrain.ClosestPoint(particle.Position, out Point3d closestPt, out Vector3d normal, 0.0);
                if (result != -1)
                {
                    normal.Unitize();
                    // Corrected position: particle sits on the surface + lifted by its radius
                    Point3d targetPos = closestPt + (normal * particle.Radius);

                    particle.Position = targetPos;

                }
            }




            return false;
        }

    }
}
