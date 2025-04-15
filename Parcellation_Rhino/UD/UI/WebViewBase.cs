using Eto.Forms;
using Eto.Wpf.Forms.Controls;
using Microsoft.Web.WebView2.Wpf;
using UrbanDesign.Ui.ViewModels;


namespace UD.UI
{
    public class WebViewBase : Form
    {
       public WebView2 _webView { get; private set; }
        public WebViewBase(Uri uri)
        {
            this.InitializeWebView2(uri);
        }

        private void InitializeWebView2(Uri uri)
        {

            WebView wV = new WebView();
            wV.Url = uri;
            this.Content = wV;

            _webView = WebView2Handler.GetControl(wV);

            _webView.NavigationCompleted += async (sender, e) =>
            {
                await this.Resize();
            };

        }

        public async Task Resize()
        {
            try
            {
                // JavaScript to get dimensions of #main div
                string jsCode = @"
                (function() {
                    const el = document.getElementById('main');
                    if (!el) return JSON.stringify({ width: 800, height: 600 }); // fallback
                    const rect = el.getBoundingClientRect();
                    return JSON.stringify({ width: rect.width, height: rect.height });
                })();";

                string result = await _webView.ExecuteScriptAsync(jsCode);

                // Result is JSON inside double quotes — remove them and deserialize
                var dimensionsJson = JsonDocument.Parse(result.Trim('"').Replace("\\", ""));
                var width = dimensionsJson.RootElement.GetProperty("width").GetDouble();
                var height = dimensionsJson.RootElement.GetProperty("height").GetDouble();

                // Update Eto Form size (account for window borders if needed)
                this.Resizable = true;
                this.ClientSize = new Eto.Drawing.Size((int)width, (int)height);
                this.Resizable = false;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error resizing window: " + ex.Message);
            }
        }
    }
}
