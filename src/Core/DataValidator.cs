using System.Net;

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
  public static bool ValidUrl(string uri){
    return ValidUrl(uri, out UrlErr uriErr, out Uri? result);
  }
  public static bool ValidUrl(string uri, out UrlErr uriErr){
    return ValidUrl(uri, out uriErr, out Uri? result);
  }
  public static bool ValidUrl(string uri, out UrlErr uriErr, out Uri? result){
    uriErr = UrlErr.None;
    try
    {
      if (Uri.TryCreate(uri, UriKind.Absolute, out result) && result is not null)
      {
        if
        (
          result.Scheme != Uri.UriSchemeHttp
          && result.Scheme != Uri.UriSchemeHttps
        )
        { uriErr = UrlErr.InvalidScheme; }
        else
        { return true; }
      }
      else { uriErr = UrlErr.InvalidUri; }
      return false;
    }
    catch { throw; }
  }
  public static bool ValidVersion(string? versionStr, out Version? version){
    try
    {
      if
      (
        Version.TryParse(versionStr, out version!)
        && version is not null
      )
      {
        if
        (
          version.Equals(HttpVersion.Version10)
          || version.Equals(HttpVersion.Version11)
          || version.Equals(HttpVersion.Version20)
          || version.Equals(HttpVersion.Version30)
        )
        { return true; }
      }
      return false;
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