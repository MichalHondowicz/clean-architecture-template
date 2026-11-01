using System.Net;
using System.Net.Sockets;
using System.Text;

namespace HookTests.Infrastructure;

/// <summary>A local stand-in for an ntfy topic: records each POST body and answers with a fixed status.</summary>
public sealed class TopicServer : IDisposable
{
    private readonly HttpListener _listener = new();
    private readonly List<string> _messages = [];
    private readonly HttpStatusCode _status;
    private readonly Task _serving;

    private TopicServer(HttpStatusCode status)
    {
        _status = status;
        int port = FreePort();
        Url = $"http://127.0.0.1:{port}/loop-topic";
        _listener.Prefixes.Add($"http://127.0.0.1:{port}/");
        _listener.Start();
        _serving = Task.Run(ServeAsync);
    }

    public string Url { get; }

    public IReadOnlyList<string> Messages
    {
        get
        {
            lock (_messages)
            {
                return [.. _messages];
            }
        }
    }

    public static TopicServer Start(HttpStatusCode status = HttpStatusCode.OK) => new(status);

    public void Dispose()
    {
        _listener.Close();
        try
        {
            _serving.Wait(TimeSpan.FromSeconds(5));
        }
        catch (AggregateException)
        {
            // The listener throws once it is closed; that ends the loop.
        }
    }

    private static int FreePort()
    {
        using var probe = new TcpListener(IPAddress.Loopback, 0);
        probe.Start();
        return ((IPEndPoint)probe.LocalEndpoint).Port;
    }

    private async Task ServeAsync()
    {
        while (_listener.IsListening)
        {
            HttpListenerContext context;
            try
            {
                context = await _listener.GetContextAsync();
            }
            catch (Exception exception) when (exception is HttpListenerException or ObjectDisposedException)
            {
                return;
            }

            using (var reader = new StreamReader(context.Request.InputStream, Encoding.UTF8))
            {
                string body = await reader.ReadToEndAsync();
                lock (_messages)
                {
                    _messages.Add(body);
                }
            }
            context.Response.StatusCode = (int)_status;
            context.Response.Close();
        }
    }
}
