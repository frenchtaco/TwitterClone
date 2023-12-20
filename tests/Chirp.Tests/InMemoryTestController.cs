using Microsoft.Data.Sqlite;
using DBContext;
using Microsoft.EntityFrameworkCore;
using Chirp.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Chirp.Interfaces;

public class InMemoryTestController //Inspired by https://learn.microsoft.com/en-us/ef/core/testing/testing-without-the-database
{

    private readonly SqliteConnection _connection;
    private readonly DbContextOptions<DatabaseContext> _contextOptions;
    private readonly IServiceProvider _serviceProvider;
    private readonly DatabaseContext _databaseContext;

    public InMemoryTestController()
    {
        // 01. We create the connection string and open our SQLite DB:
        _connection = new SqliteConnection("filename=:memory:");
        _connection.Open();

        // 02. We specify that we are using an SQLite server:
        _contextOptions = new DbContextOptionsBuilder<DatabaseContext>().UseSqlite(_connection).Options;

        // 03. We configure the services:
        var services = new ServiceCollection();
        ConfigureServices(services);

        _serviceProvider = services.BuildServiceProvider();
        _serviceProvider.GetRequiredService<DatabaseContext>().Database.EnsureCreated();
        _databaseContext = _serviceProvider.GetRequiredService<DatabaseContext>();
    }

    private void ConfigureServices(IServiceCollection services)
    {
        services.AddDbContext<DatabaseContext>(options => options.UseSqlite(_connection));
        services.AddSingleton<IAuthorRepository, AuthorRepository>();
        services.AddSingleton<ILikeDisRepository, LikeDisRepository>();
        services.AddSingleton<ICheepRepository, CheepRepository>();
    }

    public DatabaseContext GetDatabaseContext() { return _databaseContext; }
    public IServiceProvider ServiceProvider => _serviceProvider;

    public void Dispose()
    {
        _connection.Dispose();
        (_serviceProvider as IDisposable)?.Dispose();
    }
}
