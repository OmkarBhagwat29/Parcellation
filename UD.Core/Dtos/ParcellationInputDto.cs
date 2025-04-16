
namespace UrbanDesign.Core.Dtos
{
    public record GetAllParcellationInputDto(int Id,
    string ParcelCurve,
    string RoadNetwork, double MajorRoadWidth, double MinorRoadWidth);
}
