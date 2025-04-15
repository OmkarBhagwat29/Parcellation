using Rhino.Geometry;



namespace UD.GeoJson
{
    public static class RhinoConvert
    {

        public static Point3d LonLatToRhinoXY(double lon, double lat)
        {
            const double R = 6378137; // Earth's radius in meters (WGS84)

            double x = R * Math.PI * lon / 180.0;
            double y = R * Math.Log(Math.Tan(Math.PI / 4 + Math.PI * lat / 360.0));

            return new Point3d(x, y, 0);
        }


        // Converts a LineString to a Rhino Polyline
        public static Polyline ToRhinoPolyline(this List<List<double>> coords)
        {
            var polyline = new Polyline();
            foreach (var coord in coords)
            {
                var point = LonLatToRhinoXY(coord[0], coord[1]);

                polyline.Add(point);
            }

            return polyline;
        }

        // Converts a Polygon (with optional inner rings) to Rhino Polylines
        public static List<Polyline> ToRhinoPolygon(this List<List<List<double>>> ringsout)
        {
            var polylines = new List<Polyline>();

            foreach (var ring in ringsout)
            {
                var polyline = new Polyline();
                foreach (var coord in ring)
                {
                    double x = coord[0];
                    double y = coord[1];

                    var point = LonLatToRhinoXY(coord[0], coord[1]);

                    polyline.Add(point);
                }

                polylines.Add(polyline);
            }
            return polylines;
        }

        public static List<Polyline> ToRhinoMultiPolygon(this List<List<List<List<double>>>> multipolygons)
        {
            var polylines = new List<Polyline>();

            foreach (var polygon in multipolygons)
            {
                var polygonPolylines = ToRhinoPolygon(polygon);

                polylines.AddRange(polygonPolylines);
            }

            return polylines;
        }



        public static Point3d GetGeoJsonCenter(List<List<List<double>>> rings)
        {
            double minX = double.MaxValue, minY = double.MaxValue;
            double maxX = double.MinValue, maxY = double.MinValue;

            foreach (var ring in rings)
            {
                foreach (var coord in ring)
                {
                    double lon = coord[0];
                    double lat = coord[1];

                    if (lon < minX) minX = lon;
                    if (lon > maxX) maxX = lon;
                    if (lat < minY) minY = lat;
                    if (lat > maxY) maxY = lat;
                }
            }

            double centerLon = (minX + maxX) / 2.0;
            double centerLat = (minY + maxY) / 2.0;

            return LonLatToRhinoXY(centerLon, centerLat);
        }
    }
}
