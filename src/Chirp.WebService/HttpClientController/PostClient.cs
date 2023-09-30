using System.Web;
using System.Diagnostics;
using SimpleDB;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PostClient;

public class PostClientRequest 
{
    private readonly HttpClient _client;
    private readonly string _port;
    //private ILogger<ChirpClient> _logger;
    private readonly Stopwatch _stopwatch = new();

    public PostClientRequest(HttpClient client /*ILogger<ChirpClient> logger*/)
    {
        _client = client;
        //_logger = logger;
        _port   = $"http://localhost:{3000}";
    }

    // Get Single Cheep:
    public async Task<IResult> PostCheep(HttpContext context, IDatabaseRepository<Cheep> database)
    {
        try
        {
            //_logger.LogInformation("Getting Cheeps");
            _stopwatch.Start();

            var username = Environment.UserName;
            var timestamp = DateTime.Now;
            long unixTime = ((DateTimeOffset)timestamp).ToUnixTimeSeconds();

            using (StreamReader reader = new StreamReader(context.Request.Body)) //fetching what is written in the terminal.
            {
                string message = await reader.ReadToEndAsync(); //putting that into a string an doing something asynchronously.

                var cheep = new Cheep(username, message, unixTime); // shoving it into our record.
                database.Store(cheep); //storing the record in our database.
            }

            _stopwatch.Stop();
            var elapsedTime = _stopwatch.ElapsedMilliseconds;

            //_logger.LogInformation($"Request completed {elapsedTime} ms");
            _stopwatch.Reset();

            return Results.StatusCode(201);
        }
        catch(Exception e)
        {
            Console.WriteLine(e.Message);
            throw;
        }
    }
}