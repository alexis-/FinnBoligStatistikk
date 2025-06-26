namespace FinnStatistikk.DiscoveryTool.Models;

using System.Text.Json.Serialization;

public class DiscoveryRegistryModel
{
  #region Properties & Fields - Public

  [JsonPropertyName("schemas")]
  public HashSet<string> Schemas { get; set; } = new();

  [JsonPropertyName("paths")]
  public Dictionary<string, HashSet<string>> Paths { get; set; } = new();

  #endregion
}
