namespace FBS.Scrapper
{
  using Database;
  using Services;
  using Utilities;
  using Workers;

  public class Program
  {
    #region Methods

    public static void Main(string[] args)
    {
      IHost host = Host.CreateDefaultBuilder(args)
                       .ConfigureServices((hostContext, services) =>
                       {
                         services.LoadAndInjectConfig(hostContext);
                         services.ConfigureDatabase(hostContext);

                         services.AddSingleton<FinnService>();

                         services.AddSingleton<HttpRequestQueue>();
                         services.AddSingleton<HttpResponseQueue>();
                         services.AddHostedService<HttpRequestExecutorWorker>();
                         services.AddHostedService<HttpResponseProcessorWorker>();
                         services.AddHostedService<PeriodicSearchRequestWorker>();
                       })
                       .Build();

      host.CreateDatabaseIfNotExists();
      host.Run();
    }

    #endregion
  }
}
