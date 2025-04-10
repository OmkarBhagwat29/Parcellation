using System;

namespace Parcellation_API.Features.Parcellation.GetParcellationInputs;

public record GetAllParcellationInputDto(int Id,
string ParcelCurve,
string RoadNetwork, double MajorRoadWidth, double MinorRoadWidth);
