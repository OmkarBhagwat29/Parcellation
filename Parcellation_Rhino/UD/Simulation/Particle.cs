using Rhino.Geometry;

namespace UD.Simulation
{
    public class Particle
    {
        public Point3d Position;
        public Point3d PreviousPosition;
        public Vector3d Velocity = Vector3d.Zero;
        public Vector3d Acceleration = Vector3d.Zero;
        public static double Mass = 1.0;
        public bool IsFixed = false;
        public static double Radius = 2;
        public static int PositionLength = 50;
        public static double Friction = 0.01;

        // History to store past positions
        //public List<Point3d> PositionHistory { get; set; } = new List<Point3d>();
        public Polyline Path = new Polyline();

        public Particle(Point3d start)
        {
            Position = start;
            PreviousPosition = start;
            Mass = 1.0;
        }


        public void Update(double deltaTime)
        {
            if (IsFixed) return;

            // Apply damping to velocity
            this.Velocity *= (1 - Friction);

            // Update velocity with acceleration (considering deltaTime)
            this.Velocity += this.Acceleration * deltaTime;

            // Update position
            this.Position += this.Velocity * deltaTime;

            // Reset acceleration only
            this.Acceleration = Vector3d.Zero;
        }


        public void ApplyForce(Vector3d force)
        {
           force /= Mass;
            Acceleration += force;
        }
    }
}
