using Azure.Core.Serialization;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Serialization;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

builder.Services.Configure<WorkerOptions>(workerOptions =>
{
    var settings = NewtonsoftJsonObjectSerializer.CreateJsonSerializerSettings();
    settings.ContractResolver = new CamelCasePropertyNamesContractResolver();
    workerOptions.Serializer = new NewtonsoftJsonObjectSerializer(settings);
});
    
builder.Services.AddApplicationInsightsTelemetryWorkerService()
    .ConfigureFunctionsApplicationInsights();

builder.Build().Run();