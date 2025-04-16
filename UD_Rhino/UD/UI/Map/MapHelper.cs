
using Rhino.Geometry;
using UD.Map;
using UD.GeoJson;
using Rhino;


namespace UD.UI.Map
{
    public static class MapHelper
    {

      public static MapSystem Sys = new MapSystem();
      public static GeoJsonRoot Root = new GeoJsonRoot();
        public static MapUi Ui;

        #region Inputs

        public static void  SetGeoJsonRoot(JsonElement payload)
        {
            Root = JsonSerializer.Deserialize<GeoJsonRoot>(payload);
        }

        #endregion


        #region Functionality

        public static void Clear()
        {
            Sys.MapObjects.Clear();
        }

        public static void TransformSystemToOrigin()
        {
            var center = Sys.Bbx.Center;
            var xForm = Transform.Translation(-center.X, -center.Y, 0);
            Sys.TransformSystem(xForm);
        }

        public static void ProcessMap()
        {

            Task.Run(() =>
            {
                Sys.IsExecuting = true;
                Sys.SetMapObjects(Root);
                TransformSystemToOrigin();

                Sys.ComputeShapes();

                

                Sys.IsExecuting = false;
                RhinoDoc.ActiveDoc.Views.ActiveView.ActiveViewport.ZoomBoundingBox(Sys.Bbx);
                RhinoDoc.ActiveDoc.Views.Redraw();
                RhinoApp.WriteLine("Extraction Completed!!!");

                var sendData = new { eventType = "extraction_completed", message = "hello world" };
                RhinoApp.InvokeOnUiThread(()=>  Ui._webView.CoreWebView2.PostWebMessageAsString(JsonSerializer.Serialize(sendData)));
            });
        }


        #endregion


    }
}
