using UD.Helper;


namespace UD.GeoJson
{
    public class RoadModel : Feature
    {
        // ✅ Computed property to extract enum
        [JsonIgnore]
        public HighwayType HighwayType => TypeHelper.FindHighwayType(Properties);
    }
}
