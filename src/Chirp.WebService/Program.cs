using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using DBContext;
using Chirpin.Startup;
using Chirpin.Data;

/*
    @DESCRIPTION:
        - This our program entry point. 
        - The entire Entity Framework Core relies heavily on the 'Fluent Interface' API design pattern used in OOP languages.
        - 

*/


namespace ChirpApp;

public class Program 
{
    public static void Main(string[] args) 
    {
        var server = CreateNewServerBuilder(args).Build();

        CreateDb(server);

        server.Run();
    }

    private static void CreateDb(IHost server)
    {
        using (var scope = server.Services.CreateScope())
        {
            var services = scope.ServiceProvider;
                try
                {
                    var context = services.GetRequiredService<DatabaseContext>();

                    // if(!context.Database.EnsureCreated()) {
                    //     Console.WriteLine("Migration has not occurred");
                    // }

                    DbInitializer.SeedDatabase(context);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Migration and or Database Seeding Error: " + e.Message);
                }
        }
    }

    public static IHostBuilder CreateNewServerBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
            


    
}


/*

 app.UseEndpoints(endpoints =>
{
    endpoints.MapRazorPages();
});

*/