﻿
using Eto.Forms;
using Eto.Wpf.Forms.Controls;
using Microsoft.Web.WebView2.Wpf;

using UrbanDesign.Ui.ViewModels;


namespace UrbanDesign.Ui.Views
{
    public class ParcellationView : Form
    {
        WebView2 _webView { get; set; }
         
        ParcellationViewModel _vm { get; set; }

        public ParcellationView()
        {
            Title = "Parcellation";
            Resizable = true;
            WindowStyle = WindowStyle.Default;
            Size = new Eto.Drawing.Size(737, 900);

            this.InitializeWebView2();

            this.Closed += ParcellationView_Closed;
            //this.Content = wV;
        }

        private void ParcellationView_Closed(object sender, EventArgs e)
        {
            this._vm.Dispose();
        }


        private void InitializeWebView2()
        {

            WebView wV = new WebView();
            wV.Url = new Uri("http://localhost:3000");
            this.Content = wV;

            var webView = WebView2Handler.GetControl(wV);

            this._vm = new ParcellationViewModel(webView);

        }



    }
}
