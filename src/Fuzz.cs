using System.Net;
using System.Text;
using FuzzStorm.Configuration;
using FuzzStorm.Core;
using FuzzStorm.Entities;

namespace FuzzStorm;

sealed class Fuzzer
{
    private readonly SessionConfig _config;
    private readonly string[] _payloads;
    private readonly HttpClient _client;

    public Fuzzer(SessionConfig config, HttpClient client, string[] payloads)
    {
        _config = config;
        _payloads = payloads;
        _client = client;
    }

    public async Task Fuzz()
    {
        var results = new Results();
        var sb = new StringBuilder();
        string originalValue = string.Empty;
        Console.WriteLine(
            "\n"+
            $"Target\t\t:\t{_client.BaseAddress}\n" + 
            $"Wordlist\t:\t{_config.WordlistPath}\n" + 
            $"Method\t\t:\t{_config.Method}\n" +
            $"____________________\n\n"
        );
        Console.WriteLine(Result.ResultTableHeader());

        foreach (string payload in _payloads)
        {
            var requestMsg = CreateRequestMessage();
            bool isValid = true;

            switch (_config.FuzzTarget)
            {
                case FuzzTarget.Method:
                    originalValue = _config.Method;
                    sb.Clear().Append(originalValue);
                    sb.Replace(SessionConfig.FuzzKeyWord, payload);

                    isValid = DataValidator.ValidMethod(ref sb);
                    if (!isValid)
                    {
                        results.AddErr();
                        continue;
                    }
                    requestMsg.Method = new HttpMethod(sb.ToString());
                    break;
                    
            }

            using var responseMsg = await _client.SendAsync(requestMsg);
            await results.HandleResult(requestMsg, responseMsg, payload);
        }
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"\nERRORS: {results.ErrCount}");
        Console.ResetColor();
    }
    private HttpRequestMessage CreateRequestMessage()
    {
        var requestMsg = new HttpRequestMessage
        {
            Method = new HttpMethod(_config.Method),
            RequestUri = new Uri(_config.Url),
            Version = new Version(_config.Version),
            Content = new StringContent(_config.Body ?? "")
        };

        foreach (var hkvp in _config.Headers)
        {
            if (!IsContentHeader(hkvp.Key))
                requestMsg.Headers.Add(hkvp.Key, hkvp.Value);
            else
            {
                requestMsg.Content.Headers.Remove(hkvp.Key);
                requestMsg.Content.Headers.TryAddWithoutValidation(hkvp.Key, hkvp.Value);
            }
        }

        return requestMsg;
    }

    private bool IsContentHeader(string headerName)
    {
        var contentHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "Allow", "Content-Disposition", "Content-Encoding", "Content-Language",
            "Content-Length", "Content-Location", "Content-MD5", "Content-Range",
            "Content-Type", "Expires", "Last-Modified"
        };

        return contentHeaders.Contains(headerName);
    }
}