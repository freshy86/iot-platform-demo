using IotPlatformDemo.Domain;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Azure.Devices;
using Microsoft.Identity.Web;
using Microsoft.OpenApi;

var apiTitle = "IoT Demo API";
const string apiVersion = "v1";

var builder = WebApplication.CreateBuilder(args);
var env = builder.Environment;
var configuration = builder.Configuration;
var isDevelopment = env.IsDevelopment();

if (isDevelopment)
    apiTitle += " (Development)";

builder.Services.AddSingleton(RegistryManager.CreateFromConnectionString(configuration.GetValue<string>("iothub:connectionString")));

builder.Services.AddControllers();

builder.Services.AddRouting(options => options.LowercaseUrls = true);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddMicrosoftIdentityWebApi(builder.Configuration);

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc(apiVersion, new ()
    {
        Title = apiTitle,
        Version = apiVersion,
        Description = ""
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
// Serve the Swagger UI at the app root ("/") by setting RoutePrefix to empty.
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint($"/swagger/{apiVersion}/swagger.json", $"{apiTitle} {apiVersion}");
    options.RoutePrefix = string.Empty; // serve UI at application root
});

app.UseHttpsRedirection();

app.UseAuthorization();

//if (isDevelopment)
//    app.MapControllers().WithMetadata(new AllowAnonymousAttribute());
//else
    app.MapControllers();

app.Run();
