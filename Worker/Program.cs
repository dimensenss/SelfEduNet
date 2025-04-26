using OpenAI;
using Worker.Extensions;
using StackExchange.Redis;
using Worker.Extensions;
using Worker.Services;
using Microsoft.AspNetCore.Builder;

var builder = Host.CreateApplicationBuilder(args);

var environment = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "Production";
builder.Configuration
	.SetBasePath(Directory.GetCurrentDirectory())
	.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
	.AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true)
	.AddEnvironmentVariables();

builder.Configuration.AddEnvironmentVariables();

builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
	ConnectionMultiplexer.Connect(builder.Configuration.GetConnectionString("RedisConnection")));
builder.Services.AddOptionsInjection(builder.Configuration);

builder.Services.AddScoped<IOpenAIClientFactory, OpenAiClientFactory>();
builder.Services.AddScoped<IFileService, FileService>();
builder.Services.AddScoped<IVideoProcessorService, VideoProcessorService>();
builder.Services.AddScoped<IContextProcessorService, ContextProcessorService>();

builder.Services.AddHostedService<Worker.TranscriptionWorker>();
builder.Services.AddHostedService<Worker.ResumeWorker>();

var host = builder.Build();
await host.RunAsync();
