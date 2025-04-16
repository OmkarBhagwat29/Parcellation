using Rhino;
using Rhino.Display;
using Rhino.Geometry;
using System.Drawing;
using UrbanDesign.Helper;



namespace UD.Parcellation
{
    public class ParcelSystem : ParcellationDisplay
    {
        static RhinoDoc _doc = RhinoDoc.ActiveDoc;
        Point3d? _attractor;

        public Point3d? Attractor
        {
            get { return _attractor; }
            set
            {
                _attractor = value;
                if (Parcel != null && Parcel.RoadNetwork != null)
                {

                }
            }
        }

        public bool CommercialDistributionBasedOnAttractor = true;
        public double CommercialPercentage = 0;
        public double GreenPercentage = 0;

        public List<Point3d> GreenZones = [];

        double _subParcelDepth = 60;

        public double SubParcelDepth
        {
            get { return _subParcelDepth; }
            set { _subParcelDepth = value; }
        }

        double _subParcelWidth = 30;

        public double SubParcelWidth
        {
            get { return _subParcelWidth; }
            set { _subParcelWidth = value; }
        }

        public double BuildingPlotDepth_Min = 15;
        public double BuildingPlotDepth_Max = 60;

        public double BuildingPlotWidth_Min = 15;
        public double BuildingPlotWidth_Max = 60;

        public double BuildingPlotDepth = 20;
        public double BuildingPlotWidth = 10;
        public double MinimumSubParcelArea = 400;


        double _majorRoadWidth = 18;
        double _minorRoadWidth = 9;
        private Parcel _parcel;

        public Parcel Parcel
        {
            get { return _parcel; }
            set
            {
                _parcel = value;
            }
        }


        public double MajorRoadWidth
        {
            get => _majorRoadWidth;
            set
            {
                _majorRoadWidth = value;

            }
        }

        public double MinorRoadWidth
        {
            get => _minorRoadWidth; set
            {
                _minorRoadWidth = value;
            }
        }



        public void Evaluate()
        {

            //sub first parcel

            CreateSubParcelsFromMajorRoads();

            ApplyGreenZone();

            CreateSubParcelsFromMinorRoads();

        }

        public void ApplyGreenZone()
        {
            Parcel.Children.ForEach(child =>
            {

                if (GreenZones.Any(pt => child.ParcelCurve.Contains(pt, Plane.WorldXY, 0.01) == PointContainment.Inside))
                {
                    child.Type = ParcelType.Green;
                    child.Children.Clear();
                }
                else
                {
                    child.Type = ParcelType.Residential;
                }

            });
        }

        public void CreateSubParcelsFromMajorRoads()
        {
            if (Parcel.RoadNetwork == null)
                return;
            Parcel.RoadNetwork.Width = MajorRoadWidth;
            Parcel.SetSplitParcelWithRoadNetwork();
            Parcel.SetChildrenByRoadWidth();


        }

        List<Parcel> _allSubParcels = [];

        /// <summary>
        /// 
        /// </summary>
        /// <param name="level">level 1 => Main Boundary
        /// Level 2 => Parcels splited by Major Roads
        /// Level 3 = Parcels splited by Minor Roads
        /// Level 4 = Building Parcels
        /// </param>
        /// <returns></returns>
        public List<Parcel> GetParcelsOnLevel(int level)
        {
            if (level == 1)
            {
                return new List<Parcel>() { Parcel };
            }
            else if (level == 2)
            {
                return Parcel.Children;
            }
            else if (level == 3)
            {
                return Parcel.Children.SelectMany(child => child.Children).ToList();
            }
            else if (level == 4)
            {
                return Parcel.Children.SelectMany(child => child.Children.SelectMany(ch => ch.Children)).ToList();
            }
            else
                return [];
        }


        public void CreateBuildingParcels()
        {

            if (Attractor is null)
            {
                double depth = BuildingPlotDepth;
                double width = BuildingPlotWidth;
                _allSubParcels.ForEach(parcel =>
                {
                    if (parcel.Type != ParcelType.Green)
                    {
                        parcel.SetRoadNetworkBasedOnParcelChildrenSize(depth, width, true);
                        parcel.SetSplitParcelWithRoadNetwork();
                        parcel.SetChildrenByRoadWidth();
                    }

                });
            }
            else
            {
                AttractorBasedDivision();
            }

            ParcellationHelper.SetCommercialTypeByPercentage(CommercialPercentage, CommercialDistributionBasedOnAttractor);
            ParcellationHelper.SetOllamaContext();

        }

        private void AttractorBasedDivision()
        {
            var max = _allSubParcels.Max(p =>
            {
                p.SqDistanceToAttractor = p.Props.Centroid.DistanceToSquared(Attractor.Value);

                return p.SqDistanceToAttractor;

            });

            var min = _allSubParcels.Min(p =>
            {
                p.SqDistanceToAttractor = p.Props.Centroid.DistanceToSquared(Attractor.Value);

                return p.SqDistanceToAttractor;

            });

            _allSubParcels.ForEach(parcel =>
            {

                if (parcel.Type != ParcelType.Green)
                {

                    BuildingPlotDepth = parcel.SqDistanceToAttractor.Value.Remap(min.Value, max.Value, BuildingPlotDepth_Min, BuildingPlotDepth_Max);
                    BuildingPlotWidth = parcel.SqDistanceToAttractor.Value.Remap(min.Value, max.Value, BuildingPlotWidth_Min, BuildingPlotWidth_Max);

                    parcel.SetRoadNetworkBasedOnParcelChildrenSize(BuildingPlotDepth, BuildingPlotWidth, true);
                    parcel.SetSplitParcelWithRoadNetwork();
                    parcel.SetChildrenByRoadWidth();
                }

            });
        }

        public void CreateSubParcelsFromMinorRoads()
        {

            if (Parcel == null)
                return;
            Parcel.Children.ForEach(child =>
            {

                child.SetRoadNetworkBasedOnParcelChildrenSize(SubParcelDepth, SubParcelWidth, false);

                child.RoadNetwork.Width = MinorRoadWidth;
                child.SetSplitParcelWithRoadNetwork();
                child.SetChildrenByRoadWidth();

            });

            SetAllSubParcelsByMinorRoads();
            CreateBuildingParcels();
        }

        public void SetParcelTypeBasedOnArea()
        {
            _allSubParcels.ForEach(p =>
            {
                if (p.Props.Area > MinimumSubParcelArea)
                {
                    p.Type = ParcelType.Residential;

                }
                else
                {
                    p.Type = ParcelType.Green;
                    p.Children.Clear();

                }
            });
        }

        public void SetAllSubParcelsByMinorRoads()
        {
            _allSubParcels = Parcel.Children.SelectMany(child =>
            {
                return child.GetDescendants();
            }).ToList();


            SetParcelTypeBasedOnArea();
        }


        #region JSON Schema

        public object ToBuildingsParcelJson()
        {
            var parcels = Parcel.Children
                .SelectMany(child => child.GetDescendants())
                .Select(d => d.ToBuildingParcelJson()) // Convert to object first
                .ToList();

            return new
            {
                features = new
                {
                    type = "parcels",
                    parcelData = parcels
                }
            };

        }

        #endregion


        #region Display
        protected override void PostDrawObjects(DrawEventArgs e)
        {
            //main parcel as road
            e.Display.DrawBrepShaded(Parcel.Geometry, new DisplayMaterial(Color.Gray));
            //Display Children Parcel
            DisplayChildren(e, Parcel, 1);


            if (Parcel.RoadNetwork is not null)
            {
                foreach (var c in Parcel.RoadNetwork.Roads)
                {
                    e.Display.DrawCurve(c.Curve, new DisplayPen() { Color = Color.BurlyWood, Thickness = 5 });
                }
            }


            GreenZones.ForEach(zPt => e.Display.DrawPoint(zPt, PointStyle.Circle, 10, Color.DarkSlateBlue));


            if (Attractor is not null)
            {
                e.Display.DrawPoint(Attractor.Value, PointStyle.Circle, 10, Color.Red);
            }

        }

        static void DisplayChildren(DrawEventArgs e, Parcel parcel, int depth)
        {


            for (int i = 0; i < parcel.Children.Count; i++)
            {
                if (parcel.Children[i].Type == ParcelType.Residential)
                {
                    if (depth == 3)
                    {
                        e.Display.DrawBrepShaded(parcel.Children[i].Geometry, new DisplayMaterial(Color.Yellow));
                        e.Display.DrawCurve(parcel.Children[i].ParcelCurve, Color.Black, 2);
                    }


                    DisplayChildren(e, parcel.Children[i], depth + 1);
                }
                else if (parcel.Children[i].Type == ParcelType.Green)
                {
                    //e.Display.DrawCurve(parcel.Children[i].ParcelCurve,
                    //new DisplayPen() { Color = Color.Green, Thickness = 3 });

                    e.Display.DrawBrepShaded(parcel.Children[i].Geometry, new DisplayMaterial(Color.LightGreen));
                    e.Display.DrawCurve(parcel.Children[i].ParcelCurve, Color.DarkGreen, 1);
                }
                else if (parcel.Children[i].Type == ParcelType.Commercial)
                {
                    e.Display.DrawBrepShaded(parcel.Children[i].Geometry, new DisplayMaterial(Color.LightBlue));
                    e.Display.DrawCurve(parcel.Children[i].ParcelCurve, Color.Black, 2);
                }
            }

        }

        #endregion
    }
}
