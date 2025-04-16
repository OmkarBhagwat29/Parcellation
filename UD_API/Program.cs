using Parcellation_API.Data;
using Parcellation_API.Features.Parcellation;



var builder = WebApplication.CreateBuilder(args);

//create sqlite db
var connection = builder.Configuration.GetConnectionString("Parcellation");
builder.Services.AddSqlite<ParcellationContext>(connection);

var app = builder.Build();

app.MapGet("/", () => "Hello World!");


app.MapParcellation();

await app.InitializeDbAsync();

app.Run();
