using System;

namespace Parcellation_API.Models;

public class ParcellationInput
{
    public int Id { get; set; }

    // Consider storing geometry as JSON string or another serializable format
    public required string ParcelCurve { get; set; } = "";
    public required string RoadNetworkCurves { get; set; } = "";

    public required double MajorRoadWidth { get; set; }
    public required double MinorRoadWidth { get; set; }

    public required string Caller { get; set; } = "";

    public ParcellationOutput? Output { get; set; }

}
