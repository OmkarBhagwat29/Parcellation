
using UD.GeoJson;
using UD.Helper;


namespace UD.Map
{
    public class Transportation : MapObjectBase
    {
        public TransportationType Type;

        public override void SetMapData(Feature feature)
        {
            base.SetMapData(feature);

            this.Color = System.Drawing.Color.LightGray;
        }
    }

}
