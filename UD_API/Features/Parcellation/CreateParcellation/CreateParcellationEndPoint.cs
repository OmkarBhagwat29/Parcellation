
using Parcellation_API.Data;
using Parcellation_API.Models;


namespace Parcellation_API.Features.Parcellation
{
    public static class CreateParcellationEndPoint
    {
        public static void MapCreateParcellation(this IEndpointRouteBuilder app)
        {
            app.MapPost("/", async (ParcellationContext dbContext, CreateParcellationDto dto) =>
            {

                dbContext.Inputs.Add(new ParcellationInput()
                {
                    ParcelCurve = dto.Boundary,
                    RoadNetworkCurves = dto.RoadNetwork,
                    MajorRoadWidth = dto.MajorRoadWidth,
                    MinorRoadWidth = dto.MinorRoadWidth,
                    Caller = dto.caller
                });

                await dbContext.SaveChangesAsync();

                return Results.Ok();

            }).WithParameterValidation();
        }
    }
}
