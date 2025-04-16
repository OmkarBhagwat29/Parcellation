using System.Text.Json.Serialization;

namespace UrbanDesign.Models
{
    public class PieChartProps
    {
        [JsonPropertyName("labels")]
        public List<string> Labels { get; set; }

        [JsonPropertyName("datasets")]
        public List<Dataset> Datasets { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }
    }

    public class Dataset
    {
        [JsonPropertyName("label")]
        public string Label { get; set; }

        [JsonPropertyName("data")]
        public List<double> Data { get; set; }

        [JsonPropertyName("backgroundColor")]
        public List<string> BackgroundColor { get; set; }

        [JsonPropertyName("hoverOffset")]
        public int HoverOffset { get; set; }
    }

}
