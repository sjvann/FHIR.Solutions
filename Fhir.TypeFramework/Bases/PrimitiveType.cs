using Fhir.TypeFramework.Abstractions;
using Fhir.TypeFramework.DataTypes;
// using Fhir.TypeFramework.DataTypes.ComplexTypes; // 暫時註解，ComplexTypes 目錄已刪除
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
namespace Fhir.TypeFramework.Bases;

/// <summary>
/// FHIR R5 PrimitiveType（抽象，非泛型層）— 對應規格中 <c>DataType &lt;| PrimitiveType</c>。
/// 所有具體 primitive 皆透過 <see cref="PrimitiveType{T}"/> 繼承此類別。
/// </summary>
public abstract class PrimitiveType : DataType;

/// <summary>
/// FHIR Primitive Type 泛型基礎類別
/// 提供所有 FHIR 基本型別的通用功能
/// </summary>
/// <remarks>
/// FHIR R5 PrimitiveType (Abstract)
/// Base definition for all primitive types in FHIR.
/// PrimitiveType 繼承自 DataType，因此具有 id 和 extension 屬性
/// </remarks>
public abstract class PrimitiveType<T> : PrimitiveType
{
    /// <summary>
    /// 原始值（字串形式）
    /// </summary>
    protected string? _stringValue;

    /// <summary>
    /// 強型別值
    /// </summary>
    protected T? _typedValue;

    /// <summary>
    /// 當 <see cref="PrimitiveTypeOptions.TypedParseTiming"/> 為 <see cref="PrimitiveTypedParseTiming.Deferred"/> 時，
    /// 表示字串自上次與強型別同步後已變更，需延遲至讀取 <see cref="Value"/> 或呼叫 <see cref="EnsureTypedValueParsed"/> 再解析。
    /// </summary>
    private bool _typedValueStale;

    /// <summary>
    /// 字串值
    /// </summary>
    [JsonPropertyName("value")]
    public string? StringValue
    {
        get => _stringValue;
        set
        {
            var normalized = NormalizeStringInput(value);
            _stringValue = normalized;
            if (PrimitiveTypeOptions.TypedParseTiming == PrimitiveTypedParseTiming.Eager)
            {
                _typedValue = ParseTypedValue(normalized);
                _typedValueStale = false;
            }
            else
            {
                _typedValueStale = true;
            }

            OnStringValueChanged(normalized);
        }
    }

    /// <summary>
    /// 強型別值
    /// </summary>
    [JsonIgnore]
    public T? Value
    {
        get
        {
            SyncTypedValueFromStringIfNeeded();
            return _typedValue;
        }
        set
        {
            _typedValue = value;
            _stringValue = ConvertToStringValue(value);
            _typedValueStale = false;
        }
    }

    /// <summary>
    /// 檢查是否為 NULL 值
    /// </summary>
    [JsonIgnore]
    public bool IsNull => string.IsNullOrEmpty(_stringValue);

    /// <summary>
    /// 取得 FHIR 型別名稱
    /// </summary>
    public override string TypeName => GetType().Name;

    /// <summary>
    /// 當字串值改變時的回調
    /// </summary>
    /// <param name="value">新的字串值</param>
    protected virtual void OnStringValueChanged(string? value)
    {
        // 子類別可以覆寫此方法來處理值變更
    }

    /// <summary>
    /// 在指派 <see cref="StringValue"/> 前對輸入字串做輕量正規化（預設不變更）。
    /// 完整 FHIR 規則仍在 <see cref="Validate(ValidationContext)"/> 與型別專屬驗證中處理。
    /// </summary>
    protected virtual string? NormalizeStringInput(string? value) => value;

    /// <summary>
    /// 若目前為延遲解析模式且強型別尚未與字串同步，則立即執行 <see cref="ParseTypedValue"/>。
    /// </summary>
    public void EnsureTypedValueParsed() => SyncTypedValueFromStringIfNeeded();

    private void SyncTypedValueFromStringIfNeeded()
    {
        if (PrimitiveTypeOptions.TypedParseTiming != PrimitiveTypedParseTiming.Deferred)
            return;
        if (!_typedValueStale)
            return;
        _typedValue = ParseTypedValue(_stringValue);
        _typedValueStale = false;
    }

    private void ThrowIfPrimitiveValueInvalidForJsonWrite()
    {
        if (!PrimitiveTypeOptions.ValidateBeforeJsonWrite)
            return;
        if (_stringValue == null)
            return;
        var parsed = ParseValue(_stringValue);
        if (!IsValidValue(parsed))
            throw new InvalidOperationException($"Invalid value for {GetType().Name}: {_stringValue}");
    }

    /// <summary>
    /// 從字串解析值
    /// </summary>
    /// <param name="value">要解析的字串</param>
    /// <returns>解析後的值</returns>
    public abstract object? ParseValue(string value);

    /// <summary>
    /// 將值轉換為字串
    /// </summary>
    /// <param name="value">要轉換的值</param>
    /// <returns>值的字串表示</returns>
    public abstract string? ValueToString(object? value);

    /// <summary>
    /// 驗證值是否符合 FHIR 規範
    /// </summary>
    /// <param name="value">要驗證的值</param>
    /// <returns>如果值符合規範則為 true，否則為 false</returns>
    public abstract bool IsValidValue(object? value);

    /// <summary>
    /// 從字串解析為強型別值
    /// </summary>
    /// <param name="value">要解析的字串</param>
    /// <returns>解析後的強型別值</returns>
    protected abstract T? ParseTypedValue(string? value);

    /// <summary>
    /// 將強型別值轉換為字串
    /// </summary>
    /// <param name="value">要轉換的強型別值</param>
    /// <returns>字串表示</returns>
    protected abstract string? ConvertToStringValue(T? value);

    /// <summary>
    /// 轉換為 JSON 值（簡化表示）
    /// </summary>
    /// <returns>JSON 值</returns>
    public virtual JsonValue? ToJsonValue()
    {
        ThrowIfPrimitiveValueInvalidForJsonWrite();
        return JsonValue.Create(_stringValue);
    }

    /// <summary>
    /// 從 JSON 值建立（簡化表示）
    /// </summary>
    /// <param name="jsonValue">要解析的 JSON 值</param>
    public virtual void FromJsonValue(JsonValue? jsonValue)
    {
        StringValue = jsonValue?.GetValue<string>();
    }

    /// <summary>
    /// 轉換為完整 JSON 物件（包含 Extension）
    /// </summary>
    /// <returns>完整的 JSON 物件</returns>
    public virtual JsonObject? ToFullJsonObject()
    {
        ThrowIfPrimitiveValueInvalidForJsonWrite();

        var jsonObject = new JsonObject();
        
        if (!string.IsNullOrEmpty(_stringValue))
        {
            jsonObject["value"] = JsonValue.Create(_stringValue);
        }

        // 添加擴展資訊
        if (Id != null || (Extension != null && Extension.Count > 0))
        {
            var elementObject = new JsonObject();
            
            if (Id != null)
            {
                elementObject["id"] = JsonValue.Create(Id.Value);
            }
            
            if (Extension != null && Extension.Count > 0)
            {
                var extensionArray = new JsonArray();
                foreach (var extension in Extension)
                {
                    if (extension is Element element)
                    {
                        extensionArray.Add(element.GetJsonNode());
                    }
                }
                elementObject["extension"] = extensionArray;
            }
            
            jsonObject["_" + GetType().Name.ToLowerInvariant()] = elementObject;
        }

        return jsonObject;
    }

    /// <summary>
    /// 從完整 JSON 物件建立（包含 Extension）
    /// </summary>
    /// <param name="jsonObject">要解析的完整 JSON 物件</param>
    public virtual void FromFullJsonObject(JsonObject? jsonObject)
    {
        if (jsonObject == null) return;

        // 解析主要值
        if (jsonObject.TryGetPropertyValue("value", out var valueNode))
        {
            StringValue = valueNode?.GetValue<string>();
        }

        // 解析擴展資訊
        var elementKey = "_" + GetType().Name.ToLowerInvariant();
        if (jsonObject.TryGetPropertyValue(elementKey, out var elementNode) && elementNode is JsonObject elementObject)
        {
            if (elementObject.TryGetPropertyValue("id", out var idNode) && idNode is JsonValue idJv)
            {
                Id = idJv.GetValue<string>();
            }

            if (elementObject.TryGetPropertyValue("extension", out var extensionNode) && extensionNode is JsonArray extensionArray)
            {
                Extension = new List<IExtension>();
                foreach (var extensionItem in extensionArray)
                {
                    if (extensionItem is JsonObject extensionObject)
                    {
                        var extension = new Extension(extensionObject);
                        Extension.Add(extension);
                    }
                }
            }
        }
    }

    /// <summary>
    /// 建立物件的深層複本
    /// </summary>
    /// <returns>PrimitiveType 的深層複本</returns>
    public override Base DeepCopy()
    {
        var copy = (PrimitiveType<T>)MemberwiseClone();
        copy._stringValue = _stringValue;
        copy._typedValue = _typedValue;
        copy._typedValueStale = _typedValueStale;
        copy.Id = Id?.DeepCopy() as FhirString;

        if (Extension != null)
        {
            copy.Extension = Extension.Select(ext => (ext.DeepCopy() as IExtension)!).ToList();
        }

        return copy;
    }

    /// <summary>
    /// 判斷與另一個物件是否相等
    /// </summary>
    /// <param name="other">要比較的物件</param>
    /// <returns>如果兩個物件相等則為 true，否則為 false</returns>
    public override bool IsExactly(Base other)
    {
        if (other is not PrimitiveType<T> otherPrimitive)
            return false;

        // Primitive 的規範表示以字串為準；延遲解析時 _typedValue 可能尚未同步，故不比對強型別。
        return base.IsExactly(other) &&
               _stringValue == otherPrimitive._stringValue;
    }

    /// <summary>
    /// 驗證 PrimitiveType 是否符合 FHIR 規範
    /// </summary>
    /// <param name="validationContext">驗證上下文</param>
    /// <returns>驗證結果集合</returns>
    public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        // 呼叫基礎驗證
        foreach (var result in base.Validate(validationContext))
        {
            yield return result;
        }

        // 驗證值（允許 null 代表未設定）
        if (_stringValue != null && !IsValidValue(ParseValue(_stringValue)))
        {
            yield return new ValidationResult($"Invalid value for {GetType().Name}: {_stringValue}");
        }
    }

    /// <summary>
    /// 轉換為字串表示
    /// </summary>
    /// <returns>字串表示</returns>
    public override string ToString() => _stringValue ?? string.Empty;
} 