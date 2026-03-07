using USharp.Core.Exceptions;

namespace USharp.Net.Http;

/// <summary>
/// A simple HTTP client wrapper for U# programs.
/// </summary>
public sealed class USharpHttpClient : IDisposable
{
    private readonly HttpClient _client;
    private bool _disposed;

    /// <summary>Creates a new HTTP client with default settings.</summary>
    public USharpHttpClient()
    {
        _client = new HttpClient();
    }

    /// <summary>Creates a new HTTP client with a specified base URL.</summary>
    public USharpHttpClient(string baseUrl)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(baseUrl);
        _client = new HttpClient { BaseAddress = new Uri(baseUrl) };
    }

    /// <summary>Sets a default request header.</summary>
    public USharpHttpClient WithHeader(string name, string value)
    {
        _client.DefaultRequestHeaders.TryAddWithoutValidation(name, value);
        return this;
    }

    /// <summary>Sets the request timeout.</summary>
    public USharpHttpClient WithTimeout(TimeSpan timeout)
    {
        _client.Timeout = timeout;
        return this;
    }

    /// <summary>Performs a GET request and returns the response body as a string.</summary>
    public async Task<string> GetStringAsync(string url,
        CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        ArgumentException.ThrowIfNullOrWhiteSpace(url);
        try
        {
            return await _client.GetStringAsync(url, cancellationToken)
                .ConfigureAwait(false);
        }
        catch (HttpRequestException ex)
        {
            throw new USharpException($"HTTP GET failed for '{url}'.", ex);
        }
    }

    /// <summary>Performs a GET request and returns the raw <see cref="HttpResponseMessage"/>.</summary>
    public async Task<HttpResponseMessage> GetAsync(string url,
        CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        ArgumentException.ThrowIfNullOrWhiteSpace(url);
        try
        {
            return await _client.GetAsync(url, cancellationToken).ConfigureAwait(false);
        }
        catch (HttpRequestException ex)
        {
            throw new USharpException($"HTTP GET failed for '{url}'.", ex);
        }
    }

    /// <summary>Performs a POST request with a JSON body and returns the response body.</summary>
    public async Task<string> PostJsonAsync(string url, string jsonBody,
        CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        ArgumentException.ThrowIfNullOrWhiteSpace(url);
        using var content = new StringContent(jsonBody,
            System.Text.Encoding.UTF8, "application/json");
        try
        {
            var response = await _client.PostAsync(url, content, cancellationToken)
                .ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync(cancellationToken)
                .ConfigureAwait(false);
        }
        catch (HttpRequestException ex)
        {
            throw new USharpException($"HTTP POST failed for '{url}'.", ex);
        }
    }

    /// <summary>Performs a DELETE request.</summary>
    public async Task DeleteAsync(string url,
        CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        ArgumentException.ThrowIfNullOrWhiteSpace(url);
        try
        {
            var response = await _client.DeleteAsync(url, cancellationToken)
                .ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
        }
        catch (HttpRequestException ex)
        {
            throw new USharpException($"HTTP DELETE failed for '{url}'.", ex);
        }
    }

    /// <inheritdoc />
    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        _client.Dispose();
    }
}

/// <summary>
/// Represents a structured HTTP response for use in U# programs.
/// </summary>
public sealed class USharpHttpResponse
{
    /// <summary>Gets the HTTP status code.</summary>
    public int StatusCode { get; }

    /// <summary>Gets the response body.</summary>
    public string Body { get; }

    /// <summary>Gets the response headers.</summary>
    public IReadOnlyDictionary<string, string> Headers { get; }

    /// <summary>Gets whether the response indicates success (2xx).</summary>
    public bool IsSuccess => StatusCode is >= 200 and < 300;

    /// <summary>Creates a new <see cref="USharpHttpResponse"/>.</summary>
    public USharpHttpResponse(int statusCode, string body,
        IReadOnlyDictionary<string, string>? headers = null)
    {
        StatusCode = statusCode;
        Body = body;
        Headers = headers ?? new Dictionary<string, string>();
    }

    /// <inheritdoc />
    public override string ToString() => $"HTTP {StatusCode} ({Body.Length} bytes)";
}
