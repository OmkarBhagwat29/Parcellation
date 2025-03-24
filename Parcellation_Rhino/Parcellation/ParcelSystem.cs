using Rhino;
using Rhino.Display;
using Rhino.Geometry;
using System.Drawing;
using System.Text.Json;
using UrbanDesign.Helper;



namespace UrbanDesign.Parcellation
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
                if (this.Parcel != null && this.Parcel.RoadNetwork != null)
                {

                }
            }
        }
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
            this.Parcel.Children.Clear();
            this.CreateSubParcelsFromMajorRoads();
            this.CreateSubParcelsFromMinorRoads();

        }



        public void CreateSubParcelsFromMajorRoads()
        {
            if (this.Parcel.RoadNetwork == null)
                return;
            this.Parcel.RoadNetwork.Width = this.MajorRoadWidth;
            this.Parcel.SetSplitParcelWithRoadNetwork();
            this.Parcel.SetChildrenByRoadWidth();

        }
        List<Parcel> _allSubParcels = [];




        public void CreateBuildingParcels()
        {
            


            if (this.Attractor is null)
            {
                double depth = this.BuildingPlotDepth;
                double width = this.BuildingPlotWidth;
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
                this.AttractorBasedDivision();
            }

        }

        private void AttractorBasedDivision()
        {
            var max = _allSubParcels.Max(p =>
            {
                p.SqDistanceToAttractor = p.Props.Centroid.DistanceToSquared(this.Attractor.Value);

                return p.SqDistanceToAttractor;

            });

            var min = _allSubParcels.Min(p =>
            {
                p.SqDistanceToAttractor = p.Props.Centroid.DistanceToSquared(this.Attractor.Value);

                return p.SqDistanceToAttractor;

            });

            _allSubParcels.ForEach(parcel =>
            {

                if (parcel.Type != ParcelType.Green)
                {

                   this.BuildingPlotDepth = parcel.SqDistanceToAttractor.Value.Remap(min.Value, max.Value, this.BuildingPlotDepth_Min, this.BuildingPlotDepth_Max);
                    this.BuildingPlotWidth = parcel.SqDistanceToAttractor.Value.Remap(min.Value,max.Value,this.BuildingPlotWidth_Min,this.BuildingPlotWidth_Max);

                    parcel.SetRoadNetworkBasedOnParcelChildrenSize(this.BuildingPlotDepth, this.BuildingPlotWidth, true);
                    parcel.SetSplitParcelWithRoadNetwork();
                    parcel.SetChildrenByRoadWidth();
                }

            });
        }

        public void CreateSubParcelsFromMinorRoads()
        {

            if (this.Parcel == null)
                return;
            this.Parcel.Children.ForEach(child => {

                child.SetRoadNetworkBasedOnParcelChildrenSize(this.SubParcelDepth, this.SubParcelWidth, false);

                child.RoadNetwork.Width = this.MinorRoadWidth;
                child.SetSplitParcelWithRoadNetwork();
                child.SetChildrenByRoadWidth();

            });

            this.SetAllSubParcelsByMinorRoads();
            this.CreateBuildingParcels();
        }

        public void SetParcelTypeBasedOnArea()
        {
            _allSubParcels.ForEach(p => {
                if (p.Props.Area > this.MinimumSubParcelArea)
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
            _allSubParcels = this.Parcel.Children.SelectMany(child =>
            {
                return child.GetDescendants();
            }).ToList();


            this.SetParcelTypeBasedOnArea();  
        }


        #region JSON Schema

        public object ToBuildingsParcelJson()
        {
            var parcels = this.Parcel.Children
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
        protected override void DrawForeground(DrawEventArgs e)
        {
            {

                //Display Children Parcel
                DisplayChildren(e, this.Parcel,1);



                //this._greenPoints.ForEach(zPt => e.Display.DrawBrepShaded(new Sphere(zPt, 15).ToBrep(),
                //        new DisplayMaterial(Color.DarkGreen)));
                if (this.Parcel.RoadNetwork is not null)
                {
                    foreach (var c in this.Parcel.RoadNetwork.Roads)
                    {
                        e.Display.DrawCurve(c.Curve, new DisplayPen() { Color = Color.BurlyWood, Thickness = 5 });
                    }
                }

                if (this.Attractor is not null)
                {
                    e.Display.DrawPoint(this.Attractor.Value,PointStyle.Circle,10,Color.Red);
                }


            }

        }

        static void DisplayChildren(DrawEventArgs e, Parcel parcel,int depth)
        {
            for (int i = 0; i < parcel.Children.Count; i++)
            {
                if (parcel.Children[i].Type == ParcelType.Residential)
                {
                    if (depth == 1)
                    {
                        e.Display.DrawBrepShaded(parcel.Children[i].Geometry,new DisplayMaterial(Color.Black));
                    }
                    else if (depth == 2)
                    {
                    //    e.Display.DrawCurve(parcel.Children[i].ParcelCurve,
                    //new DisplayPen() { Color = Color.Black, Thickness = 2 });

                    //    e.Display.Draw3dText(new Text3d($"P_{i}", Plane.WorldXY, 5), Color.Red, parcel.Children[i].Props.Centroid);
                    }
                    else if (depth == 3)
                    {
                        e.Display.DrawCurve(parcel.Children[i].ParcelCurve,
                        new DisplayPen() { Color = Color.Black, Thickness = 1 });

                        e.Display.DrawBrepShaded(parcel.Children[i].Geometry, new DisplayMaterial(Color.White));
                    }


                    DisplayChildren(e, parcel.Children[i], depth + 1);
                }
                else if (parcel.Children[i].Type==ParcelType.Green)
                {
                    //e.Display.DrawCurve(parcel.Children[i].ParcelCurve,
                    //new DisplayPen() { Color = Color.Green, Thickness = 3 });

                    e.Display.DrawBrepShaded(parcel.Children[i].Geometry, new DisplayMaterial(Color.LightGreen));
                }
            }

        }

        #endregion
    }
}
