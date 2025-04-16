

namespace UD.Parcellation
{
    public class RoadNetwork(List<Road> roads)
    {
        public List<Road> Roads { get; } = roads;
        public double Width { get; set; }
    }
}
