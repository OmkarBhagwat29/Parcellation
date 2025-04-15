
using Parcellation.UD.Flooding;
using Rhino;
using Rhino.Display;
using Rhino.Geometry;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using UD.Simulation;


namespace UD.Flooding
{
    public class FloodingSystem : DisplayConduit
    {
        private CancellationTokenSource _cancellationSource;
        private CancellationToken _cancellationToken;
        private Task _simulationTask;

        public static bool ShowPoints = true;
        public static bool ShowPaths = true;

        public double Gravity = -9.8;
        public double TimeStep = 0.1;
        public double Friction = 0.9;

        public bool Run = true;
        public bool Reset = false;

       public FloodingSolver _solver { get; private set; } = new FloodingSolver();

        public Brep Terrain { get;private set; }

        public Mesh TerrainMesh { get;private set; }

        public FloodingSystem()
        {
            this.Enabled = true;
        }

        public FloodingSystem(Brep _terrain)
        {
            this.Enabled = true;
            this.Terrain = _terrain;
            this.InIt();
        }

        void InIt()
        {
            if (Terrain == null)
                return;

            TerrainMesh = Mesh.CreateFromBrep(Terrain, MeshingParameters.Default)[0];
            _solver.Terrain = TerrainMesh;
            //TerrainMesh.FaceNormals.ComputeFaceNormals();
            // TerrainMesh.Normals.ComputeNormals();
        }

        public void SetTerrain(Brep brp)
        {
            this.Terrain = brp;
            this.InIt();
        }

        public void Stop()
        {
            if (_cancellationSource is not null &&
                !_cancellationSource.IsCancellationRequested)
            {
                _cancellationSource.Cancel();

                try
                {
                    Reset = true;
                    _simulationTask?.Wait();

                    Reset = false;

                   // _cancellationSource?.Dispose();

                }
                catch (AggregateException)
                {

                   
                }


            }

        }

        public void SolveAsync()
        {

            _cancellationSource = new CancellationTokenSource();
            _cancellationToken = _cancellationSource.Token;



            _simulationTask = Task.Run(() =>
            {
                Stopwatch timer = Stopwatch.StartNew();
                double previousTime = 0;



                while (!_cancellationToken.IsCancellationRequested)
                {
                    if (!Run)
                        continue;

                    double currentTime = timer.Elapsed.TotalSeconds;
                    double deltaTime = currentTime - previousTime;
                    previousTime = currentTime;
                    deltaTime = Math.Min(deltaTime, 0.1);


                    _solver.Run(deltaTime);

                }

            },_cancellationToken);
        }

        protected override void PostDrawObjects(DrawEventArgs e)
        {
            if (_solver.Particles == null || _solver.Particles.Count == 0 || (!ShowPaths && !ShowPoints))
                return;

            var display = e.Display;

            var mat = new DisplayMaterial(Color.Blue, 0.5);

            foreach (var particle in _solver.Particles)
            {

                if(ShowPoints)
                    display.DrawSphere(new Sphere(particle.Position,Simulation.Particle.Radius),Color.Blue,1);

                if (ShowPaths)
                {

                    display.DrawPolyline(particle.Path, Color.Red);
                }


            }
        }

    }
}
