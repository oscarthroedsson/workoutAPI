using DotNetEnv;
using workoutAPI.Extensions;
using workoutAPI.Middlewear;
using workoutAPI.Service;

Env.Load();
var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddEnvironmentVariables(); // make it possible to get env variables
builder.Services.AddControllers(); // add controllers
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.DefaultIgnoreCondition =
            System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
    });

await builder.AddSupabaseAsync();



// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
var app = builder.Build();

app.UseWhen(
    context => context.Request.Path.StartsWithSegments("/api/exercise"),
    appBuilder => appBuilder.UseExerciseMiddleware());





// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}


app.MapControllers();
app.UseHttpsRedirection();
app.Run();





   




