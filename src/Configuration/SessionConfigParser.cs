using FuzzStorm.Models;
using CommandLine;
using System.Net;
using FuzzStorm.Configuration;
using FuzzStorm.Core;

namespace FuzzStorm.Models.Configuration;
internal static class SessionConfigParser
{
    private static FuzzTarget _fuzzTarget = FuzzTarget.None;
    private static string _fuzzTargetArg = "";
    private static void PreParse(SessionConfigBuilder builder, Options o)
    {
        if (o.AllowRedirects)
            builder.AllowRedirects();
        if (o.UseDefaultHeaders)
            builder.UseDefaultHeaders();
    }
    private static void ParseMethod(SessionConfigBuilder builder, string? method)
    {
        if (string.IsNullOrWhiteSpace(method)) return;

        builder.WithMethod(method);

        if (_fuzzTarget == FuzzTarget.None && method.Contains(_fuzzTargetArg))
            _fuzzTarget = FuzzTarget.Method;
    }
    private static void ParseUrl(SessionConfigBuilder builder, string? url)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            Helper.InvalidCmdArgument("target url is required");
            return;
        }

        UrlErr urlErr = UrlErr.None;

        builder.WithUrl(url, ref urlErr);

        if (urlErr != UrlErr.None)
        {
            if (urlErr == UrlErr.InvalidScheme)
                Helper.InvalidCmdArgument("Invalid Uri Scheme");
            else
                Helper.InvalidCmdArgument("Invalid Uri");
        }

        if (_fuzzTarget == FuzzTarget.None && url.Contains(_fuzzTargetArg))
            _fuzzTarget = FuzzTarget.Url;
    }
    private static void ParseVersion(SessionConfigBuilder builder, string? version)
    {
        if (string.IsNullOrWhiteSpace(version))
            return;

        builder.WithVersion(version);

        if (_fuzzTarget == FuzzTarget.None && version.Contains(_fuzzTargetArg))
            _fuzzTarget = FuzzTarget.Version;
    }
    private static void ParseHeaders(SessionConfigBuilder builder, IEnumerable<string>? headers)
    {
        if (headers is null)
            headers = new List<string>();
            
        builder.WithHeaders(headers);

        if (_fuzzTarget == FuzzTarget.None)
        {
            foreach (var header in headers) // searches for targeted header
            {
                if (header.Contains(_fuzzTargetArg))
                    _fuzzTarget = FuzzTarget.Headers;
            }
        }
    }
    private static void ParseBody(SessionConfigBuilder builder, string? body)
    {
        if (string.IsNullOrWhiteSpace(body))
            return;

        builder.WithBody(body);

        if (_fuzzTarget == FuzzTarget.None && _fuzzTargetArg.Contains(body))
            _fuzzTarget = FuzzTarget.Body;
    }
    private static void ParseWordlistPath(SessionConfigBuilder builder, string? path)
    {
        if (_fuzzTargetArg.Contains(path ?? ""))
            Helper.InvalidCmdArgument($"{SessionConfig.FuzzKeyWord} can't be on wordlist");
        if (string.IsNullOrEmpty(path))
            Helper.InvalidCmdArgument("Wordlist path must be provided");
        else {
            builder.WithWordlist(path, out WordlistErr err);

            if (err == WordlistErr.NotFound)
                Helper.InvalidCmdArgument($"No such file `{path}`");
        }
    }

    private static string GetFirstFuzzTarget(string[] args)
    {
        string target = "";
        int i = 0;
        do
        {
            if (i == args.Length)
                Helper.InvalidCmdArgument($"fuzz target wasn't specified, specify it with {SessionConfig.FuzzKeyWord}");
            target = args[i];
            ++i;
        } while (!target.Contains(SessionConfig.FuzzKeyWord)); // until first FuzzKeyWord

        return target;
    }

    public static SessionConfig ParseConfig(string[] args)
    {
        _fuzzTargetArg = GetFirstFuzzTarget(args);

        SessionConfigBuilder sessionConfigBuilder = new();

        var parser = Parser.Default.ParseArguments<Options>(args);

        try
        {
            parser.WithParsed<Options>
            (
                (o) =>
                {
                    PreParse(sessionConfigBuilder, o); // parses arguments that other parse methods relay on.
                    ParseMethod(sessionConfigBuilder, o.Method);
                    ParseUrl(sessionConfigBuilder, o.Uri);
                    ParseVersion(sessionConfigBuilder, o.Version);
                    ParseHeaders(sessionConfigBuilder, o.Headers);
                    ParseBody(sessionConfigBuilder, o.Body);
                    ParseWordlistPath(sessionConfigBuilder, o.WordlistPath);

                    sessionConfigBuilder.WithFuzzTarget(_fuzzTarget);
                }
            );
        }
        catch (ArgumentNullException ex)
        {
            Console.WriteLine(ex.ParamName + "\r\n");
            Environment.Exit(0);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message + "\r\n");
            Environment.Exit(0);
        }

        parser.WithNotParsed<Options>
        (
            (errs) =>
            {
                Environment.Exit(0);
            }
        );

        return sessionConfigBuilder.Build();
    }

}
