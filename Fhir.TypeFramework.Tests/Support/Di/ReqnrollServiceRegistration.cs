using Fhir.TypeFramework.Tests.Application.BehaviorSuites;
using Fhir.TypeFramework.Tests.Application.Contracts;
using Reqnroll;
using Reqnroll.BoDi;

namespace Fhir.TypeFramework.Tests.Support.Di;

/// <summary>
/// 將行為套件註冊到 Reqnroll 情境容器（依賴反轉：步驟定義僅依賴介面）。
/// </summary>
[Binding]
public sealed class ReqnrollServiceRegistration
{
    [BeforeScenario(Order = int.MinValue)]
    public static void RegisterServices(IObjectContainer container)
    {
        container.RegisterTypeAs<ValidationBehaviorSuite, IValidationBehaviorSuite>(null);
        container.RegisterTypeAs<SerializationBehaviorSuite, ISerializationBehaviorSuite>(null);
        container.RegisterTypeAs<PrimitiveAndExtensionBehaviorSuite, IPrimitiveAndExtensionBehaviorSuite>(null);
        container.RegisterTypeAs<PerformanceBehaviorSuite, IPerformanceBehaviorSuite>(null);
        container.RegisterTypeAs<BackboneAndHierarchyBehaviorSuite, IBackboneAndHierarchyBehaviorSuite>(null);
        container.RegisterTypeAs<CoverageReflectionBehaviorSuite, ICoverageReflectionBehaviorSuite>(null);
        container.RegisterTypeAs<ComplexTypeJsonBehaviorSuite, IComplexTypeJsonBehaviorSuite>(null);
    }
}
