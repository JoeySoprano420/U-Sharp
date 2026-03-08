using System.Text.Json;
using System.Text.Json.Serialization;
using USharp.Core.Exceptions;

namespace USharp.Serialization;

/// <summary>
/// Provides JSON serialization and deserialization for U# programs.
/// </summary>
public static class USharpJson
{
    private static readonly JsonSerializerOptions _defaultOptions = new()
    {
        WriteIndented = false,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    };

    private static readonly JsonSerializerOptions _prettyOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    };

    /// <summary>Serializes <paramref name="value"/> to a JSON string.</summary>
    public static string Serialize<T>(T value, bool pretty = false)
    {
        try
        {
            return JsonSerializer.Serialize(value, pretty ? _prettyOptions : _defaultOptions);
        }
        catch (JsonException ex)
        {
            throw new USharpException("JSON serialization failed.", ex);
        }
    }

    /// <summary>
    /// Deserializes a JSON string to <typeparamref name="T"/>.
    /// </summary>
    public static T Deserialize<T>(string json)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(json);
        try
        {
            var result = JsonSerializer.Deserialize<T>(json, _defaultOptions);
            if (result is null)
                throw new USharpException(
                    $"JSON deserialization produced null for type '{typeof(T).Name}'.");
            return result;
        }
        catch (JsonException ex)
        {
            throw new USharpException("JSON deserialization failed.", ex);
        }
    }

    /// <summary>
    /// Attempts to deserialize a JSON string, returning <see langword="null"/> on failure.
    /// </summary>
    public static T? TryDeserialize<T>(string json) where T : class
    {
        if (string.IsNullOrWhiteSpace(json)) return null;
        try
        {
            return JsonSerializer.Deserialize<T>(json, _defaultOptions);
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Deep-clones an object by round-tripping it through JSON.
    /// </summary>
    public static T Clone<T>(T value)
    {
        var json = Serialize(value);
        return Deserialize<T>(json);
    }

    /// <summary>Checks whether a string is valid JSON.</summary>
    public static bool IsValidJson(string json)
    {
        if (string.IsNullOrWhiteSpace(json)) return false;
        try
        {
            using var doc = JsonDocument.Parse(json);
            return true;
        }
        catch (JsonException)
        {
            return false;
        }
    }
}

/// <summary>
/// Provides binary serialization using <see cref="System.Runtime.Serialization"/>.
/// </summary>
public static class USharpBinary
{
    /// <summary>Serializes <paramref name="value"/> to a byte array using JSON encoding.</summary>
    public static byte[] Serialize<T>(T value)
    {
        var json = USharpJson.Serialize(value);
        return System.Text.Encoding.UTF8.GetBytes(json);
    }

    /// <summary>Deserializes a byte array back to <typeparamref name="T"/>.</summary>
    public static T Deserialize<T>(byte[] data)
    {
        ArgumentNullException.ThrowIfNull(data);
        var json = System.Text.Encoding.UTF8.GetString(data);
        return USharpJson.Deserialize<T>(json);
    }

    /// <summary>Encodes <paramref name="data"/> to a Base64 string.</summary>
    public static string ToBase64(byte[] data)
    {
        ArgumentNullException.ThrowIfNull(data);
        return Convert.ToBase64String(data);
    }

    /// <summary>Decodes a Base64 string to a byte array.</summary>
    public static byte[] FromBase64(string base64)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(base64);
        try
        {
            return Convert.FromBase64String(base64);
        }
        catch (FormatException ex)
        {
            throw new USharpException("Invalid Base64 string.", ex);
        }
    }
}
