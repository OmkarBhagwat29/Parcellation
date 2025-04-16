

namespace UD.GeoJson
{
    public class Geometry
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }

        // The coordinates can be either 2D (for LineString) or 3D (for Polygon),
        // so we use object and cast accordingly after deserialization
        [JsonPropertyName("coordinates")]
        public JsonElement Coordinates { get; set; }


        public List<List<List<double>>> GetMultiLineString()
        {
            var result = new List<List<List<double>>>();
            foreach (var line in Coordinates.EnumerateArray())
            {
                var lineCoords = new List<List<double>>();
                foreach (var point in line.EnumerateArray())
                {
                    var coords = new List<double>();
                    foreach (var coord in point.EnumerateArray())
                    {
                        coords.Add(coord.GetDouble());
                    }
                    lineCoords.Add(coords);
                }
                result.Add(lineCoords);
            }
            return result;
        }

        public List<List<double>> GetMultiPoint()
        {
            var result = new List<List<double>>();
            foreach (var point in Coordinates.EnumerateArray())
            {
                var coords = new List<double>();
                foreach (var coord in point.EnumerateArray())
                {
                    coords.Add(coord.GetDouble());
                }
                result.Add(coords);
            }
            return result;
        }

        public List<double> GetPoint()
        {
            var point = new List<double>();
            foreach (var coord in Coordinates.EnumerateArray())
            {
                point.Add(coord.GetDouble());
            }
            return point;
        }


        public List<List<double>> GetLineString()
        {
            var result = new List<List<double>>();

            foreach (var coord in Coordinates.EnumerateArray())
            {
                var point = new List<double>
            {
                coord[0].GetDouble(),
                coord[1].GetDouble()

            };
                result.Add(point);
            }

            return result;
        }

        public List<List<List<double>>> GetPolygon()
        {
            var result = new List<List<List<double>>>();

            foreach (var ring in Coordinates.EnumerateArray())
            {
                var ringCoords = new List<List<double>>();
                foreach (var point in ring.EnumerateArray())
                {
                    ringCoords.Add(new List<double>
                {
                    point[0].GetDouble(),
                    point[1].GetDouble()
                });
                }
                result.Add(ringCoords);
            }

            return result;
        }

        public List<List<List<List<double>>>> GetMultiPolygon()
        {
            var result = new List<List<List<List<double>>>>();
            foreach (var polygon in Coordinates.EnumerateArray())
            {
                var polygonRings = new List<List<List<double>>>();
                foreach (var ring in polygon.EnumerateArray())
                {
                    var ringCoords = new List<List<double>>();
                    foreach (var point in ring.EnumerateArray())
                    {
                        var coords = new List<double>();
                        foreach (var coord in point.EnumerateArray())
                        {
                            coords.Add(coord.GetDouble());
                        }
                        ringCoords.Add(coords);
                    }
                    polygonRings.Add(ringCoords);
                }
                result.Add(polygonRings);
            }
            return result;
        }

    }
}
