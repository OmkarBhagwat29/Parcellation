
using Microsoft.Web.WebView2.Wpf;
using Parcellation.UD.Flooding;

using Rhino;
using Rhino.Geometry;
using UD.Flooding;
using UD.Simulation.Constraints;

namespace UD.UI.Flooding
{
    public class FloodingViewModel
    {
        public WebView2 WebView { get; set; }
        public FloodingUi Ui { get; set; }
        public FloodingViewModel(WebView2 webView,FloodingUi ui)
        {
            this.WebView = webView;
            this.Ui = ui;

            this.WebView.WebMessageReceived += this.WebView2_MessageReceived;

            FloodingHelper.InitializeSystem();

            this.Ui.Closing += Ui_Closing;

        }

        private void Ui_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.Dispose();
        }

        public void Dispose()
        {
            RhinoApp.Idle -= RhinoApp_Idle;

            FloodingHelper.Reset();
        }


        private static void RhinoApp_Idle(object sender, EventArgs e)
        {
            Rhino.RhinoDoc.ActiveDoc.Views.Redraw();
        }

        private void WebView2_MessageReceived(object sender,
            Microsoft.Web.WebView2.Core.CoreWebView2WebMessageReceivedEventArgs e)
        {

            var json = e.WebMessageAsJson;

            using JsonDocument doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            string id = root.GetProperty("id").GetString();
            string command = root.GetProperty("command").GetString();

            try
            {
                switch (command)
                {
                    case "SELECT_TERRAIN":

                        if (FloodingHelper.System.Terrain is not null)
                            return;

                        FloodingHelper.SetFloodingTerrain();
                        FloodingHelper.Evaluate();

                        RhinoApp.Idle += RhinoApp_Idle;

                        break;
                    case "SELECT_OBSTACLES":
                        //FloodingHelper.SetFloodingTerrain();
                        break;
                    case "RUN_SIMULATION":
                        {                        
                            var payload = root.GetProperty("payload").GetProperty("value");
                            var run = payload.GetBoolean();

                            FloodingHelper.System.Run = run;

                            if (run)
                            {
                                RhinoApp.Idle += RhinoApp_Idle;
                            }
                            else
                            {
                                RhinoApp.Idle -= RhinoApp_Idle;
                            }
                        }
                        break;
                    case "RESET_SIMULATION":
                        FloodingHelper.Reset();
                        RhinoApp.Idle -= RhinoApp_Idle;
                        break;
                    case "SET_GRAVITY":
                      { 
                            var payload = root.GetProperty("payload").GetProperty("value");

                            var gravity = payload.GetDouble();

                            GravityConstraint.Gravity = new Vector3d(0, 0, -gravity);
                      }

                        break;
                    case "SET_FRICTION":
                        {
                            var payload = root.GetProperty("payload").GetProperty("value");

                            var friction = payload.GetDouble();
                        }
                        break;
                    case "SET_MASS":
                        {
                            var payload = root.GetProperty("payload").GetProperty("value");

                            var particleMass = payload.GetDouble();
                            Simulation.Particle.Mass = particleMass;
                        }
                        break;
                    case "SET_TIME_STEP":
                        {
                            var payload = root.GetProperty("payload").GetProperty("value");

                            var step = payload.GetDouble();
                            FloodingSolver.TimeStep = step;
                        }
                        break;

                    case "SET_PARTICLE_RADIUS":
                        {
                            var payload = root.GetProperty("payload").GetProperty("value");
                            var particleRadius = payload.GetDouble();

                            Simulation.Particle.Radius = particleRadius;
                        }
                        break;

                    case "UI_RESIZED":
                        {
                            this.Ui.Resize();
                        }
                        break;
                    case "VISIBILITY_POINTS":
                        {
                            var payload = root.GetProperty("payload").GetProperty("value");
                            var showPoints = payload.GetBoolean();

                            FloodingSystem.ShowPoints = showPoints;
                        }
                        break;
                    case "VISIBILITY_PATHS":
                        {
                            var payload = root.GetProperty("payload").GetProperty("value");
                            var showPaths = payload.GetBoolean();

                            FloodingSystem.ShowPaths = showPaths;
                        }
                        break;
                }

                RhinoDoc.ActiveDoc.Views.Redraw();

            }
            catch (Exception ex)
            {

     
            }
        }
    }
}
