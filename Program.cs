using System.Reflection;
using System.Reflection.Emit;
using Elastic.Apm.NetCoreAll;
using Serilog;
using Serilog.Exceptions;
using Serilog.Sinks.Elasticsearch;

var builder = WebApplication.CreateBuilder(args);




ConfigureLogging();
builder.Host.UseSerilog();

// Add services to the container.
builder.Services.AddControllersWithViews();
Environment.GetEnvironmentVariable("ORTAM");

if (Environment.GetEnvironmentVariable("ORTAM") == "dev")
{
    builder.Configuration.SetBasePath(Environment.CurrentDirectory + "/appconf")
    .AddJsonFile("appsettings.dev.json",optional:true, reloadOnChange:true);
    
}
else
{
    builder.Configuration.SetBasePath(Environment.CurrentDirectory + "/appconf")
    .AddJsonFile("appsettings.json",optional:true, reloadOnChange:true);
}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}





app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAllElasticApm(builder.Configuration);
app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();



void ConfigureLogging()
{
    var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
    var configuration = new ConfigurationBuilder()
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .AddJsonFile(
            $"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json",
            optional: true)
        .Build();

    Log.Logger = new LoggerConfiguration()
        .Enrich.FromLogContext()
        .Enrich.WithExceptionDetails()
        .WriteTo.Debug()
        .WriteTo.Console()
        .WriteTo.Elasticsearch(ConfigureElasticSink(configuration, environment))
        .Enrich.WithProperty("Environment", environment)
        .ReadFrom.Configuration(configuration)
        .CreateLogger();
}

ElasticsearchSinkOptions ConfigureElasticSink(IConfigurationRoot configuration, string environment)
{
    return new ElasticsearchSinkOptions(new Uri(configuration["ElasticConfiguration:Uri"]))
    {
        AutoRegisterTemplate = true,
        ModifyConnectionSettings = x => x.ServerCertificateValidationCallback((o, certificate, chain, errors) => true),
        IndexFormat = "demos-{0:yyyy.MM.dd}",
    };
}