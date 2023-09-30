using System.Web;
using System.Diagnostics;
using SimpleDB;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GetClient;

public class GetRequestClient 
{
    private readonly HttpClient _client;
    private readonly string _port;
    //private ILogger<ChirpClient> _logger;
    private readonly Stopwatch _stopwatch = new();

    public GetRequestClient(HttpClient client /*ILogger<ChirpClient> logger*/)
    {
        _client = client;
        //_logger = logger;
        _port   = $"http://localhost:{3000}";
    }

    // Get Single Cheep:
    public async Task<string> GetAllCheeps(IDatabaseRepository<Cheep> database)
    {
        try
        {
            //_logger.LogInformation("Getting Cheeps");
            _stopwatch.Start();

            IEnumerable<Cheep> records = database.Read();
            
            _stopwatch.Stop();
            var elapsedTime = _stopwatch.ElapsedMilliseconds;

            //_logger.LogInformation($"Request completed {elapsedTime} ms");
            _stopwatch.Reset();
            return JsonSerializer.Serialize(records);
        }
        catch(Exception e)
        {
            Console.WriteLine(e.Message);
            throw;
        }
    }

    public async Task<string> GetAllUserCheeps(IDatabaseRepository<Cheep> database)
    {
        try 
        {
            _stopwatch.Start();

            IEnumerable<Cheep> records = database.Read();
            List<Cheep> retList = new List<Cheep>();

            foreach(Cheep record in records) 
            {
                if(record.Author == Environment.UserName) 
                {
                    retList.Add(record);
                }
            }

            _stopwatch.Stop();
            var elapsedTime = _stopwatch.ElapsedMilliseconds;
            Console.WriteLine(_stopwatch.ElapsedMilliseconds);
            _stopwatch.Reset();

            return JsonSerializer.Serialize(retList);
        }
        catch(Exception e)
        {
            Console.WriteLine(e.Message);
            throw;
        }
    }
}