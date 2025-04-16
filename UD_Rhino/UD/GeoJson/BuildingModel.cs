

using UD.Helper;

namespace UD.GeoJson
{
    public class BuildingModel : Feature
    {
        // ✅ Computed property to extract enum
        [JsonIgnore]
        public BuildingType BuildingType => TypeHelper.FindBuildingType(Properties);
    }
}
