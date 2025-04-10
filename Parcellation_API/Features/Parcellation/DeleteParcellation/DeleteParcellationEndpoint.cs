using System;
using Microsoft.EntityFrameworkCore;
using Parcellation_API.Data;

namespace Parcellation_API.Features.Parcellation;

public static class DeleteParcellationEndpoint
{
    public static void MapDeleteParcellation(this IEndpointRouteBuilder app)
    {
        app.MapDelete("/all", async (ParcellationContext dbContext) =>
        {

            await dbContext.Inputs.ExecuteDeleteAsync();
            await dbContext.Outputs.ExecuteDeleteAsync();

            return Results.NoContent();
        });
    }
}
