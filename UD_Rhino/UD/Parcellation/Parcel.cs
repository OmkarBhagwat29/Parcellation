
using Rhino.Geometry;
using System.Text.Json;
using UrbanDesign.Helper.Geometry;


namespace UD.Parcellation
{
    public enum ParcelType
    {
        Green,
        Residential,
        Commercial
    }

    public class Parcel
    {
        static Random rand = new Random(23);

        public Parcel Parent { get; private set; }
        public List<Parcel> Children { get; private set; } = new();


        public AreaMassProperties Props { get; private set; }

        public Curve ParcelCurve { get; private set; }
        public ParcelType Type { get; set; }
        public double Depth { get; private set; }
        public double Width { get; private set; }
        public double? SqDistanceToAttractor { get; set; }

        public List<Line> BoundingEdges { get; private set; }
        public RoadNetwork RoadNetwork { get; set; }
        public List<Curve> SplittedParcelsWithRoads { get; private set; }

        public Brep Geometry;

        public List<Point3d> Coords = [];

        public Guid ParcelId = Guid.NewGuid();

        public string DisplayName = "";
        public string DisplayColor = $"rgb({rand.Next(0, 256)}, {rand.Next(0, 256)}, {rand.Next(0, 256)})";

        public Parcel(Curve parcelCurve, AreaMassProperties props, ParcelType type = ParcelType.Residential)
        {
            ParcelCurve = parcelCurve;
            Props = props;
            Type = type;
            BoundingEdges = parcelCurve.GetBoundingEdges();

            SetParcelCoords();
            SetGeometry();
        }




        public Parcel(Curve parcelCurve, ParcelType type = ParcelType.Residential)
            : this(parcelCurve, AreaMassProperties.Compute(parcelCurve), type)
        {

            SetParcelCoords();
            SetGeometry();
        }

        public void SetParcelCurve(Curve cv)
        {
            ParcelCurve = cv;
            Props = AreaMassProperties.Compute(ParcelCurve);
            SetParcelCoords();
            SetGeometry();
        }

        void SetGeometry()
        {

            var breps = Brep.CreatePlanarBreps(ParcelCurve, 0.001);

            if (breps is not null)
            {
                Geometry = breps[0];
            }
        }

        void SetParcelCoords()
        {
            Coords.Clear();
            var curves = ParcelCurve.DuplicateSegments();

            foreach (var item in curves)
            {
                Coords.Add(item.PointAtStart);
                Coords.Add(item.PointAtEnd);
            }
        }

        /// <summary>
        /// Splits the parcel with the road network and stores the resulting child parcels.
        /// </summary>
        public void SetSplitParcelWithRoadNetwork()
        {
            if (RoadNetwork == null) return;

            SplittedParcelsWithRoads = ParcelCurve
.SplitCurveWithCurves(RoadNetwork.Roads.Select(r => r.Curve).ToList())
.Where(c => c.IsClosed)
.ToList();

        }

        /// <summary>
        /// Creates child parcels by offsetting based on road width.
        /// </summary>
        public void SetChildrenByRoadWidth()
        {
            if (RoadNetwork == null || SplittedParcelsWithRoads == null) return;

            var children = new List<Parcel>(Children);
            Children.Clear();
            if (RoadNetwork.Width > 0)
            {
                AddChildrenByRoadWidth();
            }
            else
            {
                AddChildrenWithoutRoad();

            }

            if (children.Count == Children.Count)
            {
                //match the colors
                for (int i = 0; i < Children.Count; i++)
                {
                    Children[i].DisplayColor = children[i].DisplayColor;
                }
            }

        }

        void AddChildrenWithoutRoad()
        {
            foreach (var splitParcel in SplittedParcelsWithRoads)
            {
                var props = AreaMassProperties.Compute(splitParcel);

                if (ParcelCurve.Contains(props.Centroid, Plane.WorldXY, 0.01) == PointContainment.Inside)
                {
                    var childParcel = new Parcel(splitParcel, props) { Parent = this };
                    Children.Add(childParcel);


                }
            }
        }
        void AddChildrenByRoadWidth()
        {

            foreach (var splitParcel in SplittedParcelsWithRoads)
            {
                var offsetParcels = splitParcel.Offset(Plane.WorldXY, -RoadNetwork.Width / 2.0, 0.01, CurveOffsetCornerStyle.Sharp);

                if (offsetParcels?.Length == 1 && offsetParcels[0].IsClosed)
                {
                    var props = AreaMassProperties.Compute(offsetParcels[0]);

                    if (ParcelCurve.Contains(props.Centroid, Plane.WorldXY, 0.01) == PointContainment.Inside)
                    {

                        var childParcel = new Parcel(offsetParcels[0], props) { Parent = this };


                        Children.Add(childParcel);

                    }
                }
            }
        }

        /// <summary>
        /// Generates a road network by subdividing the parcel based on given dimensions.
        /// </summary>
        public void SetRoadNetworkBasedOnParcelChildrenSize(double requiredDepth, double requiredWidth, bool limitTo2)
        {
            var roadCurves = ParcelCurve.SubDivideClosedCurve(requiredDepth, requiredWidth, out double parcelWidth, out double parcelDepth, limitTo2: limitTo2);

            if (roadCurves == null) return;

            Depth = parcelDepth;
            Width = parcelWidth;
            RoadNetwork = new RoadNetwork(roadCurves.Select(c => new Road(c)).ToList());
        }

        /// <summary>
        /// Recursively retrieves all descendant parcels.
        /// </summary>
        public List<Parcel> GetDescendants()
        {
            var descendants = new List<Parcel>();
            foreach (var child in Children)
            {
                descendants.Add(child);
                descendants.AddRange(child.GetDescendants());
            }
            return descendants;
        }

        /// <summary>
        /// Adds a child parcel and ensures proper parent linkage.
        /// </summary>
        public void AddChild(Parcel child)
        {
            child.Parent = this;
            Children.Add(child);
        }

        public string ToCityBoundaryJsonString()
        {
            // Convert Point3d list to List<List<double[]>>
            var coords = new List<List<double[]>>
                     {
                         Coords.ConvertAll(p => new double[] { p.X, p.Y }) // Extract X, Y coordinates
                    };

            var cityBoundaryJson = new
            {

                cityBoundary = new
                {
                    type = "Polygon",
                    coordinates = coords
                }

            };

            string jsonString = JsonSerializer.Serialize(cityBoundaryJson);

            return jsonString;
        }



        public object ToBuildingParcelJson()
        {
            var coords = new List<List<double[]>>
                     {
                         Coords.ConvertAll(p => new double[] { p.X, p.Y }) // Extract X, Y coordinates
                    };

            var parcelJson = new
            {

                geometry = new
                {
                    type = "Polygon",
                    coordinates = coords,
                },
                properties = new
                {

                    parcelId = ParcelId,
                    type = Type,
                    area = Props.Area,
                    center = $"[{Props.Centroid.X},{Props.Centroid.Y}]",
                    parent_parcel_id = Parent.ParcelId,
                    squred_distance_to_attractor = SqDistanceToAttractor
                }

            };

            return parcelJson;
        }

        private static string GetConsistentColor(Guid id)
        {
            int hash = id.GetHashCode();

            // Ensure non-negative values
            int r = Math.Abs(hash >> 16 & 255);
            int g = Math.Abs(hash >> 8 & 255);
            int b = Math.Abs(hash & 255);

            return $"rgb({r},{g},{b})";
        }
    }
}
