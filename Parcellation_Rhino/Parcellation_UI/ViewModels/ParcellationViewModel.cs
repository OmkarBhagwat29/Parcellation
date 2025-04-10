

using Microsoft.Web.WebView2.Wpf;
using Rhino;
using UrbanDesign.AI;
using UrbanDesign.Core.Services;
using UrbanDesign.Models;
using UrbanDesign.Parcellation;
using UrbanDesign.Ui.Helpers;



namespace UrbanDesign.Ui.ViewModels
{   public class ParcellationViewModel
    {
        RhinoDoc doc = RhinoDoc.ActiveDoc;
        public WebView2 WebView { get; set; }

        Functions _functions = new Functions();


        ParcellationService _service = new ParcellationService();
        public ParcellationViewModel(WebView2 webView)
        {
            this.WebView = webView;
            ParcellationHelper.View = this.WebView;
            this.WebView.WebMessageReceived += WebView2_WebMessageReceived;

            RhinoDoc.AddRhinoObject += ParcellationHelper.ObjectModified;
            RhinoDoc.DeleteRhinoObject += ParcellationHelper.DeleteRhinoObject;

            OllamaHelper.Request.SetModel("llama3.2")
                .AddFunctions(_functions);

            
        }

        public void Dispose()
        {
            ParcellationHelper.Reset();

           RhinoDoc.AddRhinoObject -= ParcellationHelper.ObjectModified;
            RhinoDoc.DeleteRhinoObject -= ParcellationHelper.DeleteRhinoObject;

            doc.Views.Redraw();
        }




        private void WebView2_WebMessageReceived(object sender, Microsoft.Web.WebView2.Core.CoreWebView2WebMessageReceivedEventArgs e)
        {
            try
            {
                var data = e.WebMessageAsJson.ToString();

                var obj = JsonSerializer.Deserialize<WebViewEventModel>(data, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase, allowIntegerValues: true),
                    new JsonNumberConverter() // Custom converter for handling numbers }  // This enables enum string conversion
                    }
                });

                switch (obj.Command)
                {
                    case CommandAction.Select_Parcel:
                        // obj.Command.UseCommandAction(SelectParcel);
                        obj.Command.UseCommandAction(ParcellationHelper.SelectParcel);
                       
                        
                        ParcellationHelper.Evaluate();

                        if (ParcellationHelper.System.Parcel == null)
                            return;

                        ParcellationHelper.SendParcelSelectedInfo();

                        //ParcellationHelper.SendPieChartInfoOfSubParcelAreaDistribution();

                        break;
                    case CommandAction.Select_Road_Network:

                        obj.Command.UseCommandAction(ParcellationHelper.SelectRoadNetwork);
                        ParcellationHelper.Evaluate();

                        if (ParcellationHelper.System.Parcel.RoadNetwork == null)
                            return;

                        ParcellationHelper.SendRoadSelectedInfo();

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
                        ParcellationHelper.SelectGreenZones();

                        // ParcellationHelper.SetGreenSpacesBasedOnPercentage(55);

                       //ParcellationHelper.SetCommercialTypeByPercentage(15,false);
     
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
                        var query = $"{aiMessage.message}";
                        OllamaHelper.OllamaFunctionCall(_functions, query, ParcellationHelper.SendAiResponseToUI);

                        break;
                    default:
                        break;
                }

                ParcellationHelper.PostToDbAsync(_service);     

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
