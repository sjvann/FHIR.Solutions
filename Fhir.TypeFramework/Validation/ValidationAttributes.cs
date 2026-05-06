using System;
using System.ComponentModel.DataAnnotations;

namespace Fhir.TypeFramework.Validation;

// 目前僅提供擴展點（README 中有規劃）；後續可依需要補齊更完整的 DataAnnotations 整合。

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
public sealed class FhirRequiredAttribute : RequiredAttribute
{
}

