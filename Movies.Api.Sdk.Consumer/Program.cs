using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Movies.Api.Sdk;
using Movies.Api.Sdk.Consumer;
using Movies.Contracts.Requests;
using Refit;

var builder = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddUserSecrets<Program>(); 

IConfiguration configuration = builder.Build();

var services = new ServiceCollection();

services
    .AddSingleton(configuration)
    .AddSingleton<TokenProvider>()
    .AddTransient<AuthHeaderHandler>()
    .AddRefitClient<IMoviesApi>()
    .ConfigureHttpClient(client =>
    {
        client.BaseAddress = new Uri(configuration["MoviesAPI:BaseURL"]!);
    })
    .AddHttpMessageHandler<AuthHeaderHandler>();
    
var provider = services.BuildServiceProvider();

var moviesApi = provider.GetRequiredService<IMoviesApi>();


var movieToCreate = new CreateMovieRequest
{
    Genres = ["Test test"],
    Title = "Test test",
    YearOfRelease = 2019
};

var created = await moviesApi.CreateMovieAsync(movieToCreate);

Console.WriteLine(JsonSerializer.Serialize(created));