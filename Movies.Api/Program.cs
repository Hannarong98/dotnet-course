using Movies.Api.Mapping;
using Movies.Application.Database;
using Movies.Application.Extensions;


var builder = WebApplication.CreateBuilder(args);

var config = builder.Configuration;

builder.Services.AddAuth(config);

builder.Services.AddOpenApiWithSecuritySchemes();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddApplication();
builder.Services.AddDatabase(config);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
  app.MapOpenApiSpec(config);
}

app.UseHttpsRedirection();


app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<ValidationMappingMiddleware>();
app.MapControllers();

var dbInitializer = app.Services.GetRequiredService<DbInitializer>();
await dbInitializer.InitializeAsync();

app.Run();

