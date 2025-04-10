using Rhino.Geometry;

namespace UD.Simulation
{
    public class Particle
    {
        public Point3d Position;
        public Point3d PreviousPosition;
        public Vector3d Velocity = Vector3d.Zero;
        public Vector3d Acceleration = Vector3d.Zero;
        public double Mass = 1.0;
        public bool IsFixed = false;
        public double Radius = 0.5;

        public const double MovementEpsilon = 0.0000001;
        public const int PositionLength = 50;
        public const double Friction = 0.01;

        // History to store past positions
        public List<Point3d> PositionHistory { get; set; } = new List<Point3d>();

        public Particle(Point3d start)
        {
            Position = start;
            PreviousPosition = start;
            Mass = 1.0;
        }

        public void Update()
        {
            // Velocity = (Position - PreviousPosition) / dt;
            this.PreviousPosition = this.Position;
            this.Velocity += this.Acceleration;


            this.Position += this.Velocity;

            if (PositionHistory.Count > PositionLength)
            {
                PositionHistory.RemoveAt(0);
            }
            else
            {
                PositionHistory.Add(this.Position);
            }

           this.Acceleration = Vector3d.Zero;
            this.Velocity = Vector3d.Zero;
        }

        public void ApplyForce(Vector3d force)
        {
            force /= this.Mass;
            Acceleration += force;
        }
    }
}
