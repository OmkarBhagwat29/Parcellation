using Microsoft.EntityFrameworkCore;
using Parcellation_API.Models;

namespace Parcellation_API.Data;

public class ParcellationContext : DbContext
{
    public ParcellationContext(DbContextOptions<ParcellationContext> options) : base(options) { }

    public DbSet<ParcellationInput> Inputs => Set<ParcellationInput>();
    public DbSet<ParcellationOutput> Outputs => Set<ParcellationOutput>();
}
