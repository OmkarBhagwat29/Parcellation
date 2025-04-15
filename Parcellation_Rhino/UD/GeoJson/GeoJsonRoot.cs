


using UD.Helper;

namespace UD.GeoJson
{
    public class GeoJsonRoot
    {
        [JsonPropertyName("buildings")]
        public List<BuildingModel> Buildings {get;set;}

        [JsonPropertyName("greenery")]
        public List<GreeneryModel> Greenery { get; set; }

        [JsonPropertyName("roads")]
        public List<RoadModel> Roads { get; set; }

        [JsonPropertyName("transportation")]
        public List<TransportationModel> Transportations { get; set; }

        [JsonPropertyName("waterBodies")]
        public List<WaterBodyModel> WaterBodies { get; set; }

        [JsonPropertyName("landuseAreas")]
        public List<LandUseModel> landUseModels { get; set; }
    }
}
