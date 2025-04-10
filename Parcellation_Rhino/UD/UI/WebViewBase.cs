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

        }
    }
}
