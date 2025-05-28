using System.Net;

namespace FuzzStorm.Entities;

internal class ResponseInfo
{
  public HttpStatusCode StatusCode = 0;
  public int SizeInBytes = 0;
  public int Lines = 0;
  public int Words = 0;
  public int Chars = 0;
}