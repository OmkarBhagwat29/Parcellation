
using UD.Helper;

namespace UD.GeoJson
{
    public class GreeneryModel : Feature
    {
        [JsonIgnore]
        public GreenSpaceType GreenSpaceType => TypeHelper.FindGreenSpaceType(Properties);
    }
}
