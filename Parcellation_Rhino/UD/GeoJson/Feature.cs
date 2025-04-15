


namespace UD.GeoJson
{
    public class Feature
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("properties")]
        public Dictionary<string, string> Properties { get; set; }

        [JsonPropertyName("geometry")]
        public Geometry Geometry { get; set; }


    }
}
