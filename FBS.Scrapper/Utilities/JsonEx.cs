namespace FBS.Scrapper.Utilities
{
  using Newtonsoft.Json;

  public static class JsonEx
  {
    #region Methods

    public static async Task SerializeToFileAsync<T>(this T value, string path, CancellationToken ct = default)
      where T : new()
    {
      var json = JsonConvert.SerializeObject(value, typeof(T), Formatting.Indented, default);

      await File.WriteAllTextAsync(path, json, ct).ConfigureAwait(false);
    }

    public static async Task<T?> DeserializeFromFileAsync<T>(string path, CancellationToken ct = default)
      where T : new()
    {
      if (!File.Exists(path))
        return default;

      var json = await File.ReadAllTextAsync(path, ct).ConfigureAwait(false);

      return JsonConvert.DeserializeObject<T>(json);
    }

    public static void SerializeToFile<T>(this T value, string path)
      where T : new()
    {
      var json = JsonConvert.SerializeObject(value, typeof(T), Formatting.Indented, default);

      File.WriteAllText(path, json);
    }

    public static T? DeserializeFromFile<T>(string path)
      where T : new()
    {
      if (!File.Exists(path))
        return default;

      var json = File.ReadAllText(path);

      return JsonConvert.DeserializeObject<T>(json);
    }

    #endregion
  }
}
