using System.Dynamic;
using System.Net;

namespace FuzzStorm.Configuration;
internal class SessionConfig // in-memory representation of end-user configuration
{
    public string Method { get; set; } = null!;
    public string Url { get; set; } = null!;
    public string Version { get; set; } = null!;
    public Dictionary<string, string> Headers { get; set; } = null!;
    public string? Body { get; set; }
    public WebProxy? Proxy { get; set; }
    public bool AllowRedirects { get; set; } = true;
    public bool UseDefaultHeaders { get; set; } = true;
    public TimeSpan TimeoutInSeconds { get; set; }
    public TimeSpan DelayInSeconds { get; set; }
    public FuzzTarget FuzzTarget { get; set; }
    public string WordlistPath { get; set; } = null!;
    public string? TargetedHeader { get; set; }
    public static readonly string FuzzKeyWord = "FuzzStorm";
    public SessionConfig ()
    {
        Method = "GET";
        Version = HttpVersion.Version11.ToString();
        TimeoutInSeconds = TimeSpan.FromSeconds(10);
        DelayInSeconds = TimeSpan.FromSeconds(0);
        FuzzTarget = FuzzTarget.None;
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
