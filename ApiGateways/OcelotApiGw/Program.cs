using Ocelot.DependencyInjection;
using Ocelot.Cache.CacheManager;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

builder.Services.AddOcelot();
builder.Services.AddOcelot().AddCacheManager(settings => settings.WithDictionaryHandle());

builder.Configuration.AddJsonFile($"ocelot.{app.Environment.EnvironmentName}json",true,true);


app.MapGet("/", () => "Hello World!");

app.Run();

