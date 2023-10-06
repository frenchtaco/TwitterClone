using Microsoft.EntityFrameworkCore;
using Chirpin.Models;

namespace DBContext;

public class DatabaseContext : DbContext
{
    public virtual DbSet<Cheep> Cheeps { get; set; }
    public virtual DbSet<Author> Authors { get; set; }

    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Cheep>().ToTable("Cheeps");
        modelBuilder.Entity<Author>().ToTable("Authors");
    }
}