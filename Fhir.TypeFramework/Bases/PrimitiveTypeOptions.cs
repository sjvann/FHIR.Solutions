namespace Fhir.TypeFramework.Bases;

/// <summary>
/// 控制將 FHIR 字串指派至 <see cref="PrimitiveType{T}.StringValue"/> 時，是否立即解析為強型別 <see cref="PrimitiveType{T}.Value"/>。
/// </summary>
public enum PrimitiveTypedParseTiming
{
    /// <summary>
    /// 每次設定字串時立即執行 <c>ParseTypedValue</c>（與早期行為一致）。
    /// </summary>
    Eager,

    /// <summary>
    /// 僅保存字串；首次讀取 <see cref="PrimitiveType{T}.Value"/>、或呼叫 <see cref="PrimitiveType{T}.EnsureTypedValueParsed"/> 時才解析。
    /// 完整規則仍以 <see cref="PrimitiveType{T}.Validate(System.ComponentModel.DataAnnotations.ValidationContext)"/> 為準。
    /// </summary>
    Deferred,
}

/// <summary>
/// Primitive 型別的全域選項（字串→強型別解析時機、JSON 寫出前是否強制驗證 value）。
/// </summary>
public static class PrimitiveTypeOptions
{
    /// <summary>
    /// 預設為延遲解析，以降低大量指派 <see cref="PrimitiveType{T}.StringValue"/> 時的重複解析成本。
    /// </summary>
    public static PrimitiveTypedParseTiming TypedParseTiming { get; set; } = PrimitiveTypedParseTiming.Deferred;

    /// <summary>
    /// 若為 true，<see cref="PrimitiveType{T}.ToJsonValue"/> 與 <see cref="PrimitiveType{T}.ToFullJsonObject"/> 在寫出 value 前
    /// 會執行與 <see cref="PrimitiveType{T}.Validate(System.ComponentModel.DataAnnotations.ValidationContext)"/> 相同的 value 檢查；
    /// 不符時擲回 <see cref="InvalidOperationException"/>。
    /// </summary>
    public static bool ValidateBeforeJsonWrite { get; set; }
}
