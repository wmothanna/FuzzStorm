using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Security;
using FuzzStorm.Configuration;
using FuzzStorm.Models.Configuration;

namespace FuzzStorm;

sealed class FuzzSession : IDisposable
{

  private HttpMessageHandler _clientHandler = null!;
  private HttpClient _client = null!;
  private readonly SessionConfig _config = null!;
  private bool disposedValue;

  public FuzzSession(string[] args)
  {
    _config = SessionConfigParser.ParseConfig(args);
    _clientHandler = CreateHandler();
    _client = CreateClient(_clientHandler);
  }
  public void DisplayConfig()
  {
    if (_config is null)
      return;

    string body = "";

    if (_config.Body is not null)
      body = _config.Body;

    Console.Write(_config.Method + " ");
    Console.Write(_config.Url + " ");
    Console.Write(_config.Version + " \r\n");
    Console.WriteLine();
    Console.WriteLine(string.Join("\r\n", _config.Headers ?? []));
    Console.WriteLine();
    Console.WriteLine(body);
    Console.WriteLine();
    Console.WriteLine(_config.WordlistPath);
  }

  public async Task start()
  {

    string[] payloads = await GetPayloads();

    var fuzzer = new Fuzzer(_config, _client, payloads);
    await fuzzer.Fuzz();

  }

  private HttpClient CreateClient(HttpMessageHandler handler){
    var client = new HttpClient(handler);
    try
    {
      client.BaseAddress = new Uri(_config.Url);
      client.DefaultRequestVersion = new Version (_config.Version);

      foreach(var header in _config.Headers){
        client.DefaultRequestHeaders.TryAddWithoutValidation(header.Key, header.Value);
      }
    } catch
    { throw new Exception($"Some thing went wrong instantiating {nameof(HttpClient)} object"); }
    return client;
  }
  private HttpMessageHandler CreateHandler(){
    try
    {
      return new SocketsHttpHandler{
        AllowAutoRedirect = _config.AllowRedirects,
        PooledConnectionLifetime = TimeSpan.FromMinutes(1),
        ConnectTimeout = _config.TimeoutInSeconds,
      };
    } catch
    { throw new Exception($"Some thing went wrong instantiating {nameof(SocketsHttpHandler)} object"); }
  }
    private async Task<string[]> GetPayloads(){
      var cts = new CancellationTokenSource();
      return await File.ReadAllLinesAsync(Path.GetFullPath(_config.WordlistPath), cts.Token);
    }

  private void Dispose(bool disposing)
  {
    if (!disposedValue)
    {
      if (disposing)
      {
        _clientHandler.Dispose();
        _client.Dispose();
      }
      disposedValue = true;
    }
  }

  public void Dispose()
  {
    Dispose(disposing: true);
    GC.SuppressFinalize(this);
  }
}