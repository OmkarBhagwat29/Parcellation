
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
            Resizable = false;
            WindowStyle = WindowStyle.Default;
            this.Maximizable = false;

            this._vm = new MapViewModel( this);
        }
    }

}
