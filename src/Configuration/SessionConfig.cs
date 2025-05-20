using System.Net;

namespace FuzzStorm.Configuration;
internal class SessionConfig // in-memory representation of end-user configuration
{
    public string? Method { get; set; }
    public string Url { get; set; } = null!;
    public string? Version { get; set; }
    public Dictionary<string, string>? Headers { get; set; }
    public string? Body { get; set; }
    public WebProxy? Proxy { get; set; }
    public bool AllowRedirects { get; set; } = true;
    public TimeSpan TimeoutInSeconds { get; set; } = TimeSpan.FromSeconds(10);

    public SessionConfig ()
    {
        Method = "GET";
        Version = HttpVersion.Version11.ToString();
    }

    public bool HasContent()
    {
        return Body is not null;
    }
    public bool isProxied()
    {
        return Proxy is not null;
    }

}
