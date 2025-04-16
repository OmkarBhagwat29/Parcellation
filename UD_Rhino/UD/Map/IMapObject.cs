using Rhino.Geometry;
using System.Drawing;
using UD.GeoJson;


namespace UD.Map
{
    public interface IMapObject
    {
        public void SetMapData(Feature feature);

        public void Transform(Transform xForm);

        public BoundingBox Bbx { get; set; }

        public List<Curve> Outline { get; set; }

        public List<Brep> Shapes { get; set; }

        public Color Color { get; set; }
    }
}
