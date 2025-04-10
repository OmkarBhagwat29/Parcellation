using System;
using Microsoft.EntityFrameworkCore;
using Parcellation_API.Data;
using Parcellation_API.Features.Parcellation.GetParcellationInputs;

namespace Parcellation_API.Features.Parcellation.GetParcellation;

public static class GetParcellationEndpoint
{
    public static void MapGetParcellationInputs(this IEndpointRouteBuilder app)
    {
        app.MapGet("/inputs/all", async (ParcellationContext dbContext) =>
        {

            var inputs = await dbContext.Inputs.Select(x => new GetAllParcellationInputDto(x.Id, x.ParcelCurve, x.RoadNetworkCurves, x.MajorRoadWidth, x.MinorRoadWidth))
            .AsNoTracking()
            .ToArrayAsync();

            return Results.Ok(inputs);
        });
    }
}
