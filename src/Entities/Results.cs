using System.Runtime.InteropServices;
using FuzzStorm.Entities;

class Results
{
  public uint ErrCount { get; private set; }
  public List<Result> ResultsSet { get; private set; } = new();

  public uint AddErr(){
    return ++ErrCount;
  }

  public void AddResult(Result result){
    ResultsSet.Add(result);
  }

  private async Task<Result> AddResult(HttpRequestMessage requestMsg, HttpResponseMessage responseMsg, string payload){
    if (!responseMsg.IsSuccessStatusCode)
      ++ErrCount;

    Result result = await Result.SerializeResponseToResult(requestMsg, responseMsg, payload);
    AddResult(result);
    return result;
  }

  public async Task HandleResult(HttpRequestMessage requestMsg, HttpResponseMessage responseMsg, string payload){
    var result = await AddResult(requestMsg, responseMsg, payload);
    Console.WriteLine(result);
  }
}