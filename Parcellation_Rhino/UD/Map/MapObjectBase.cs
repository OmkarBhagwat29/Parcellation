
using Rhino.Geometry;
using System.Drawing;
using UD.GeoJson;


namespace UD.Map
{
    public abstract class MapObjectBase : IMapObject
    {
        public Dictionary<string, string> Properties { get; set; }
        public BoundingBox Bbx { get; set; } = new BoundingBox();
        public List<Curve> Outline { get; set; } = [];
        public Color Color { get; set; }
        public List<Brep> Shapes { get; set; } = [];

        public void Transform(Transform xForm)
        {
            for (int i = 0; i < Outline.Count; i++)
            {
                Outline[i].Transform(xForm);
            }

            Bbx.Transform(xForm);
        }

        public void CalculateBoundingBox()
        {
            var box = BoundingBox.Empty;
            foreach (var item in Outline)
            {
                var tempBx = item.GetBoundingBox(true);
                box.Union(tempBx);
                //this.Bbx = tempBx;
            }

            Bbx = box;
        }

        public virtual void SetMapData(Feature feature)
        {
            Properties = feature.Properties;

            if (feature.Geometry.Type == "Polygon")
            {
                var data = feature.Geometry.GetPolygon();
                var rhPolylines = data.ToRhinoPolygon()
                    .Select(p => p.ToNurbsCurve()).ToList();

                Outline.AddRange(rhPolylines);
            }
            else if (feature.Geometry.Type == "MultiPolygon")
            {
                var data = feature.Geometry.GetMultiPolygon();
                var rhPolygons = data.ToRhinoMultiPolygon()
                    .Select(p => p.ToNurbsCurve()).ToList();

                Outline.AddRange(rhPolygons);
            }
            else if (feature.Geometry.Type == "LineString")
            {
                var data = feature.Geometry.GetLineString();
                var rhPolylines = data.ToRhinoPolyline().ToNurbsCurve();

                Outline.Add(rhPolylines);
            }

            CalculateBoundingBox();
        }
    }
}
