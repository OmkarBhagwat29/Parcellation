
using Rhino.Display;
using Rhino.Geometry;
using System.Reflection;

using UD.GeoJson;
using UD.Helper;


namespace UD.Map
{
    public class MapSystem : DisplayConduit
    {
        public bool IsExecuting = false;
        public List<IMapObject> MapObjects = [];

        public bool ShowBuildings = true;
        public bool ShowGreenSpaces = true;
        public bool ShowRoads = true;
        public bool ShowTransportation = true;
        public bool ShowWaterBodies = true;
        public bool ShowLandUse = true;

        public bool ShowShapes = true;

        public BoundingBox Bbx;

        public MapSystem()
        {
            this.Enabled = true;
        }

        public void CalculateSystemBoundingBox()
        {
            var box = BoundingBox.Empty;

            foreach (var item in MapObjects)
            {
                box.Union(item.Bbx);
            }
            this.Bbx = box;
        }

        public void TransformSystem(Transform xForm)
        {

            this.MapObjects.ForEach(item=>item.Transform(xForm));

            this.Bbx.Transform(xForm);
        }

        public void SetMapObjects(GeoJsonRoot root)
        {
            this.MapObjects.Clear();
            // Get all public instance properties of GeoJsonRoot
            PropertyInfo[] properties = typeof(GeoJsonRoot).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var property in properties)
            {
                // Get the value of the current property from the root object
                var value = property.GetValue(root);

                // Check if the value is a list (you can refine this to List<T> if needed)
                if (value is System.Collections.IEnumerable list && !(value is string))
                {
                    //Console.WriteLine($"Property: {property.Name}");

                    foreach (var item in list)
                    {
                        //Console.WriteLine($" - Item: {item}");
                        // You can add your own logic here to handle each item

                        if (item is BuildingModel bm)
                        {
                            var building = new Building();
                            building.Type = bm.BuildingType;
                            building.SetMapData(bm);

                            this.MapObjects.Add(building);
                        }
                        else if (item is GreeneryModel gm)
                        {
                            var greenery = new GreenSpace();
                            greenery.Type = gm.GreenSpaceType;
                            greenery.SetMapData(gm);

                            this.MapObjects.Add(greenery);
                        }
                        else if (item is RoadModel rm)
                        {
                            var road = new Road();
                            road.Type = rm.HighwayType;
                            road.SetMapData(rm);

                            this.MapObjects.Add(road);
                        }
                        else if (item is TransportationModel tM)
                        {
                            var t = new Transportation();
                            t.Type = tM.TransportationType;
                            t.SetMapData(tM);

                            this.MapObjects.Add(t);
                        }
                        else if (item is WaterBodyModel wM)
                        {
                            var w = new WaterBody();
                            w.Type = wM.WaterBodyType;
                            w.SetMapData(wM);

                            this.MapObjects.Add(w);
                        }
                        else if (item is LandUseModel lU)
                        {
                            var l = new Landuse();
                            l.Type = lU.LandType;
                            l.SetMapData(lU);

                            this.MapObjects.Add(l);
                        }
                    }
                }
            }

            this.CalculateSystemBoundingBox();
        }

        public void ComputeShapes()
        {
            this.MapObjects.ForEach(item =>
            {
                var shapes = Brep.CreatePlanarBreps(item.Outline,0.001);

                if (shapes is not null)
                {
                    item.Shapes = shapes.ToList();
                }
            });
        }

        public void SendToWebView()
        {
            
        }

        #region Display

        void DisplayTags()
        {
            
        }

        protected override void CalculateBoundingBox(CalculateBoundingBoxEventArgs e)
        {
            e.IncludeBoundingBox(this.Bbx);
        }

        private void DisplayMapObject<T>(DrawEventArgs e,int curveThickness) where T:IMapObject
        {

            this.MapObjects.Where(o => o is T)
                           .ToList()
                           .ForEach(b => b.Outline.ForEach(o =>
                            {
                                e.Display.DrawCurve(o, b.Color, curveThickness);
                            }));


            if (this.ShowShapes)
            {
                this.MapObjects.Where(o => o is T)
                                .ToList()
                                .ForEach(b => b.Shapes.ForEach(shape =>
                                {
                                    e.Display.DrawBrepShaded(shape, new DisplayMaterial(b.Color));

                                }));
            }

        }

        #endregion

        protected override void DrawForeground(DrawEventArgs e)
        {
            if(this.IsExecuting)
                return;

            if (this.ShowBuildings)
            {
                DisplayMapObject<Building>(e, 2);
            }

            if (this.ShowGreenSpaces)
            {
                DisplayMapObject<GreenSpace>(e, 1);
            }

            if (this.ShowRoads)
            {
                DisplayMapObject<Road>(e, 1);
            }


            if (this.ShowTransportation)
            {
               DisplayMapObject<Transportation>(e, 3);
            }

            if (this.ShowWaterBodies)
            {
               DisplayMapObject<WaterBody>(e, 1);
            }

            if(this.ShowLandUse)
            {
                DisplayMapObject<Landuse>(e, 1);
            }

        }

    }
}
