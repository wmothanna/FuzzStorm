using System.Net;
using FuzzStorm.Core;
using System.Linq;

namespace FuzzStorm.Configuration;
internal class SessionConfigBuilder 
{
    private SessionConfig _sessionConfig = new();

    public SessionConfigBuilder WithMethod(string? method)
    {
        if (DataValidator.ValidMethod(method, out string? validMethod) && validMethod is not null)
            _sessionConfig.Method = validMethod;

        return this;
    }
    public SessionConfigBuilder WithUrl(string url, ref UrlErr urlErr)
    {
        try
        {
        if (DataValidator.ValidUrl(url, ref urlErr, out Uri? result) && result is not null)
        {
            _sessionConfig.Url = result.ToString();
        }
        }
        catch { throw; }
        return this;
    }
    public SessionConfigBuilder WithVersion(string version)
    {
        try
        {
            if (DataValidator.ValidVersion(version, out Version? result) && result is not null)
                _sessionConfig.Version = result.ToString();
        }
        catch { throw; }
        return this;
    }
    public SessionConfigBuilder WithHeaders(IEnumerable<string> headers)
    {
        IEnumerable<string> defaultHeaders = new List<string>()
        {
            "Accept: application/json",
            "Content-Type: application/json",
            "User-Agent: FuzzStorm"
        };

        var combinedHeaders = _sessionConfig.UseDefaultHeaders
            ? defaultHeaders.Concat(headers ?? Enumerable.Empty<string>())
            : headers ?? Enumerable.Empty<string>();

        try
        {
            if (DataValidator.ValidHeaders(combinedHeaders, out Dictionary<string, string>? result) && result is not null)
                _sessionConfig.Headers = result;
            else
            {
                _sessionConfig.Headers = _sessionConfig.UseDefaultHeaders
                ? HeaderParser.ParseHeaders(defaultHeaders)
                : new Dictionary<string, string>();
            }
            return this;
        }
        catch { throw; }
    }
    public SessionConfigBuilder WithBody(string body)
    {
        if (DataValidator.ValidBody(body))
        _sessionConfig.Body = body;

        return this;
    }
    public SessionConfigBuilder WithProxy(string? proxyAddr)
    {
        try
        {
            Uri? address = null;
            if (proxyAddr is not null)
                new Uri(proxyAddr);
            _sessionConfig.Proxy = new WebProxy(address);
        }
        catch { throw; }
        return this;
    }
    public SessionConfigBuilder AllowRedirects(bool allow = true)
    {
        _sessionConfig.AllowRedirects = allow;
        return this;
    }
    public SessionConfigBuilder UseDefaultHeaders(bool use = true)
    {
        _sessionConfig.UseDefaultHeaders = use;
        return this;
    }
    public SessionConfigBuilder WithTimeout(TimeSpan timeout)
    {
        _sessionConfig.TimeoutInSeconds = timeout;
        return this;
    }

    public SessionConfigBuilder WithDelay(TimeSpan? delay)
    {
        _sessionConfig.DelayInSeconds = delay ?? TimeSpan.FromSeconds(0);
        return this;
    }
    public SessionConfigBuilder WithFuzzTarget(FuzzTarget target)
    {
        _sessionConfig.FuzzTarget = target;
        return this;
    }
    public SessionConfigBuilder WithWordlist(string path, out WordlistErr err)
    {
        err = WordlistErr.None;

        if (!File.Exists(path))
            err = WordlistErr.NotFound; 
            
        _sessionConfig.WordlistPath = path;
        return this;
    }
    public SessionConfigBuilder WithTargetedHeader(string? header) {
        _sessionConfig.TargetedHeader = header;
        return this;
    }

    public SessionConfigBuilder Reset()
    {
        _sessionConfig = new();
        return this;
    }
    public SessionConfig Build()
    {
        var messageConfig = _sessionConfig;
        this.Reset();
        return messageConfig;
    }
}
