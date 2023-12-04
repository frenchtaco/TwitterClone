using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

using Chirp.Models;
using Microsoft.Extensions.Configuration;

/*
    @DESCRIPTION:
        - In following is a subclass of the class 'DbContext'.
        - It follows the behavioural pattern 'Unit of Work'. Simply put, this is everything one does in a single business transaction 
        which may alter a databas - [https://en.wikipedia.org/wiki/Unit_of_work].

    @KEY INFO:
        - Our 'Models' folder contains the "Entities" of our SQLite database.
            -> In DBMS, an entity is a piece of data tracked and stored by the system. 
        - The ModelBuilder class acts as a Fluent API, a design pattern based on method chaining - [https://www.entityframeworktutorial.net/efcore/fluent-api-in-entity-framework-core.aspx].

*/

namespace DBContext;
public class DatabaseContext : IdentityDbContext<Author>
{
    public virtual DbSet<Cheep> Cheeps { get; set; } = null!;
    public virtual DbSet<Author> Authors { get; set; } = null!;
    public virtual DbSet<CheepLikeDis> CheepLikeDis { get; set; } = null!;


    public DatabaseContext()
    {
        // Must have empty constructor for Compile-Time Migrtion to work.
    }

    public DatabaseContext(DbContextOptions<DatabaseContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Author>(entity =>
        {
            entity.ToTable("Authors");
            entity.HasIndex(a => a.Email).IsUnique();
        });

        modelBuilder.Entity<Cheep>(entity =>
        {
            entity.ToTable("Cheeps");
            entity.Property(cheep => cheep.Text).HasMaxLength(160);
        });

        modelBuilder.Entity<CheepLikeDis>(entity =>
        {
            entity.ToTable("CheepLikesDislikes");
            entity.HasKey(ld => new { ld.CheepLikeDisId });

            entity.HasMany(ld => ld.Likes)
                .WithMany()
                .UsingEntity(j => j.ToTable("CheepLikes"));

            entity.HasMany(ld => ld.Dislikes)
                .WithMany()
                .UsingEntity(j => j.ToTable("CheepDislikes"));
        });
    }


    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
    }
}