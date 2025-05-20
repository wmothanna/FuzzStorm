using System;
using System.Collections.Generic;
using System.Linq;

public class HeaderParser
{
    public static Dictionary<string, string> ParseHeaders(IEnumerable<string> headerStrings)
    {
        var headers = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        
        foreach (var header in headerStrings)
        {
            if (string.IsNullOrWhiteSpace(header))
                continue;

            var colonIndex = header.IndexOf(':');
            if (colonIndex <= 0)
                throw new FormatException($"Invalid header format: '{header}'. Expected 'Name: Value'.");

            var name = header.Substring(0, colonIndex).Trim();
            var value = header.Substring(colonIndex + 1).Trim();

            if (string.IsNullOrEmpty(name))
                throw new FormatException($"Header name cannot be empty in: '{header}'");

            // validates header name doesn't contain forbidding characters
            if (!System.Text.RegularExpressions.Regex.IsMatch(name, @"^[!#$%&'*+\-.^_`|~0-9a-zA-Z]+$")) 
                throw new FormatException($"Invalid header name: '{name}'");

            headers[name] = value;
        }

        return headers;
    }
}