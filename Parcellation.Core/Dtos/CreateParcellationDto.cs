
namespace UrbanDesign.Core.Dtos
{

    /// <summary>
    /// 
    /// </summary>
    /// <param name="Boundary">Serialize curve</param>
    /// <param name="RoadNetwork">Serialize List<curve></param>
    /// <param name="MajorRoadWidth"></param>
    /// <param name="MinorRoadWidth"></param>
    /// <param name="caller"></param>
    public record CreateParcellationDto(string Boundary,
        string RoadNetwork,
        double MajorRoadWidth,
        double MinorRoadWidth,Caller caller);
}
