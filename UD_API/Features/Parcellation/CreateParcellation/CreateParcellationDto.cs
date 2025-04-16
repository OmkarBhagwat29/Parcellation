
using System.ComponentModel.DataAnnotations;

namespace Parcellation_API.Features.Parcellation
{

    /// <param name="Boundary">Serialize curve</param>
    /// <param name="RoadNetwork">Serialize List<curve></param>
    /// <param name="MajorRoadWidth"></param>
    /// <param name="MinorRoadWidth"></param>
    public record CreateParcellationDto([Required]string Boundary,
        [Required]string RoadNetwork,
        [Required][Range(9,30)]double MajorRoadWidth,
        [Required][Range(6,18)]double MinorRoadWidth, [Required]string caller);
}
