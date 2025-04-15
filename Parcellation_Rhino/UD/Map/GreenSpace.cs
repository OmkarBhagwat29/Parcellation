
using UD.GeoJson;
using UD.Helper;


namespace UD.Map
{
    public class GreenSpace : MapObjectBase
    {
        public GreenSpaceType Type;

        public override void SetMapData(Feature feature)
        {
            base.SetMapData(feature);

            this.Color = System.Drawing.Color.DarkGreen;
        }
    }
}
