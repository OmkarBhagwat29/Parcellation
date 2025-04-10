using GeometryEngine.RhinoCompute.Parcellation;
using Parcellation_API.Data;
using Rhino.Geometry;

namespace Parcellation_API.Features.RhinoCompute.Parcellation.CreateParcellation
{

    public record CreateParcellationDto(List<Point3d>boundaryPoints,List<List<Point3d>>roadNetwork);

    public static class ComputeParcellationEndpoint
    {
        public static void MapComputeParcellation(this IEndpointRouteBuilder app)
        {
            app.MapPost("/", async (ParcellationContext dbContext, CreateParcellationDto dto) =>
            {
                var boundaryPolyline = new Polyline(dto.boundaryPoints);

                if (!boundaryPolyline.IsClosed)
                    return Results.BadRequest();

                var roadNetwork = dto.roadNetwork.Select(rd => new Polyline(rd).ToNurbsCurve()).ToList();



                ParcellationHelper.Test(boundaryPolyline.ToNurbsCurve(),roadNetwork);
                return Results.Ok();

            }).WithParameterValidation();
        }
    }
}
