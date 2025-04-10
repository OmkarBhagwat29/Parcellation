using Microsoft.Web.WebView2.Wpf;
using Parcellation.UD.Flooding;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UD.Simulation.Constraints;

namespace UD.UI.Flooding
{
    public class FloodingViewModel
    {
        public WebView2 WebView { get; set; }
        public FloodingViewModel(WebView2 webView)
        {
            this.WebView = webView;

            this.WebView.WebMessageReceived += this.WebView2_MessageReceived;

            FloodingHelper.InitializeSystem();
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
                        
                        FloodingHelper.SetFloodingTerrain();
                        FloodingHelper.Evaluate();
                        break;
                    case "SELECT_OBSTACLES":
                        //FloodingHelper.SetFloodingTerrain();
                        break;
                    case "RUN_SIMULATION":
                        {                        
                            var payload = root.GetProperty("payload").GetProperty("value");
                            var run = payload.GetBoolean();

                            FloodingHelper.System.Run = run;
                        }
                        break;
                    case "RESET_SIMULATION":
                        FloodingHelper.Reset();

                        break;
                    case "SET_GRAVITY":
                      { 
                            var payload = root.GetProperty("payload").GetProperty("value");

                            var gravity = payload.GetDouble();

                            GravityConstraint.Gravity = new Vector3d(0, 0, -gravity);
                      }

                        break;
                }

            }
            catch (Exception ex)
            {

     
            }
        }
    }
}
