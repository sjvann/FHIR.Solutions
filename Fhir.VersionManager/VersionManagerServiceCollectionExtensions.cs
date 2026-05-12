using Fhir.VersionManager.Capability;
using Microsoft.Extensions.DependencyInjection;

namespace Fhir.VersionManager;

public static class VersionManagerServiceCollectionExtensions
{
    public static IServiceCollection AddFhirVersionManager(this IServiceCollection services)
    {
        services.AddSingleton<IFhirCapabilityRuntime, FhirCapabilityRuntime>();
        return services;
    }
}
