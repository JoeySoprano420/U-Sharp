using System.Net.Sockets;
using System.Text;
using USharp.Core.Exceptions;

namespace USharp.Net.Sockets;

/// <summary>
/// A simple TCP client for U# programs.
/// </summary>
public sealed class USharpTcpClient : IDisposable
{
    private TcpClient? _client;
    private NetworkStream? _stream;
    private bool _disposed;

    /// <summary>Gets whether the client is connected.</summary>
    public bool IsConnected => _client?.Connected ?? false;

    /// <summary>Connects to the specified host and port.</summary>
    public async Task ConnectAsync(string host, int port,
        CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        ArgumentException.ThrowIfNullOrWhiteSpace(host);
        try
        {
            _client = new TcpClient();
            await _client.ConnectAsync(host, port, cancellationToken).ConfigureAwait(false);
            _stream = _client.GetStream();
        }
        catch (SocketException ex)
        {
            throw new USharpException($"Failed to connect to {host}:{port}.", ex);
        }
    }

    /// <summary>Sends a UTF-8 string to the remote host.</summary>
    public async Task SendAsync(string data,
        CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        EnsureConnected();
        var bytes = Encoding.UTF8.GetBytes(data);
        await _stream!.WriteAsync(bytes, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>Reads a UTF-8 string from the remote host (up to <paramref name="bufferSize"/> bytes).</summary>
    public async Task<string> ReceiveAsync(int bufferSize = 4096,
        CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        EnsureConnected();
        var buffer = new byte[bufferSize];
        int bytesRead = await _stream!.ReadAsync(buffer, cancellationToken).ConfigureAwait(false);
        return Encoding.UTF8.GetString(buffer, 0, bytesRead);
    }

    private void EnsureConnected()
    {
        if (!IsConnected)
            throw new USharpException("TCP client is not connected.");
    }

    /// <inheritdoc />
    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        _stream?.Dispose();
        _client?.Dispose();
    }
}
