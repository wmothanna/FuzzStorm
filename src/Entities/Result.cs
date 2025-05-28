using System.Text;

namespace FuzzStorm.Entities;

internal class Result
{
  public RequestInfo RequestInfo { get; private set; } = null!;
  public ResponseInfo ResponseInfo { get; private set; }= null!;

  static public async Task<Result> SerializeResponseToResult(HttpRequestMessage requestMsg, HttpResponseMessage responseMsg, string payload){
    string content = await responseMsg.Content.ReadAsStringAsync();

    var result = new Result
    {
      RequestInfo = new RequestInfo
      {
        Method = requestMsg.Method.ToString(),
        Payload = payload
      },
      ResponseInfo = new ResponseInfo
      {
        StatusCode = responseMsg.StatusCode,
        SizeInBytes = Encoding.UTF8.GetByteCount(content),
        Lines = content.Count(c => c.Equals('\n')),
        Words = content.Count(c => c.Equals(' ')),
        Chars = content.Length,
      }
    };
    return result;
  }
  public static string ResultTableHeader()
  {
    return  "Payloads".PadRight(17) +
            "Methods".PadRight(30) +
            "Status".PadRight(17) +
            "Size".PadRight(17) +
            "Lines".PadRight(17) +
            "Words".PadRight(17) +
            "Characters".PadRight(17);
  }

  public override string ToString()
  {
    return $"{RequestInfo.Payload}".PadRight(17) +
            $"{RequestInfo.Method}".PadRight(30) +
            $"{(int)ResponseInfo.StatusCode}/{ResponseInfo.StatusCode}".PadRight(17) +
            $"{ResponseInfo.SizeInBytes}".PadRight(17) +
            $"{ResponseInfo.Lines}".PadRight(17) +
            $"{ResponseInfo.Words}".PadRight(17) +
            $"{ResponseInfo.Chars}".PadRight(17);
  }
}