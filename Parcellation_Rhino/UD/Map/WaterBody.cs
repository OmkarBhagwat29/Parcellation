using UD.Helper;
using UD.GeoJson;


namespace UD.Map
{
    public class WaterBody : MapObjectBase
    {
        public WaterBodyType Type;

        public override void SetMapData(Feature feature)
        {
            base.SetMapData(feature);

            this.Color = System.Drawing.Color.SkyBlue;
        }
    }
}
