
using Eto.Forms;
using Parcellation.UD.UI.Map;

namespace UD.UI.Map
{
    public class MapUi : WebViewBase
    {
        MapViewModel _vm { get; set; }
        public MapUi(Uri uri) : base(uri)
        {
            Title = "MAP";
            Resizable = true;
            WindowStyle = WindowStyle.Default;
            this.Maximizable = false;
            //this.ClientSize = new Eto.Drawing.Size(416, 507);
            this._vm = new MapViewModel( this);
            this.SizeChanged += MapUi_SizeChanged;
           
        }

        private void MapUi_SizeChanged(object sender, EventArgs e)
        {
            if (this.ResizedCalled)
                return;

            var newSize = this.ClientSize; // or just `this.Size` for outer bounds

            var data = new
            {
                eventType = "app_resized",
                payload = new {
                    width = newSize.Width,
                    height = (newSize.Height)
                }
            };

            //* 0.8587745287474773
            // Send to WebView
            _webView?.CoreWebView2?.PostWebMessageAsJson(JsonSerializer.Serialize(data));

          // this.SetMainDivSizeAsync(newSize.Width, newSize.Height);
        }


        public async Task SetMainDivSizeAsync(int width, int height)
        {
            try
            {
                var result = await this.GetMainSize();

            }
            catch (Exception ex)
            {
                Console.WriteLine("Error setting main div size: " + ex.Message);
            }
        }
    }

}
