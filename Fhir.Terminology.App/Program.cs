using Fhir.Terminology.App.Hosting;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddTerminologyApplicationServices(builder.Configuration);

var app = builder.Build();

await TerminologyDatabaseInitializer.RunMigrationsAndSeedAsync(app);

app.UseTerminologyOpenApiReference();
app.MapTerminologyApplicationPipeline();

app.Run();

public partial class Program { }
