using Eto.Forms;
using Microsoft.Web.WebView2.Wpf;
using Rhino;
using Rhino.DocObjects;
using Rhino.DocObjects.Custom;
using Rhino.Geometry;
using System.Text.Json;
using System.Text.Json.Serialization;
using UrbanDesign.AI.Ollama;
using UrbanDesign.Helper.Inputs;
using UrbanDesign.Parcellation ;
using UrbanDesign.Ui.Helpers;
using UrbanDesign.Ui.Models;



namespace UrbanDesign.Ui.ViewModels
{   public class ParcellationViewModel
    {
        RhinoDoc doc = RhinoDoc.ActiveDoc;
        public WebView2 WebView { get; set; }

        public ParcellationViewModel(WebView2 webView)
        {
            this.WebView = webView;

            this.WebView.WebMessageReceived += WebView2_WebMessageReceived;

            Rhino.RhinoDoc.AddRhinoObject += ParcellationHelper.ObjectModified;
            RhinoDoc.DeleteRhinoObject += ParcellationHelper.DeleteRhinoObject;
        }


        public void Dispose()
        {
            ParcellationHelper.Reset();

           RhinoDoc.AddRhinoObject -= ParcellationHelper.ObjectModified;
            RhinoDoc.DeleteRhinoObject -= ParcellationHelper.DeleteRhinoObject;

            foreach (var item in doc.Objects)
            {

                var roadUserData = item.Geometry.UserData.Find(typeof(RoadObject));
                if (roadUserData is not null)
                {
                    item.Geometry.UserData.Remove(roadUserData);
                    continue;
                }

                var parcelUserData = item.Geometry.UserData.Find(typeof(ParcelObject));


                if (parcelUserData is not null)
                {
                    item.Geometry.UserData.Remove(parcelUserData);

                }

            }

            doc.Views.Redraw();
        }

        public void SendAiResponseToUI(string question, string answer)
        {
            var response = new
            {
                eventType = "ai_response", // Rename 'event' to avoid C# keyword conflict
                message = $"Your Question:\n{question}\n\n" +
                 $"Ai Response:\n{answer}\n\n"
            };

            string jsonResponse = JsonSerializer.Serialize(response);

            this.WebView.CoreWebView2.PostWebMessageAsString(jsonResponse);

        }


        private void WebView2_WebMessageReceived(object sender, Microsoft.Web.WebView2.Core.CoreWebView2WebMessageReceivedEventArgs e)
        {
            try
            {
                var data = e.WebMessageAsJson.ToString();

                var obj = JsonSerializer.Deserialize<ParcellationEventModel>(data, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase, allowIntegerValues: true),
                    new JsonNumberConverter() // Custom converter for handling numbers }  // This enables enum string conversion
                    }
                });


                if ((ParcellationHelper.System.Parcel == null || ParcellationHelper.System.Parcel.RoadNetwork == null) && 
                    (obj.Command!=CommandAction.Select_Parcel && obj.Command != CommandAction.Select_Road_Network) )
                {
                    ParcellationHelper.System.Enabled = false;
                    return;
                }

                switch (obj.Command)
                {
                    case CommandAction.Select_Parcel:
                        // obj.Command.UseCommandAction(SelectParcel);
                        obj.Command.UseCommandAction(ParcellationHelper.SelectParcel);
                       
                        
                        ParcellationHelper.Evaluate();

                        if (ParcellationHelper.System.Parcel == null)
                            return;

                        {
                            var res = new { eventType = "info_message", message = $"Parcel Selected!\nArea: {Math.Round(ParcellationHelper.System.Parcel.Props.Area, 2)}" };

                            var resString = JsonSerializer.Serialize(res);

                            this.WebView.CoreWebView2.PostWebMessageAsJson(resString);
                        }


                        break;
                    case CommandAction.Select_Road_Network:
                        //obj.Command.UseCommandAction(SelectRoadNetwork);
                        obj.Command.UseCommandAction(ParcellationHelper.SelectRoadNetwork);
                        ParcellationHelper.Evaluate();

                        if (ParcellationHelper.System.Parcel.RoadNetwork == null)
                            return;

                        {
                            var res = new { eventType = "info_message", message = $"{ParcellationHelper.System.Parcel.RoadNetwork.Roads.Count}Roads Selected!" };

                            var resString = JsonSerializer.Serialize(res);

                            this.WebView.CoreWebView2.PostWebMessageAsJson(resString);
                        }
                        break;
                    case CommandAction.Major_Road_Width:
                        
                        obj.Command.UseCommandAction(() =>
                        {
                            var data = obj.Payload.Value.Deserialize<RoadWidth>();

                            ParcellationHelper.SetMajorRoadWidth(data.Width);
                            //this._parcellation.MajorRoadWidth = data.Width;
                            //var report = _parcellation.SubParcelsReport();
                            //this.PostInformation(report);
                        });
                        break;
                    case CommandAction.Minor_Road_Width:
                        
                        obj.Command.UseCommandAction(() => {
                            var data = obj.Payload.Value.Deserialize<RoadWidth>();
                            //this._parcellation.MinorRoadWidth = data.Width;
                            ParcellationHelper.SetMinorRoadWidth(data.Width);
                        });
                        break;
                    case CommandAction.SUB_PARCEL_SIZES:
                        {
                            var size = obj.Payload.Value.Deserialize<ParcelSize>();

                            //_parcellation.SubParcelDepth = size.ParcelDepth;
                            //_parcellation.SubParcelWidth = size.ParcelWidth;
                            //_parcellation.CalculateParcels();
                            ParcellationHelper.SetSubParcelSize(size.ParcelDepth, size.ParcelWidth);
                        }
 
                        break;
                    case CommandAction.BUILDING_PARCEL_SIZES:
                        {
                          var size = obj.Payload.Value.Deserialize<ParcelSize>();

                            ParcellationHelper.SetBuildingPlotSizes(size.ParcelDepth, size.ParcelWidth);
                        }

                        break;
                    case CommandAction.CITY_ATTRACTOR:

                        ParcellationHelper.SelectCityAttractor();

                        break;
                    case CommandAction.CITY_GREEN_POINTS:
                        var pointObjs = RhinoSelect.SelectObjects("Select Green Points", Rhino.DocObjects.ObjectType.Point);

                        if (pointObjs == null)
                            return;

                        var greenPoints = pointObjs.Select(o => {

                           var userObj = o.Geometry().UserData.Find(typeof(GreenPointObject));

                            if (userObj == null)
                            {
                                o.Geometry().UserData.Add(new GreenPointObject { IsGreenPoint = true,GreenZonId=Guid.NewGuid() });
                            }

                            return o.Point().Location;
                        }).ToList();
     
                        break;
                    case CommandAction.Min_Parcel_Area:
                        var parcelArea = obj.Payload.Value.Deserialize<ParcelArea>();
                        // _parcellation.MinParcelArea = parcelArea.Area;
                        ParcellationHelper.SetMinimumSubParcelArea(parcelArea.Area);

                        break;
                    case CommandAction.PARCEL_DEPTH_RANGE:
                        var depthRange = obj.Payload.Value.Deserialize<MinMax>();

                        ParcellationHelper.SetBuildingPlotDepthRange(depthRange.Min, depthRange.Max);

                    break;
                    case CommandAction.PARCEL_WIDTH_RANGE:
                        var widthRange = obj.Payload.Value.Deserialize<MinMax>();

                        ParcellationHelper.SetBuildingPlotWidthRange(widthRange.Min, widthRange.Max);
                        break;
                    case CommandAction.HIDE:

                        var visibility = obj.Payload.Value.Deserialize<HidePacellation>();

                        ParcellationHelper.Hide(visibility.Hide);

                        break;
                    case CommandAction.AI_QUERY:
                        var aiMessage = obj.Payload.Value.Deserialize<AIMessage>();

                        //var info = ParcellationHelper.GetSystemJson();
                        var query = $"\n{aiMessage.message}";

                        OllamaHelper.GetOllamaResponse(query, "", this.SendAiResponseToUI);

                        break;
                    default:
                        break;
                }

                doc.Views.Redraw();
            }
            catch (Exception ex)
            {
                Rhino.UI.Dialogs.ShowMessage($"An error occurred: {ex.Message}", "Error");
                Rhino.RhinoApp.WriteLine($"Exception: {ex}");

            }
        }


    }


}
