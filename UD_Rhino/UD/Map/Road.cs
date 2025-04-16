
using UD.GeoJson;
using UD.Helper;

namespace UD.Map
{
    public class Road : MapObjectBase
    {
        public HighwayType Type;

        public override void SetMapData(Feature feature)
        {
            base.SetMapData(feature);

            this.Color = this.Type.GetHighwayColor();
        }
    }
}
