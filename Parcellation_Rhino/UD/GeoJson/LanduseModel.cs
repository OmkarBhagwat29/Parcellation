

using UD.Helper;


namespace UD.GeoJson
{
    public class LandUseModel : Feature
    {
        // ✅ Computed property to extract enum
        [JsonIgnore]
        public LanduseType LandType => TypeHelper.GetLanduseTypeFromTags(Properties);
    }
}
