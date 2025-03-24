using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UrbanDesign.Parcellation
{
    public class RoadNetwork(List<Road> roads)
    {
        public List<Road> Roads { get; } = roads;
        public double Width { get; set; }
    }
}
