using System.Net;
using System.Text;

namespace FuzzStorm.Core;
static class DataValidator
{
  public static bool ValidMethod(string? method, out string? validMethod){
    validMethod = null;
    string? trimmed = method?.Trim();
    if (!string.IsNullOrEmpty(trimmed))
    {
      validMethod = trimmed;
      return true;
    }
    return false;
  }
  public static bool ValidMethod(ref StringBuilder method){
    if (!string.IsNullOrWhiteSpace(method.ToString()))
    {
      method.ToString().Trim();
      return true;
    }
    return false;
  }
  public static bool ValidUrl(string url, ref UrlErr urlErr, out Uri? result){
    try
    {
      if (Uri.TryCreate(url, UriKind.Absolute, out result) && result is not null)
      {
        if
        (
          result.Scheme != Uri.UriSchemeHttp
          && result.Scheme != Uri.UriSchemeHttps
        )
        { urlErr = UrlErr.InvalidScheme; }
        else
        { return true; }
      }
      else { urlErr = UrlErr.InvalidUri; }
      return false;
    }
    catch { throw; }
  }
  public static bool ValidVersion(string? versionStr, out Version? version){
    try
    {
      return (
        Version.TryParse(versionStr, out version!)
        && version is not null
      );
    }
    catch { throw; }
  }
  public static bool ValidHeaders(IEnumerable<string> headers, out Dictionary<string, string>? result)
  {
    try
    {
      result = HeaderParser.ParseHeaders(headers);
      return true;
    }
    catch { throw; };
  }
  public static bool ValidBody(string? body)
  {
    return body is not null;
  }
}