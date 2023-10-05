using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;
using CommandType;

namespace CommandHandle;
public class CommandHandler
{
    public HttpClient _client { get; set; }

    public CommandHandler()
    {
        _client = new HttpClient();
        _client.BaseAddress = new Uri("https://bdsagroup6chirpremotedb.azurewebsites.net/"); // Replace with your server's URL
        _client.DefaultRequestHeaders.Add("Accept", "application/json");
    }

    public async Task Fondle(bool shouldReadSpecific, bool shouldReadAll, string msg)
    {
        if (shouldReadAll)
        {
            try
            {
                await GetAllRequest(_client);
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"Error Message: {e.Message}");
            }
        } 
        else if(shouldReadSpecific) 
        {
            try
            {
                await GetUserCheepsRequest(_client);
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"Error Message: {e.Message}");
            }
        }
        else if (msg != null)
        {
            try
            {
                await PostRequest(msg, _client);
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"Error Message: {e.Message}");
            }
        }
        else
        {
            Console.WriteLine("System.CommandLine input error");
        }
    }

    public async Task GetAllRequest(HttpClient client) 
    {
        HttpResponseMessage response = await client.GetAsync("/cheeps");

        // Log status code...
        if(response.IsSuccessStatusCode) 
        //Console.WriteLine(response.IsSuccessStatusCode);
        {
            string responseBody = await response.Content.ReadAsStringAsync();
            var records = JsonSerializer.Deserialize<IEnumerable<Cheep>>(responseBody);

            if(records != null)
            {
                foreach (var record in records)
                {
                    
                    Console.WriteLine($"Author: {record.Author}, Message: {record.Message}, Date: {DateTimeOffset.FromUnixTimeSeconds(record.Timestamp).LocalDateTime}");
                }
            }
        }
    }

    public async Task GetUserCheepsRequest(HttpClient client) 
    {
        HttpResponseMessage response = await client.GetAsync("/userCheeps");

        // Log status code...
        if(response.IsSuccessStatusCode) 
        {
            string responseBody = await response.Content.ReadAsStringAsync();
            var records = JsonSerializer.Deserialize<IEnumerable<Cheep>>(responseBody);

            if(records != null)
            {
                foreach (var record in records)
                {
                    Console.WriteLine($"Author: {record.Author}, Message: {record.Message}, Date: {DateTimeOffset.FromUnixTimeSeconds(record.Timestamp).LocalDateTime}");
                }
            }
        }
    }

    static async Task PostRequest(string msg, HttpClient client)
    {
        StringContent stringContent = new StringContent(msg);
        HttpResponseMessage response = await client.PostAsync("/cheep", stringContent);
        
        // Log status code...
        int statusCode = (int) response.StatusCode;

        if(response.IsSuccessStatusCode)
        {
            Console.WriteLine($"POST Request was successful with status code: {statusCode}");
        } 
        else 
        {
            Console.WriteLine($"POST Request was unsuccessful with status code: {statusCode}");
        }
    }
}