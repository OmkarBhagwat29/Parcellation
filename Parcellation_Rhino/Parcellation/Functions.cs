using UrbanDesign.Parcellation;

public sealed partial class Functions
{
    [Function("Hide all the Geometries")]
    string Hide() {

        ParcellationHelper.Hide(true);
        return "";
    }

    [Function("Show all the geometries")]
    string ShowAll()
    {
        ParcellationHelper.Hide(false);
        return "";
    }
}

