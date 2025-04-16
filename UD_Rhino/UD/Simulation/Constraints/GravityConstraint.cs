using Rhino.Geometry;


namespace UD.Simulation.Constraints
{
    public class GravityConstraint : IConstraint
    {
        private Particle _particle;
        //private Vector3d _gravity;
        public static Vector3d Gravity = new Vector3d(0,0,-0.098);

        public GravityConstraint(Particle particle)
        {
            _particle = particle;
        }

        public void Solve()
        {

            _particle.ApplyForce(Gravity);

        }
    }
}
