using Rhino;
using UD.Parcellation;
using UrbanDesign.AI;

public sealed partial class Functions
{
    [Function("Hides all geometries in the scene.")]
    string Hide()
    {
        ParcellationHelper.Hide(true);
        return "Geometries are hidden";
    }

    [Function("Unhides all geometries in the scene.")]
    string ShowAll()
    {
        ParcellationHelper.Hide(false);

        // Run Ollama here

        return "Geometries are visible";
    }

    [Function("Generates a pie chart specifically showing the area distribution of sub-parcels divided by major roads.")]
    string ShowSubParcelAreaChart()
    {
        ParcellationHelper.SendPieChartInfoOfSubParcelAreaDistribution();
        return "Pie Chart generated for sub-parcel area distribution.";
    }

    [Function("Generates a pie chart specifically showing the area distribution of different parcel types (e.g., residential, green, roads).")]
    string ShowParcelTypeAreaChart()
    {
        ParcellationHelper.SendPieCharInfoOfParcelTypeAreaDistribution();
        return "Pie Chart generated for parcel land type distribution.";
    }

    [Function("Selects and designates / assigns / apply / make parcels as green spaces/zones based on a specified percentage of total land area.")]
    string ApplyGreenZones(string data)
    {
        if (data.Contains("%"))
        {
            data = data.Replace("%", "");
        }

        if (double.TryParse(data, out double percentage))
        {
            var greenPercentageAchieved = ParcellationHelper.SetGreenSpacesBasedOnPercentage(percentage);
            RhinoDoc.ActiveDoc.Views.Redraw();
            return $"Green zone percentage achieved: {Math.Round(greenPercentageAchieved, 2)} %";
        }

        return $"Unable to assign {data}";

    }

    [Function("Apply / designates / assigns / make Commercial type with specified percentage to building parcels based on attractor only if mentioned else commercial type will be assigned randomly")]
    string ApplyCommercialTypes(string data)
    {
        if (data.Contains("%"))
        {
            data = data.Replace("%", "");
        }

        if (double.TryParse(data, out double percentage))
        {
            var percentageAchieved = ParcellationHelper.SetCommercialTypeByPercentage(percentage);
            RhinoDoc.ActiveDoc.Views.Redraw();
            return $"Commercial types applied: {Math.Round(percentageAchieved, 2)} %";
        }

        return $"Unable to assign {data}";

    }

    [Function("Handles general urban design inquiries or any question that does not match other specific functions. Use this function when no other function explicitly applies.")]
    string GetContent()
    {
        var content = OllamaHelper.GetOllamaResponseFromFunctionCalling(ParcellationHelper.SendAiResponseToUI);
        return string.Empty;
    }

}

