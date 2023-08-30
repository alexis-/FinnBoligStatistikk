namespace FBS.Scrapper.Services
{
  using FBS.Scrapper.Models;
  using System.Collections.Concurrent;

  public class HttpResponseQueue : ConcurrentQueue<HttpResponseMessage> { }
}
