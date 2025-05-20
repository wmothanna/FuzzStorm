using CommandLine;
namespace FuzzStorm.Models;

internal class Options
{
    [Option('m', "method")]
    public string? Method { get; set; }

    [Option
        ('u', "url",
        Required = true,
        HelpText = "specifies target uri, it can be relative or absolute, only schemes accepted in absolute URIs are http and https")
    ]
    public string? Uri { get; set; }

    [Option('v', "set-version")]
    public string? Version { get; set; }

    [Option(
        'H', "header", Separator = ',',
        HelpText = "Sets http request headers, comma ',' is used to separate multiple headers" +
        " (spaces before and after the comma are disallowed), colon ':' must be mentioned in the" +
        " header to separate between key and value, if two duplicate header keys exists last one" +
        " will overwrite previous ones."
    )]
    public IEnumerable<string>? Headers { get; set; }

    [Option("default-headers", Default = false)]
    public bool UseDefaultHeaders { get; set; }

    [Option('b', "body")]
    public string? Body { get; set; }


    [Option('w', "wordlist", Required = true)]
    public string? WordlistPath { get; set; }


    [Option('t', "timeout")]
    public TimeSpan? Timeout { get; set; }

    [Option('f', "follow-redirects", Default = true)]
    public bool AllowRedirects { get; set; }

    [Option('p', "proxy")]
    public string? proxyAddr { get; set; }

    [Option('d', "delay")]
    public TimeSpan? Delay { get; set; }
    

}
