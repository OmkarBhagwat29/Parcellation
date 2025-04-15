
using UD.GeoJson;
using UD.Helper;

namespace UD.Map
{
    public class Landuse : MapObjectBase
    {
        public LanduseType Type;
        public override void SetMapData(Feature feature)
        {
            base.SetMapData(feature);

            this.Color = this.Type.GetLanduseColor();
        }
    }
}
