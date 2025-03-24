
using Microsoft.Web.WebView2.Core;
using Rhino.DocObjects.Custom;
using Rhino.Geometry;
using System.Text.Json;
using System.Windows;
using UrbanDesign.Helper.Inputs;
using UrbanDesign.Ui.Models;


namespace UrbanDesign.Parcellation
{
    public static class ParcellationHelper
    {
        public static ParcelSystem System = new ();

        public static object GetSystemJson()
        {
            return System.ToBuildingsParcelJson();
        }

        public static void SelectParcel()
        {
            var crvObj = RhinoSelect.SelectCurve("Select a Parcel Boundary");

            if (crvObj == null)
                return;

            var crv = crvObj.Curve();

            var userData = crvObj.Geometry().UserData.Find(typeof(ParcelObject));



            if (crv.IsClosed)
            {
                if(userData is null)
                    crv.UserData.Add(new ParcelObject { IsParcelObject = true });

                //WebView.CoreWebView2.PostWebMessageAsString($"Parcel Selected with Area: " +
                //    $"{Math.Round(AreaMassProperties.Compute(crv).Area, 2)} units");

                System.Parcel = new Parcel(crv);
            }

        }

        public static void SelectRoadNetwork()
        {
            var roadCurveObjs = RhinoSelect.SelectCurves("Select Road Network Curves");

            if (roadCurveObjs == null || roadCurveObjs.Length == 0)
                return;

            var crvs = roadCurveObjs.Select(ro => ro.Curve()).ToList();


            if (crvs.Count > 0)
            {
                crvs.ForEach(c => {

                    var userData = c.UserData.Find(typeof(RoadObject));

                    if (userData is null)
                    {
                        var roadUserData = new RoadObject { IsRoadObject = true, RoadId = Guid.NewGuid() };
                        c.UserData.Add(roadUserData);
                    }
                });

                //this.WebView.CoreWebView2.PostWebMessageAsString($"{crvs.Count} Roads selected");

                System.Parcel.RoadNetwork = new RoadNetwork(crvs.Select(c => new Road(c)).ToList());
            }
        }

        public static void Evaluate()
        {
            if (System.Parcel.RoadNetwork == null || System.Parcel == null)
                return;

            System.Enabled = true;
            System.Evaluate();
        }

        public static void Reset()
        {
            System.Enabled = false;
            System = new();
        }

        public static void Hide(bool visibility)
        {
            System.Enabled = !visibility;
        }



        public static void SetMinimumSubParcelArea(double area)
        {
            System.MinimumSubParcelArea = area;

            System.SetParcelTypeBasedOnArea();
            System.CreateBuildingParcels();
        }
        public static void SetMajorRoadWidth(double roadWidth)
        {
            System.MajorRoadWidth = roadWidth;
            System.Evaluate();
        }

        public static void SetMinorRoadWidth(double roadWidth)
        {
            System.MinorRoadWidth = roadWidth;
           System.CreateSubParcelsFromMinorRoads();
        }

        public static void SetSubParcelSize(double depth, double width)
        {
            System.SubParcelDepth = depth;
            System.SubParcelWidth = width;

   
            System.CreateSubParcelsFromMinorRoads();
        }

        public static void SetBuildingPlotSizes(double depth, double width)
        {

            System.BuildingPlotDepth = depth;
            System.BuildingPlotWidth = width;

            System.CreateBuildingParcels();
        }

        public static void SetBuildingPlotDepthRange(double minDepth, double maxDepth)
        {
            System.BuildingPlotDepth_Min = minDepth;
            System.BuildingPlotDepth_Max = maxDepth;

            System.CreateBuildingParcels();
        }

        public static void SetBuildingPlotWidthRange(double minWidth, double maxWidth)
        {
            System.BuildingPlotWidth_Min = minWidth;
            System.BuildingPlotWidth_Max = maxWidth;

            System.CreateBuildingParcels();
        }

        public static void SelectCityAttractor()
        {
            var pointObj = RhinoSelect.SelectObject("Select Attractor Point", Rhino.DocObjects.ObjectType.Point);


            if (pointObj == null)
                return;

            var point = pointObj.Point().Location;

            var att = pointObj.Geometry().UserData.Find(typeof(AttractorObject));

            if (att == null)
                pointObj.Geometry().UserData.Add(new AttractorObject { IsAttractor = true });

            System.Attractor = point;
            System.CreateBuildingParcels();
        }

        #region interaction
        public static void ObjectModified(object sender, Rhino.DocObjects.RhinoObjectEventArgs e)
        {
            var obj = e.TheObject;

            var parcelUserData = obj.Geometry.UserData.Find(typeof(ParcelObject)) as ParcelObject;

            var roadUserData = obj.Geometry.UserData.Find(typeof(RoadObject)) as RoadObject;

            var attractorData = obj.Geometry.UserData.Find(typeof(AttractorObject)) as AttractorObject;

            var greenData = obj.Geometry.UserData.Find(typeof(GreenPointObject)) as GreenPointObject;

            if (parcelUserData is null && roadUserData is null && attractorData is null && greenData is null)
                return;


            if (attractorData != null)
            {
                if (obj.Geometry is Rhino.Geometry.Point point)
                {
                   System.Attractor = point.Location; // extracts the point3d
                    System.CreateBuildingParcels();
                }
                return;
            }

            //recompute

            if (parcelUserData is not null)
            {
                //set parcel to parcellation 
                //event will recompute

                if (obj.Geometry is Curve c)
                {
                    var roadNetwork = System.Parcel.RoadNetwork;
                    System.Parcel = new Parcel(c);
                    System.Parcel.RoadNetwork = roadNetwork;

                    System.Evaluate();

                }
                return;
            }

            if (roadUserData is not null && roadUserData.IsRoadObject)
            {
                var cv = obj.Geometry as Curve;


                //event will recompute

                var roads = System.Parcel.RoadNetwork.Roads;
                roads.Add(new Road(cv));
                System.Parcel.RoadNetwork = new RoadNetwork(roads);
                    System.Evaluate();
                

                return;
            }

        }


        public static void DeleteRhinoObject(object sender, Rhino.DocObjects.RhinoObjectEventArgs e)
        {

            var obj = e.TheObject;


           RemoveRoad(e);

           // var greenData = obj.Geometry.UserData.Find(typeof(GreenPointObject)) as GreenPointObject;


            //if (greenData != null)
            //{
            //    if (obj.Geometry is Point pt)
            //    {
            //        var greenPt = _parcellation.GreenPoints.Where(p => p.CompareTo(pt.Location) == 0).FirstOrDefault();


            //        _parcellation.GreenPoints.Remove(greenPt);


            //    }
            //    return;
            //}




        }


        static void RemoveRoad(Rhino.DocObjects.RhinoObjectEventArgs e)
        {
            var roadUserData = e.TheObject.Geometry.UserData.Find(typeof(RoadObject)) as RoadObject;

            if (roadUserData is not null)
            {
                var roads = System.Parcel.RoadNetwork.Roads;

                var modifiedRoadIndex = roads.FindIndex(r =>
                {
                    var uD = r.Curve.UserData.Find(typeof(RoadObject)) as RoadObject;

                    if (uD != null)
                    {
                        if (uD.RoadId == roadUserData.RoadId)
                            return true;
                    }

                    return false;
                });


                if (modifiedRoadIndex != -1)
                {
                    System.Parcel.RoadNetwork.Roads.RemoveAt(modifiedRoadIndex);
                }

            }
        }

        #endregion

    }



    public class ParcelObject : UserData
    {
        public bool IsParcelObject { get; set; }
        public override bool ShouldWrite => true;

    }

    public class RoadObject : UserData
    {
        public Guid RoadId { get; set; }
        public bool IsRoadObject { get; set; }

        public override bool ShouldWrite => true;

        protected override void OnDuplicate(UserData source)
        {
            if (source is RoadObject src)
            {
                RoadId = src.RoadId;
                IsRoadObject = src.IsRoadObject;
            }
        }

    }

    public class AttractorObject : UserData
    {
        public bool IsAttractor { get; set; } = false;
        public override bool ShouldWrite => true;
    }

    public class GreenPointObject : UserData
    {
        public bool IsGreenPoint { get; set; } = false;
        public Guid GreenZonId;
        public override bool ShouldWrite => true;
    }
}
