﻿
using Microsoft.Web.WebView2.Wpf;
using Rhino;
using Rhino.DocObjects;
using Rhino.DocObjects.Custom;
using Rhino.Geometry;

using UrbanDesign.AI;
using UrbanDesign.Core.Dtos;
using UrbanDesign.Core.Services;
using UrbanDesign.Helper.Inputs;
using UrbanDesign.Models;




namespace UD.Parcellation
{
    public static class ParcellationHelper
    {
        public static ParcelSystem System = new();
        public static WebView2 View;
        static Random random = new Random();

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
            var pointObj = RhinoSelect.SelectObject("Select Attractor Point", ObjectType.Point);


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
            var pointObjs = RhinoSelect.SelectObjects("Select Green Points", ObjectType.Point);

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

        public static void SetOllamaContext()
        {
            var mainParcelObject = new { ParcelArea = System.Parcel.Props.Area, ParcelCenter = System.Parcel.Props.Centroid, Level = "City" };

            var subParcels = System.GetParcelsOnLevel(2);
            var subParcelObjects = Enumerable.Range(0, subParcels.Count
                ).Select(index => new
                {
                    Index = index + 1,
                    ParcelArea = subParcels[index].Props.Area,
                    ParcelCenter = subParcels[index].Props.Centroid,
                    ParcelType = subParcels[index].Type,
                    Level = "Zone"
                })
                .ToList();

            // var buildingParcels = System.GetParcelsOnLevel(4);
            // var buildingParcelObjects = buildingParcels.Select(p => new { ParcelArea = p.Props.Area, ParcelCenter = p.Props.Centroid, ParcelType = p.Type }).ToList();

            var obj = new { CityBoundary = mainParcelObject, Zones = subParcelObjects };
            var contextString = JsonSerializer.Serialize(obj);

            OllamaHelper.Context = contextString;

        }

        public static double SetGreenSpacesBasedOnPercentage(double greenPercentage)
        {
            System.GreenPercentage = greenPercentage;
            var subParcels = System.GetParcelsOnLevel(2);
            var totalSubParcelArea = subParcels.Sum(p => p.Props.Area);

            // Calculate percentage area for each parcel and sort by descending area
            var parcelData = subParcels.Select(p => new { Percentage = 100.0 * (p.Props.Area / totalSubParcelArea), Parcel = p })
                .ToList();

            List<Parcel> selectedParcels = new List<Parcel>();
            double accumulatedPercentage = 0.0;

            // Greedy selection algorithm: Pick parcels that best help reach the required percentage
            while (accumulatedPercentage < greenPercentage && parcelData.Count > 0)
            {
                // Find the parcel that brings us closest to the target without exceeding too much
                var bestParcel = parcelData
                    .OrderBy(p => Math.Abs(accumulatedPercentage + p.Percentage - greenPercentage))
                    .FirstOrDefault();

                if (bestParcel == null)
                    break;

                selectedParcels.Add(bestParcel.Parcel);
                accumulatedPercentage += bestParcel.Percentage;

                // Remove the selected parcel from available choices
                parcelData.Remove(bestParcel);
            }



            if (selectedParcels.Count > 0)
            {
                System.GreenZones.Clear();
                System.GreenZones = selectedParcels.Select(p => p.Props.Centroid).ToList();

                System.ApplyGreenZone();
                System.CreateSubParcelsFromMinorRoads();
            }

            SetCommercialTypeByPercentage(System.CommercialPercentage, System.CommercialDistributionBasedOnAttractor);


            return accumulatedPercentage;
        }

        public static double SetCommercialTypeByPercentage(double commercialPercentage, bool considerAttractor = true)
        {
            System.CommercialPercentage = commercialPercentage;
            var buildingParcels = System.GetParcelsOnLevel(4);

            var totalBuildingArea = buildingParcels.Sum(p => p.Props.Area);

            var existngCommercials = buildingParcels.Where(p => p.Type == ParcelType.Commercial);

            // start assigning commercial type randomly 
            existngCommercials.ToList().ForEach(c => c.Type = ParcelType.Residential);


            if (System.Attractor is not null && considerAttractor)
            {
                // start assign commercial type based on attractor
                buildingParcels = buildingParcels.OrderBy(b => b.Props.Centroid.DistanceToSquared(System.Attractor.Value)).ToList();
            }
            else
            {

                buildingParcels.Sort((x, y) => random.Next()); // This will shuffle the list
            }

            double accumulatedPercentage = 0;
            foreach (var bP in buildingParcels)
            {


                var percentage = 100.0 * (bP.Props.Area / totalBuildingArea);
                accumulatedPercentage += percentage;

                if (accumulatedPercentage >= commercialPercentage)
                    break;

                bP.Type = ParcelType.Commercial;

            }

            return accumulatedPercentage;
        }

        public static void SendAiResponseToUI(string question, string answer)
        {
            var response = new
            {
                eventType = "ai_response", // Rename 'event' to avoid C# keyword conflict
                message = $"Your Question:\n{question}\n\n" +
                 $"Ai Response:\n{answer}\n\n"
            };

            string jsonResponse = JsonSerializer.Serialize(response);

            View.CoreWebView2.PostWebMessageAsString(jsonResponse);

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

            RhinoDoc.ActiveDoc.Views.Redraw();
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
        public static void ObjectModified(object sender, RhinoObjectEventArgs e)
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
                    //SetCommercialTypeByPercentage(System.CommercialPercentage,System.CommercialDistributionBasedOnAttractor);
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
                    //SendPieCharInfoOfParcelTypeAreaDistribution();
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
                // SendPieCharInfoOfParcelTypeAreaDistribution();

                return;
            }

            if (greenData is not null)
            {
                var pt = obj.Geometry as Point;
                System.GreenZones.Add(pt.Location);

                System.ApplyGreenZone();

                System.CreateSubParcelsFromMinorRoads();
                // SendPieCharInfoOfParcelTypeAreaDistribution();

            }

        }


        public static void DeleteRhinoObject(object sender, RhinoObjectEventArgs e)
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


        static void RemoveRoad(RhinoObjectEventArgs e)
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

            var labels = Enumerable.Range(0, parcels.Count).Select(i => "zone_" + i.ToString() + $": {Math.Round(100.0 * (parcels[i].Props.Area / total), 2)}%").ToList();

            var dataSet = new Dataset()
            {
                Label = "area",
                Data = Enumerable.Range(0, parcels.Count)
                .Select(i => Math.Round(parcels[i].Props.Area, 2))
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

            var pieString = JsonSerializer.Serialize(req, jsonOptions);


            View.CoreWebView2.PostWebMessageAsJson(pieString);
        }

        public static void SendPieCharInfoOfParcelTypeAreaDistribution()
        {
            // Parcels categorized by road levels
            var minorRoadParcels = System.GetParcelsOnLevel(3);
            var majorRoadParcels = System.GetParcelsOnLevel(2);
            var buildingParcels = System.GetParcelsOnLevel(4);

            if (minorRoadParcels.Count == 0)
                return;

            // Green Parcels
            var greenParcels = majorRoadParcels.Where(p => p.Type == ParcelType.Green).ToList();
            minorRoadParcels.ForEach(p =>
            {
                if (p.Type == ParcelType.Green)
                {
                    greenParcels.Add(p);
                }
            });

            // Residential Parcels
            var residentialParcels = buildingParcels.Where(p => p.Type == ParcelType.Residential);

            // Commercial Parcels
            var commercialParcels = buildingParcels.Where(p => p.Type == ParcelType.Commercial);

            // Area Calculations
            var greenTotalArea = greenParcels.Sum(p => p.Props.Area);
            var resTotalArea = residentialParcels.Sum(p => p.Props.Area);
            var comTotalArea = commercialParcels.Sum(p => p.Props.Area);

            var totalArea = System.Parcel.Props.Area;
            var roadArea = totalArea - (greenTotalArea + resTotalArea + comTotalArea);

            // Pie Chart Labels
            var title = "Zone Area Distribution";
            var labels = new List<string>
    {
        $"Green Zone: {Math.Round(100.0 * (greenTotalArea / totalArea), 2)}%",
        $"Residential Zone: {Math.Round(100.0 * (resTotalArea / totalArea), 2)}%",
        $"Commercial Zone: {Math.Round(100.0 * (comTotalArea / totalArea), 2)}%",
        $"Roads: {Math.Round(100.0 * (roadArea / totalArea), 2)}%"
    };

            // Data Set for Pie Chart
            var dataSet = new Dataset()
            {
                Label = "area",
                Data = new List<double> { Math.Round(greenTotalArea, 2), Math.Round(resTotalArea, 2), Math.Round(comTotalArea, 2), Math.Round(roadArea, 2) },
                BackgroundColor = new List<string> { "rgb(34, 139, 34)", "rgb(255,215,0)", "rgb(173, 216, 230)", "rgb(156,156,156)" }, // Blue added for Commercial
                HoverOffset = 4
            };

            // Pie Chart Data
            var pieChartData = new PieChartProps()
            {
                Labels = labels,
                Datasets = [dataSet],
                Title = title
            };

            var req = new { eventType = "pie_chart_data", message = pieChartData };

            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };

            var pieString = JsonSerializer.Serialize(req, jsonOptions);

            // Send Data to WebView
            View.CoreWebView2.PostWebMessageAsJson(pieString);
        }


        #endregion

        #region
        public static async Task PostToDbAsync(ParcellationService service)
        {
            if (System != null &&
                    System.Parcel != null &&
                    System.Parcel.RoadNetwork != null)
            {
                var parcelString = JsonSerializer.Serialize(System.Parcel.ParcelCurve);
                var roadNetwork = JsonSerializer.Serialize(System.Parcel.RoadNetwork.Roads.Select(rn => rn.Curve).ToList());

                await service
                 .CreateParcellationAsync(
                 new CreateParcellationDto(parcelString,
                 roadNetwork,
                 System.MajorRoadWidth,
                 System.MinorRoadWidth, Caller.RhinoCommon));
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
