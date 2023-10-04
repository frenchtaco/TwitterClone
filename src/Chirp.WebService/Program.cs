// Local Imports:
using SimpleDB;
using GetClient;
using PostClient;

// Nuget & Microsoft Imports:
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;

/*
    @Key Information:
        # Curl Commands:
            - POST Request [curl -X POST -d "message" http://localhost:3000/cheep]
            - GET Request  [curl -v http://localhost:3000/cheeps]
        # 
*/

// Global Variable:
var port = Environment.GetEnvironmentVariable("PORT") ?? "3000";
var builder = WebApplication.CreateBuilder(args);
IDatabaseRepository<Cheep> database_cheeps = (CSVDatabase<Cheep>)Activator.CreateInstance(typeof(CSVDatabase<Cheep>), nonPublic: true);//because something
IServerAddressesFeature addressFeature = null;

// Dependency Injection of Middleware:
builder.Services.AddControllers();
builder.Services.AddHttpClient<GetRequestClient>(client => {
    client.BaseAddress = new Uri($"https://bdsagroup6chirpremotedb.azurewebsites.net/");
});
builder.Services.AddHttpClient<PostClientRequest>(client => {
    client.BaseAddress = new Uri($"https://bdsagroup6chirpremotedb.azurewebsites.net/");
});


// App Build and Configuration:
var app = builder.Build();
app.UseHttpsRedirection();

/* ENDPOINTS */
// Todo: Add MapGroups - [Private and Public]
// 00. 
app.MapGet("/", () => $"Hi there, Kestrel is running on\n\n{string.Join("\n", addressFeature.Addresses.ToArray() )}" );


// 01. Post Request
app.MapPost("/cheep", async (PostClientRequest postClient, HttpContext context) => {
    var response = postClient.PostCheep(context, database_cheeps);
    return response;
});

app.MapGet("/cheeps", (GetRequestClient getClient) => {
    var response = getClient.GetAllCheeps(database_cheeps);
    return response;
});

app.MapGet("/userCheeps", (GetRequestClient getClient) => {
    var response = getClient.GetAllUserCheeps(database_cheeps);
    return response;
});

/* INITIATE BUILD */ 
// @Original Author :: [https://nodogmablog.bryanhogan.net/2022/01/programmatically-determine-what-ports-kestrel-is-running-on/]
app.Start();

var server = app.Services.GetService<IServer>();
addressFeature = server.Features.Get<IServerAddressesFeature>();

foreach (var address in addressFeature.Addresses)
{
    Console.WriteLine("Server is listening on address: " + address);
}

app.WaitForShutdown();