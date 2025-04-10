using Eto.Forms;
using Parcellation.UD.UI.Flooding;


namespace UD.UI.Flooding
{
    public class FloodingUi:WebViewBase
    {
        FloodingViewModel _vm { get; set; }
        public FloodingUi(Uri uri) : base(uri)
        {
            Title = "Flooding Resilience";
            Resizable = true;
            WindowStyle = WindowStyle.Default;
            Size = new Eto.Drawing.Size(350, 230);

            this._vm = new FloodingViewModel(this._webView);
        }
    }
}
