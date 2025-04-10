
using Eto.Forms;
using Eto.Wpf.Forms.Controls;
using Microsoft.Web.WebView2.Wpf;
using Parcellation.Helper;

using System.IO;
using UrbanDesign.Ui.ViewModels;


namespace UrbanDesign.Ui.Views
{
    public class ParcellationView : Form
    {
        WebView2 _webView { get; set; }

        StaticFileServer _fileServer;

        ParcellationViewModel _vm { get; set; }

        public ParcellationView()
        {
            string domain = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "out");
            Title = "Parcellation";
            Resizable = false;
            Size = new Eto.Drawing.Size(737, 900);

            _fileServer = new StaticFileServer(domain);
            _fileServer.Start();

            InitializeWebView2();

            this.Closed += (s, e) =>
            {
                _fileServer.Stop();
                _vm.Dispose();
            };
        }



        private void InitializeWebView2()
        {
            var wV = new WebView();
            wV.Url = new Uri("http://localhost:3000/parcellation.html");
            this.Content = wV;

            var webView = WebView2Handler.GetControl(wV);
            _vm = new ParcellationViewModel(webView);
        }

    }
}
