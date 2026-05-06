namespace Fhir.TypeFramework.Interface;

/// <summary>
/// 定義具有值的基礎介面
/// </summary>
/// <typeparam name="T">值的型別</typeparam>
/// <remarks>
/// 這是所有值型別介面的基礎介面，提供統一的值存取方式。
/// 使用協變泛型參數 (out T) 以支援型別安全的向上轉型。
/// </remarks>
/// <example>
/// <code>
/// public class StringWrapper : IValue&lt;string&gt;
/// {
///     public string? Value { get; set; }
/// }
/// </code>
/// </example>
public interface IValue<out T>
{
    /// <summary>
    /// 取得值
    /// </summary>
    /// <value>
    /// 儲存的值，可能為 null
    /// </value>
    public abstract T? Value { get; }
}

/// <summary>
/// 定義可空值型別的介面
/// </summary>
/// <typeparam name="T">值型別，必須是結構</typeparam>
/// <remarks>
/// 繼承自 <see cref="IValue{T}"/>，專門用於處理可空的值型別。
/// 提供額外的 <see cref="HasValue"/> 屬性來檢查值是否存在。
/// </remarks>
/// <example>
/// <code>
/// public class NullableIntWrapper : INullableValue&lt;int&gt;
/// {
///     public int? Value { get; set; }
///     public bool HasValue =&gt; Value.HasValue;
/// }
/// </code>
/// </example>
/// <seealso cref="IValue{T}"/>
public interface INullableValue<T> : IValue<T?> where T : struct
{
    /// <summary>
    /// 取得值，指示是否有值
    /// </summary>
    /// <value>
    /// 如果有值則為 <c>true</c>，否則為 <c>false</c>
    /// </value>
    public abstract bool HasValue { get; }
}

/// <summary>
/// 定義字串值的介面
/// </summary>
/// <remarks>
/// 專門用於處理字串型別的值。字串是引用型別，因此不需要額外的 HasValue 屬性。
/// </remarks>
/// <example>
/// <code>
/// public class StringValue : IStringValue
/// {
///     public string? Value { get; set; }
/// }
/// </code>
/// </example>
/// <seealso cref="IValue{T}"/>
public interface IStringValue : IValue<string> { }

/// <summary>
/// 定義日期時間值的介面
/// </summary>
/// <remarks>
/// 用於處理 <see cref="DateTime"/> 型別的可空值。
/// 繼承自 <see cref="INullableValue{T}"/>，提供 HasValue 屬性。
/// </remarks>
/// <example>
/// <code>
/// public class DateTimeValue : IDateTimeValue
/// {
///     public DateTime? Value { get; set; }
///     public bool HasValue =&gt; Value.HasValue;
/// }
/// </code>
/// </example>
/// <seealso cref="INullableValue{T}"/>
/// <seealso cref="DateTime"/>
public interface IDateTimeValue : INullableValue<DateTime> { }

/// <summary>
/// 定義帶時區偏移的日期時間值的介面
/// </summary>
/// <remarks>
/// 用於處理 <see cref="DateTimeOffset"/> 型別的可空值。
/// 適用於需要時區資訊的日期時間場景。
/// </remarks>
/// <example>
/// <code>
/// public class DateTimeOffsetValue : IDatetimeOffsetValue
/// {
///     public DateTimeOffset? Value { get; set; }
///     public bool HasValue =&gt; Value.HasValue;
/// }
/// </code>
/// </example>
/// <seealso cref="INullableValue{T}"/>
/// <seealso cref="DateTimeOffset"/>
public interface IDatetimeOffsetValue : INullableValue<DateTimeOffset> { }

/// <summary>
/// 定義布林值的介面
/// </summary>
/// <remarks>
/// 用於處理 <see cref="bool"/> 型別的可空值。
/// 適用於三態邏輯場景（true、false、null）。
/// </remarks>
/// <example>
/// <code>
/// public class BooleanValue : IBooleanValue
/// {
///     public bool? Value { get; set; }
///     public bool HasValue =&gt; Value.HasValue;
/// }
/// </code>
/// </example>
/// <seealso cref="INullableValue{T}"/>
public interface IBooleanValue : INullableValue<bool> { }

/// <summary>
/// 定義位元組值的介面
/// </summary>
/// <remarks>
/// 用於處理 <see cref="byte"/> 型別的可空值。
/// 適用於 0-255 範圍的數值場景。
/// </remarks>
/// <example>
/// <code>
/// public class ByteValue : IByteValue
/// {
///     public byte? Value { get; set; }
///     public bool HasValue =&gt; Value.HasValue;
/// }
/// </code>
/// </example>
/// <seealso cref="INullableValue{T}"/>
public interface IByteValue : INullableValue<byte> { }

/// <summary>
/// 定義 32 位元整數值的介面
/// </summary>
/// <remarks>
/// 用於處理 <see cref="int"/> 型別的可空值。
/// 適用於 -2,147,483,648 到 2,147,483,647 範圍的整數場景。
/// </remarks>
/// <example>
/// <code>
/// public class Integer32Value : IInteger32Value
/// {
///     public int? Value { get; set; }
///     public bool HasValue =&gt; Value.HasValue;
/// }
/// </code>
/// </example>
/// <seealso cref="INullableValue{T}"/>
public interface IInteger32Value : INullableValue<int> { }

/// <summary>
/// 定義 64 位元整數值的介面
/// </summary>
/// <remarks>
/// 用於處理 <see cref="long"/> 型別的可空值。
/// 適用於 -9,223,372,036,854,775,808 到 9,223,372,036,854,775,807 範圍的整數場景。
/// </remarks>
/// <example>
/// <code>
/// public class Integer64Value : IInteger64Value
/// {
///     public long? Value { get; set; }
///     public bool HasValue =&gt; Value.HasValue;
/// }
/// </code>
/// </example>
/// <seealso cref="INullableValue{T}"/>
public interface IInteger64Value : INullableValue<long> { }

/// <summary>
/// 定義 32 位元無符號整數值的介面
/// </summary>
/// <remarks>
/// 用於處理 <see cref="uint"/> 型別的可空值。
/// 適用於 0 到 4,294,967,295 範圍的非負整數場景。
/// </remarks>
/// <example>
/// <code>
/// public class UnsignedInteger32Value : IUnsignedInteger32Value
/// {
///     public uint? Value { get; set; }
///     public bool HasValue =&gt; Value.HasValue;
/// }
/// </code>
/// </example>
/// <seealso cref="INullableValue{T}"/>
public interface IUnsignedInteger32Value : INullableValue<uint> { }

/// <summary>
/// 定義十進位數值的介面
/// </summary>
/// <remarks>
/// 用於處理 <see cref="decimal"/> 型別的可空值。
/// 適用於需要高精度小數運算的場景，如金融計算。
/// </remarks>
/// <example>
/// <code>
/// public class DecimalValue : IDecimalValue
/// {
///     public decimal? Value { get; set; }
///     public bool HasValue =&gt; Value.HasValue;
/// }
/// </code>
/// </example>
/// <seealso cref="INullableValue{T}"/>
public interface IDecimalValue : INullableValue<decimal> { }


