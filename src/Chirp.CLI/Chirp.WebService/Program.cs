//namespace App; 
using SimpleDB;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

//using Microsoft.EntityFrameworkCore;

/*public class App
{
    public static WebApplication (string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var app = builder.Build();

        app.MapGet("/cheeps", () => "Bonjour Le Monde!");

        app.MapGet("/cheeps", () => new List<string>()
        {
            new string("string 1"),
            new string("string 2"),
            new string("string 3"),
        });

        return app; 
    }
}*/

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

CSVDatabase<Cheep> database_cheeps =  (CSVDatabase<Cheep>)Activator.CreateInstance(typeof(CSVDatabase<Cheep>), nonPublic: true);//because something

app.MapGet("/cheep", () => new Cheep("Billy Bones", "fuckery mockery", (long)DateTime.Now.Ticks));

app.MapGet("/cheeps", () => {
    var cheep = database_cheeps.Read();
    return Results.Json(cheep);
});

app.Run();





//builder.Services.AddDbContext<IDatabaseRepository<CSVDatabase<Cheep>>>(opt => opt.UseInMemoryDatabase("SimpleDB"));
//builder.Services.AddDatabaseDeveloperPageExceptionFiler();







// Send an asynchronous HTTP GET request and automatically construct a Cheep object from the
// JSON object in the body of the response




/*app.MapGet("/cheeps", async (CSVDatabase<Cheep> cheeps) =>
    await cheeps.CSVDatabase)*/

/*app.Use(async (Cheep cheep) =>
{
    Program program = new Program();
    program.CoreProgram(args);

    await cheep; 
    
});*/



public record Cheep(string Author, string Message, long Timestamp);
