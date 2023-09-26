using SimpleDB;
var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

CSVDatabase<Cheep> database_cheeps =  (CSVDatabase<Cheep>)Activator.CreateInstance(typeof(CSVDatabase<Cheep>), nonPublic: true);//because something

var username = Environment.UserName;
long timestamp = (long)DateTime.Now.Ticks;



// The POST command written is: 
// curl -X POST -H "Content-Type: application/json" -d "message" http://localhost:5142/cheep
// or equivalently, 
// curl -X POST -d "message" http://localhost:5142/cheep
app.MapPost("/cheep", async (HttpContext context) => {
    //var message = context.Request.Body.ToString();

    using (StreamReader reader = new StreamReader(context.Request.Body)) //fetching what is written in the terminal.
    {
        string message = await reader.ReadToEndAsync(); //putting that into a string an doing something asynchronously.

        var cheep = new Cheep(username, message, timestamp); // shoving it into our record.
        database_cheeps.Store(cheep); //storing the record in our database.
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
