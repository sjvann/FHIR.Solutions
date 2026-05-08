using FhirResourceCreator.Configuration;
using FhirResourceCreator.Pipeline;
using Microsoft.Extensions.Configuration;

var config = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
    .Build();

var opts = config.GetSection(GeneratorOptions.SectionName).Get<GeneratorOptions>()
           ?? new GeneratorOptions();

if (opts.Mode == GeneratorInputMode.Registry)
    await GenerationOrchestrator.RunRegistryPackagesAsync(opts).ConfigureAwait(false);
else
    GenerationOrchestrator.RunExcelLegacy(opts);

Console.WriteLine("Fhir.ResourceCreator finished.");
