
using Rhino.Geometry;

namespace UD.Simulation.Constraints
{
    public class ParticleRepulsionConstraint : IConstraint
    {
        private readonly List<Particle> _particles;
       // private readonly double _minDistance;

        public ParticleRepulsionConstraint(List<Particle> particles)
        {
            _particles = particles;
            //_minDistance = minDistance;
        }

        public void Solve()
        {
            for (int i = 0; i < _particles.Count; i++)
            {
                var pi = _particles[i];
 
                for (int j = i + 1; j < _particles.Count; j++)
                {
                    var pj = _particles[j];

                    var dir = pi.Position - pj.Position;
                    double dist = dir.Length;

                    if (dist < Particle.Radius*2 && dist > 0.0001)
                    {
                        dir.Unitize();

                        // Repulsion force is proportional to the overlap distance
                        double overlap = Particle.Radius * 2 - dist;
                        Vector3d push = dir * (overlap * 0.5); // Repelling force

                        // Apply repulsion to both particles unless they're fixed
                        if (!pi.IsFixed)
                            pi.Position += push;

                        if (!pj.IsFixed)
                            pj.Position -= push;


                    }
                }
            }
        }
    }


}
