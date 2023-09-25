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

var username = Environment.UserName;
long timestamp = (long)DateTime.Now.Ticks;



// The POST command written is: 
// curl -X POST -H "Content-Type: application/json" -d '{"Author":"YourUsername","Message":"YourMessage","Timestamp":1234567890}' http://localhost:5142/cheep
// or equivalently, 
// curl -X POST -d '{"Author":"YourUsername","Message":"YourMessage","Timestamp":1234567890}' http://localhost:5142/cheep





app.MapPost("/cheep", async (HttpContext context) => {
    //var message = context.Request.Body.ToString();

    using (StreamReader reader = new StreamReader(context.Request.Body))
    {
        string message = await reader.ReadToEndAsync();

        var cheep = new Cheep(username, message, timestamp);
        database_cheeps.Store(cheep);
    }
    
    return Results.Created("/cheeps", "Cheep was stored!");
});


//Get request!
//app.MapGet("/cheep", () => new Cheep("Billy Bones", "fuckery mockery", (long)DateTime.Now.Ticks));
app.MapGet("/cheeps", () => {
    var cheep = database_cheeps.Read();
    return Results.Json(cheep);
});



app.Run();
public record Cheep(string Author, string Message, long Timestamp);




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



//public record Cheep(string Author, string Message, long Timestamp);
