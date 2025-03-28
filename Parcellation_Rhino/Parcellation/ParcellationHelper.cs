
using Eto.Forms;
using Microsoft.Web.WebView2.Wpf;
using Rhino.DocObjects;
using Rhino.DocObjects.Custom;
using Rhino.Geometry;
using Rhino.Input.Custom;
using Rhino.UI;
using System.Runtime.CompilerServices;
using System.Text.Json;
using UrbanDesign.Helper.Inputs;
using UrbanDesign.Models;



namespace UrbanDesign.Parcellation
{
    public static class ParcellationHelper
    {
        public static ParcelSystem System = new();
        public static WebView2 View;

        public static object GetSystemJson()
        {
            return System.ToBuildingsParcelJson();
        }

        #region Selection
        public static void SelectParcel()
        {
            var crvObj = RhinoSelect.SelectCurve("Select a Parcel Boundary");

            if (crvObj == null)
                return;

            var crv = crvObj.Curve();

            var userData = crvObj.Geometry().UserData.Find(typeof(ParcelObject));



            if (crv.IsClosed)
            {
                if (userData is null)
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
                crvs.ForEach(c =>
                {

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

            if (System.Parcel is null || System.Parcel.RoadNetwork is null)
                return;

            System.CreateBuildingParcels();
        }

        public static void SelectGreenZones()
        {
            var pointObjs = RhinoSelect.SelectObjects("Select Green Points", Rhino.DocObjects.ObjectType.Point);

            if (pointObjs == null || pointObjs.Length == 0)
                return;

            var points = pointObjs.Select(pObj => pObj.Point().Location);

            pointObjs.ToList().ForEach(ptObj =>
            {

                var att = ptObj.Geometry().UserData.Find(typeof(GreenPointObject));

                if (att == null)
                    ptObj.Geometry().UserData.Add(new GreenPointObject { IsGreenPoint = true, GreenZonId = Guid.NewGuid() });

            });

            System.GreenZones = points.ToList();

            if (System.Parcel is null || System.Parcel.RoadNetwork is null)
                return;

            System.ApplyGreenZone();
            System.CreateSubParcelsFromMinorRoads();

        }

        #endregion

        #region Functionality
        public static void Evaluate()
        {
            if (System.Parcel.RoadNetwork == null || System.Parcel == null)
                return;

            System.Enabled = true;
            System.Evaluate();
            //SendPieChartInfoOfSubParcelAreaDistribution();
            SendPieCharInfoOfParcelTypeAreaDistribution();
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
            Evaluate();
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

        #endregion


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
                if (obj.Geometry is Point point)
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
                    System.Parcel.SetParcelCurve(c);
                    System.Parcel.RoadNetwork = roadNetwork;

                    System.Evaluate();
                    SendPieCharInfoOfParcelTypeAreaDistribution();
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
                SendPieCharInfoOfParcelTypeAreaDistribution();

                return;
            }

            if (greenData is not null)
            {
                var pt = obj.Geometry as Point;
                System.GreenZones.Add(pt.Location);

                System.ApplyGreenZone();

                System.CreateSubParcelsFromMinorRoads();
                SendPieCharInfoOfParcelTypeAreaDistribution();

            }

        }


        public static void DeleteRhinoObject(object sender, Rhino.DocObjects.RhinoObjectEventArgs e)
        {


            RemoveRoad(e);


            RemoveGreenPoint(e);

        }

        static void RemoveGreenPoint(RhinoObjectEventArgs e)
        {
            var obj = e.TheObject;
            var greenData = obj.Geometry.UserData.Find(typeof(GreenPointObject)) as GreenPointObject;


            if (greenData != null)
            {
                if (obj is PointObject ptObj)
                {
                    var pt = ptObj.Geometry as Point;
                    var greenPt = System.GreenZones.FirstOrDefault(gZ =>
                    {


                        return gZ.CompareTo(pt.Location) == 0;

                    });
                    if (pt != null)
                        System.GreenZones.Remove(greenPt);
                }

            }
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


        #region WebViewHelper

        public static void SendParcelSelectedInfo()
        {
            var res = new { eventType = "info_message", message = $"Parcel Selected!\nArea: {Math.Round(System.Parcel.Props.Area, 2)}" };

            var resString = JsonSerializer.Serialize(res);

            View.CoreWebView2.PostWebMessageAsJson(resString);
        }

        public static void SendRoadSelectedInfo()
        {
            var res = new { eventType = "info_message", message = $"{System.Parcel.RoadNetwork.Roads.Count} Roads Selected!" };

            var resString = JsonSerializer.Serialize(res);

            View.CoreWebView2.PostWebMessageAsJson(resString);
        }

        public static void SendPieChartInfoOfSubParcelAreaDistribution()
        {
            //parcel by manjor roads
            var parcels = System.GetParcelsOnLevel(2);


            if (parcels.Count == 0)
                return;

            var total = parcels.Sum(p => p.Props.Area);

            var labels = Enumerable.Range(0, parcels.Count).Select(i => "zone_"+i.ToString()+$": {Math.Round(100.0*(parcels[i].Props.Area/total),2)}%").ToList();

            var dataSet = new Dataset()
            {
                Label = "area",
                Data = Enumerable.Range(0, parcels.Count)
                .Select(i => Math.Round(parcels[i].Props.Area,2))
                .ToList(),
                BackgroundColor = Enumerable.Range(0, parcels.Count)
        .Select(i => parcels[i].DisplayColor)
        .ToList(),
                HoverOffset = 4

            };


            var pieChartData = new PieChartProps()
            {
                Labels = labels,
                Datasets = [dataSet],
                Title = "Sub Parcel Area Distribution"
            };

            var req = new { eventType = "pie_chart_data", message = pieChartData };

            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase, // Ensures JSON uses lowercase property names
                WriteIndented = true
            };

            var pieString = JsonSerializer.Serialize(req,jsonOptions);


            View.CoreWebView2.PostWebMessageAsJson(pieString);
        }

        public static void SendPieCharInfoOfParcelTypeAreaDistribution()
        {
            //parcel by manjor roads
            var minorRoadParcels = System.GetParcelsOnLevel(3);
            var majorRoadParcels = System.GetParcelsOnLevel(2);


            if (minorRoadParcels.Count == 0)
                return;

            var greenParcels = majorRoadParcels.Where(p => p.Type == ParcelType.Green).ToList();

            minorRoadParcels.ForEach(p => {
                if (p.Type == ParcelType.Green)
                {
                    greenParcels.Add(p);
                }
            
            });

            var residentialParcels = minorRoadParcels.Where(p => p.Type == ParcelType.Residential);

            var greenTotalArea = greenParcels.Sum(p => p.Props.Area);

            var resTotalArea = residentialParcels.Sum(p => p.Props.Area);

            var roadArea = System.Parcel.Props.Area - (greenTotalArea + resTotalArea);

            var totalArea = System.Parcel.Props.Area;



            var title = "Zone Area Distribution";
            var labels = new List<string>()
            {
                $"Green Zone: {Math.Round(100.0*(greenTotalArea/totalArea),2)}%",
                $"Residential Zone: {Math.Round(100.0*(resTotalArea/totalArea),2)}%",
                $"Roads: {Math.Round(100.0*(roadArea/totalArea),2)}%"
            };


            var dataSet = new Dataset()
            {
                Label = "area",
                Data = new List<double>() { Math.Round(greenTotalArea,2),Math.Round(resTotalArea,2),Math.Round(roadArea,2)},
                BackgroundColor = new List<string>() { "rgb(34, 139, 34)","rgb(255,215,0)", "rgb(156,156,156)" },
                HoverOffset = 4

            };


            var pieChartData = new PieChartProps()
            {
                Labels = labels,
                Datasets = [dataSet],
                Title = title
            };

            var req = new { eventType = "pie_chart_data", message = pieChartData };

            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase, // Ensures JSON uses lowercase property names
                WriteIndented = true
            };

            var pieString = JsonSerializer.Serialize(req, jsonOptions);


            View.CoreWebView2.PostWebMessageAsJson(pieString);
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

        protected override void OnDuplicate(UserData source)
        {
            if (source is GreenPointObject src)
            {
                GreenZonId = src.GreenZonId;
                IsGreenPoint = src.IsGreenPoint;
            }
        }
    }
}
