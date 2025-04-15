

using Rhino.Geometry;
using UD.Simulation;

namespace UD.Flooding
{
    public class FloodingSolver
    {
        public List<Simulation.Particle> Particles = new();
        public List<IConstraint> Constraints = new();
        public Mesh Terrain;

        public static double TimeStep = 1;
        //public double TimeStep = 0.05;
        //public int Iterations = 5;

        public void Run(double deltaTime)
        {

            foreach (var c in Constraints)
            {
                c.Solve();
            }

            foreach (var particle in Particles)
            {
                particle.Update(deltaTime * TimeStep);


                var result = Terrain.ClosestPoint(particle.Position, out Point3d closestPt, out Vector3d normal, 0.0);
                if (result != -1)
                {
                    normal.Unitize();
                    // Corrected position: particle sits on the surface + lifted by its radius
                    Point3d targetPos = closestPt + (normal * Simulation.Particle.Radius);

                    particle.Position = targetPos;


                    if (particle.Path.Count < 5000)
                    {
                        particle.Path.Add(particle.Position);


                    }


                }
            }

        }

    }
}
