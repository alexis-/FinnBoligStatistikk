namespace FBS.Scrapper.Services
{
  using System.Collections.Concurrent;
  using Models;

  public class HttpRequestQueue : ConcurrentQueue<FinnHttpRequestMessage> { }
}
