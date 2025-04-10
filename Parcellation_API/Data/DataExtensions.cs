using Microsoft.EntityFrameworkCore;

namespace Parcellation_API.Data
{
    public static class DataExtensions
    {
        public static async Task InitializeDbAsync(this WebApplication app)
        {
            await app.MigrateDbAsync();
        }

        static async Task MigrateDbAsync(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ParcellationContext>();

            await dbContext.Database.MigrateAsync();
        }
    }
}
