namespace FBS.Scrapper.Utilities
{
  using Models.Config;

  public static class ConfigEx
  {
    #region Methods

    /// <summary>
    ///   Computes the application's private storage folder path based on
    ///   <see cref="Environment.SpecialFolder.ApplicationData" />.
    /// </summary>
    /// <returns></returns>
    private static string GetDataStorageDirPath()
    {
      var appDataRoaming = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
      var appName        = Const.AppName;
      var path           = Path.Combine(appDataRoaming, appName);

      return path;
    }

    /// <summary>
    ///   Computes <see cref="ScraperGeneratedData" />'s file path. See
    ///   <see cref="GetDataStorageDirPath" />
    /// </summary>
    /// <returns></returns>
    private static string GetScraperGenDataFilePath()
    {
      return Path.Combine(GetDataStorageDirPath(), $"{nameof(ScraperGeneratedData)}.json");
    }

    /// <summary>
    ///   Loads or generates all configuration and data files necessary for the application to
    ///   execute properly and injects them into the DI service collection.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="hostContext"></param>
    public static void LoadAndInjectConfig(this IServiceCollection services, HostBuilderContext hostContext)
    {
      var dataStoragePath = GetDataStorageDirPath();
      Directory.CreateDirectory(dataStoragePath);

      var scraperConfig = services.LoadAndInjectConfig<ScraperConfig>(hostContext);
      _ = services.LoadAndInjectConfig<FinnConfig>(hostContext);

      services.LoadOrGenerateData(scraperConfig);
    }

    /// <summary>Generates or loads the generated data file.</summary>
    /// <param name="services"></param>
    /// <param name="scraperConfig"></param>
    private static void LoadOrGenerateData(this IServiceCollection services, ScraperConfig scraperConfig)
    {
      var filePath = GetScraperGenDataFilePath();

      var scraperGenData = JsonEx.DeserializeFromFile<ScraperGeneratedData>(filePath) ?? new();

      if (scraperGenData.CreateIdentitiesIfRequired(scraperConfig))
        scraperGenData.SerializeToFile(filePath);

      services.AddSingleton<ScraperGeneratedData>(scraperGenData);
    }

    /// <summary>
    ///   Loads or instantiates with default values configuration object of type
    ///   <typeparamref name="T" />.
    /// </summary>
    /// <typeparam name="T">Configuration model's type</typeparam>
    /// <param name="services"></param>
    /// <param name="hostContext"></param>
    /// <returns></returns>
    private static T LoadAndInjectConfig<T>(this IServiceCollection services, HostBuilderContext hostContext)
      where T : class, new()
    {
      T config = hostContext.Configuration.GetSection(nameof(T)).Get<T>() ?? new();

      services.AddSingleton<T>(config);

      return config;
    }

    #endregion
  }
}
