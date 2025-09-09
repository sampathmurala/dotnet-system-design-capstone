using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);
//builder.WebHost.UseUrls("http://localhost:5000");

// Add services to the container.
builder.Services.AddControllers();

//Add CORS policy to allow requests from specific origins
builder.Services.AddCors(options =>
{
    options.AddPolicy("DevelopmentCorsPolicy",
        policy =>
        {
            policy.SetIsOriginAllowed(origin => true) // Allows any origin
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Add authentication and authorization services
// Configure the default authentication scheme
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.Authority = builder.Configuration["Keycloak:Authority"];
    options.Audience = builder.Configuration["Keycloak:ClientId"];
    options.RequireHttpsMetadata = builder.Environment.IsProduction();  // Set to true in production
    options.SaveToken = true;
});


builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// Ensure the application uses HTTPS redirection
app.UseHttpsRedirection();

// Use the CORS middleware. This MUST be before UseRouting and UseAuthorization.
app.UseCors("DevelopmentCorsPolicy");

// Use routing to match incoming requests to endpoints
app.UseRouting();

// Add the Authentication middleware. This must be before UseAuthorization.
app.UseAuthentication();
app.UseAuthorization();

// Map controllers to endpoints
app.MapControllers();

//var summaries = new[]
//{
//    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
//};

//app.MapGet("/weatherforecast", () =>
//{
//    var forecast = Enumerable.Range(1, 5).Select(index =>
//        new WeatherForecast
//        (
//            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
//            Random.Shared.Next(-20, 55),
//            summaries[Random.Shared.Next(summaries.Length)]
//        ))
//        .ToArray();
//    return forecast;
//})
//.WithName("GetWeatherForecast")
//.RequireAuthorization(); //// Ensures the endpoint requires authentication

//app.MapGet("/public", () => "This is a public endpoint.")
//    .WithName("GetPublic");

//Run the application
app.Run();

//record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
//{
//    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
//}
