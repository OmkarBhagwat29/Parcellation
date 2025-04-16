
using Microsoft.Web.WebView2.Wpf;
using Rhino;
using UD.UI.Map;
using UrbanDesign.Helper;

namespace Parcellation.UD.UI.Map
{
    public class MapViewModel
    {
        public MapUi Ui { get; set; }

        public MapViewModel( MapUi ui)
        {

            this.Ui = ui;
            MapHelper.Ui = this.Ui;
            this.Ui._webView.WebMessageReceived += this.WebView2_MessageReceived;
        }


        void WebView2_MessageReceived(object sender,
            Microsoft.Web.WebView2.Core.CoreWebView2WebMessageReceivedEventArgs e)
        {
            var json = e.WebMessageAsJson;
            using JsonDocument doc = JsonDocument.Parse(json);
            var root = doc.RootElement;
            string id = root.GetProperty("id").GetString();
            string command = root.GetProperty("command").GetString();
            // Handle the command here

            try
            {
                switch (command)
                {
                    case "MAP_CLEAR":
                        MapHelper.Clear();
                        this.Ui.Resize();
                        break;
                    case "MAP_GEOMETRY":
                        // Handle MAP_GEOMETRY command
                        {
                            this.Ui.Resize();
                           // RhinoApp.WriteLine("Extracting the map");

                            var payload = root.GetProperty("payload")
                                .GetProperty("value");

                            MapHelper.SetGeoJsonRoot(payload);
                            MapHelper.ProcessMap();

                        }
                        break;
                    case "RESIZE":
                        {
                            this.Ui.Resize();
                        }
                        break;
                    case "VISIBILITY_BUILDINGS":
                        {
                            var hide = root.GetProperty("payload")
                                            .GetProperty("value").GetBoolean();
                            MapHelper.Sys.ShowBuildings = hide;
                        }
                        break;
                    case "VISIBILITY_GREENERY":
                        {
                            var hide = root.GetProperty("payload")
                                            .GetProperty("value").GetBoolean();
                            MapHelper.Sys.ShowGreenSpaces = hide;
                        }
                        break;
                    case "VISIBILITY_ROADS":
                        {
                            var hide = root.GetProperty("payload")
                                            .GetProperty("value").GetBoolean();
                            MapHelper.Sys.ShowRoads = hide;
                        }
                        break;
                    case "VISIBILITY_WATER_BODIES":
                        {
                            var hide = root.GetProperty("payload")
                                            .GetProperty("value").GetBoolean();
                            MapHelper.Sys.ShowWaterBodies = hide;
                        }
                        break;
                    case "VISIBILITY_TRANSPORTATION":
                        {
                            var hide = root.GetProperty("payload")
                                            .GetProperty("value").GetBoolean();
                            MapHelper.Sys.ShowTransportation = hide;
                        }
                        break;
                    case "VISIBILITY_LANDUSE":
                        {
                            var hide = root.GetProperty("payload")
                                            .GetProperty("value").GetBoolean();
                            MapHelper.Sys.ShowLandUse = hide;
                        }
                        break;
                    case "VISIBILITY_SHAPES":
                        {
                            var hide = root.GetProperty("payload")
                                            .GetProperty("value").GetBoolean();
                            MapHelper.Sys.ShowShapes = hide;
                        }
                        break;
                    default:
                        // Handle unknown command
                        break;

                }

                RhinoDoc.ActiveDoc.Views.Redraw();
            }
            catch
            {


            }
        }

    }
}
