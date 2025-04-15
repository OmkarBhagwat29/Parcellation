
using UD.Helper;

namespace UD.GeoJson
{
    public class WaterBodyModel : Feature
    {
        [JsonIgnore]
        public WaterBodyType WaterBodyType => TypeHelper.FindWaterBodyType(Properties);
    }
}
