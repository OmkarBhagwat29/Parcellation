using Rhino.Geometry;


namespace UD.Simulation.Constraints
{
    public class TerrainSlideConstraint : IConstraint
    {
        private Particle _particle;
        private Mesh _terrain;

        public TerrainSlideConstraint(Particle particle, Mesh terrain)
        {
            _particle = particle;
            _terrain = terrain;
        }

        public void Solve()
        {
            var result = _terrain.ClosestPoint(_particle.Position, out Point3d closestPt, out Vector3d normal, 0.0);
            if (result != -1)
            {
                normal.Unitize();
               // Corrected position: particle sits on the surface + lifted by its radius
               Point3d targetPos = closestPt + (normal * Particle.Radius);

                _particle.Position = targetPos;
       
            }
        }
    }
}
