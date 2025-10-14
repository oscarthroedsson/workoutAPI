using DotNetEnv;
using workoutAPI.Extensions;
using workoutAPI.Middlewear;
using EasyCaching.InMemory;
using workoutAPI.Service;
using workoutAPI.Service.Cache;

Env.Load();
var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddEnvironmentVariables();
builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<PointCalculatorService>();
builder.Services.AddControllers(); // add controllers
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.DefaultIgnoreCondition =
            System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
    });

builder.Services.AddEasyCaching(options =>
{
    options.UseInMemory(inMemoryOptions =>
    {
        // InMemoryOptions.DBConfig är typen InMemoryCachingOptions
        inMemoryOptions.DBConfig = new InMemoryCachingOptions
        {
            ExpirationScanFrequency = 60, // städa var 60 sekunder
            SizeLimit = 10000,
            EnableReadDeepClone = true,
            EnableWriteDeepClone = false
        };

        // Provider-level settings
        inMemoryOptions.MaxRdSecond = 0;
        inMemoryOptions.EnableLogging = true;
    }, "default"); 
});


builder.Services.AddSingleton<ApiKeyService>(); 
builder.Services.AddSingleton<StaticDataCacheService>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<HeaderManager>();
builder.Services.AddSingleton<QuotaService>();
builder.Services.AddSingleton<BillingService>();
builder.Services.AddSingleton<CacheManager>();
builder.Services.AddSingleton<RateLimiterCache>();



await builder.AddSupabaseAsync();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
var app = builder.Build();



app.UseWhen(
    context => context.Request.Path.StartsWithSegments("/api/exercise"),
    appBuilder => appBuilder.UseExerciseMiddleware());
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) app.MapOpenApi();




app.MapControllers();
app.UseHttpsRedirection();
app.Run();





   




