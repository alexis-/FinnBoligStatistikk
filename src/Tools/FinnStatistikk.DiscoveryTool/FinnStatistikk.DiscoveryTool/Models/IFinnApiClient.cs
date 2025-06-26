namespace FinnStatistikk.DiscoveryTool.Models;

using Newtonsoft.Json.Linq;

public interface IFinnApiClient
{
  Task<JObject?> SearchAsync(string    market, int page);
  Task<JObject?> GetAdViewAsync(string adId);
}
