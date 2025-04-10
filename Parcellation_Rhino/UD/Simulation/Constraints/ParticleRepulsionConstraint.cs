
using Rhino.Geometry;

namespace UD.Simulation.Constraints
{
    public class ParticleRepulsionConstraint : IConstraint
    {
        private readonly List<Particle> _particles;
        private readonly double _minDistance;

        public ParticleRepulsionConstraint(List<Particle> particles, double minDistance = 1.0)
        {
            _particles = particles;
            _minDistance = minDistance;
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

                    if (dist < _minDistance && dist > 0.0001)
                    {
                        dir.Unitize();
                        Vector3d push = dir * (_minDistance - dist);

                        if (!pi.IsFixed)
                            pi.Position += push;

                        if (!pj.IsFixed)
                            pj.Position -= push;

                        //var frictionMag = 0.001;
                        //var friction = pj.Velocity;
                        //friction *= -1;
                        //friction.Unitize();
                        //friction *= frictionMag;
                        //pj.Position += friction;

                
                    }
                }
            }
        }
    }


}
