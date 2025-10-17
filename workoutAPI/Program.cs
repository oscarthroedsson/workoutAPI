using DotNetEnv;
using workoutAPI.Extensions;
using workoutAPI.Middlewear;
using EasyCaching.InMemory;
using workoutAPI.Service;
using workoutAPI.Service.Cache;
using workoutAPI.Services;

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
builder.Services.AddCustomCaching();



builder.Services.AddSingleton<ApiKeyService>(); 
builder.Services.AddSingleton<StaticDataCacheService>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<HeaderManager>();
builder.Services.AddSingleton<QuotaService>();
builder.Services.AddSingleton<BillingService>();
builder.Services.AddSingleton<CacheManager>();
builder.Services.AddSingleton<RateLimiterCache>();


// Stripe
builder.Services.AddSingleton<StripeService>();
builder.Services.AddSingleton<SubscriptionWebhookHandler>();

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





   




