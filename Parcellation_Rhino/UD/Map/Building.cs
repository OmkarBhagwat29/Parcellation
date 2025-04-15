
using UD.GeoJson;
using UD.Helper;


namespace UD.Map
{
    public class Building : MapObjectBase
    {

        public BuildingType Type;

        public override void SetMapData(Feature feature)
        {
            base.SetMapData(feature);

            this.Color = this.Type.GetBuildingTypeColor();
        }

    }
}
