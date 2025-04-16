using Eto.Forms;


namespace UD.UI.Flooding
{
    public class FloodingUi:WebViewBase
    {
        FloodingViewModel _vm { get; set; }
        public FloodingUi(Uri uri) : base(uri)
        {
            Title = "Flooding Resilience";
            Resizable = false;
            WindowStyle = WindowStyle.Default;
            // Size = new Eto.Drawing.Size(350, 230);
            this.Maximizable = false;
            this._vm = new FloodingViewModel(this._webView,this);
        }
    }
}
