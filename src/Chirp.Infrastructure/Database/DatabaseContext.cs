using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

using Chirp.Models;

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
    public virtual DbSet<Cheep> Cheeps { get; set; }
    public virtual DbSet<Author> Authors { get; set; }

    public DatabaseContext()
    {
        // Must have empty constructor for Compile-Time Migrtion to work.
    }

    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);


        // [Try to move all of this to Entities and use Data Annotations]
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
    }

    //Beleive this is uneccesary the Dbcontext is configured in Startup.cs
    /* protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
     {
         string databasePath = Path.Combine(Path.GetTempPath(), "chirp.db");
         Console.WriteLine("Database Path: " + databasePath);    // [REMOVE-DEV] 

         if(!File.Exists(databasePath)) Console.WriteLine("The database file does NOT exist");

         optionsBuilder.UseSqlite($"Data Source={databasePath}");
     }
     */
}