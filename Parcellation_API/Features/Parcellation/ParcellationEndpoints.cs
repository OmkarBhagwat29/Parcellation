

using Parcellation_API.Features.Parcellation.GetParcellation;
using Parcellation_API.Features.RhinoCompute.Parcellation.CreateParcellation;

namespace Parcellation_API.Features.Parcellation
{
    public static class ParcellationEndpoints
    {
        public static void MapParcellation(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/parcellation");

            group.MapCreateParcellation();
            group.MapDeleteParcellation();
            group.MapGetParcellationInputs();

            var computeGroup = app.MapGroup("/compute/parcellation");

            computeGroup.MapComputeParcellation();
        }
    }
}
