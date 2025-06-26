namespace FinnStatistikk.DiscoveryTool;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Models;
using Polly;
using Polly.Extensions.Http;
using Services;

public class Program
{
  #region Methods

  public static async Task Main(string[] args)
  {
    var host = Host.CreateDefaultBuilder(args)
                   .ConfigureServices((context, services) =>
                   {
                     // Bind appsettings.json to the settings models
                     services.Configure<DiscoverySettings>(context.Configuration.GetSection("DiscoverySettings"));
                     services.Configure<FinnApiSettings>(context.Configuration.GetSection("FinnApiSettings"));

                     // Add services as singletons
                     services.AddSingleton<UserIdentityService>();
                     services.AddSingleton<DiscoveryRegistry>();
                     services.AddSingleton<DiscoveryLogger>();
                     services.AddSingleton<DiscoveryEngine>();
                     services.AddSingleton<DiscoveryWorker>();
                     services.AddSingleton<ProgressManager>(); // Register the new service

                     // Add the custom delegating handler for API signing
                     services.AddTransient<SigningDelegatingHandler>();

                     // Configure the HttpClient for the Finn API
                     services.AddHttpClient<IFinnApiClient, FinnApiClient>(client =>
                             {
                               var settings = context.Configuration.GetSection("FinnApiSettings").Get<FinnApiSettings>()
                                 ?? new FinnApiSettings();
                               client.BaseAddress = new Uri(settings.ApiBaseUrl);
                             })
                             .AddHttpMessageHandler<SigningDelegatingHandler>()
                             // Use the overload that provides the IServiceProvider
                             .AddPolicyHandler((serviceProvider, request) => GetRetryPolicy(serviceProvider));
                   })
                   .Build();

    var registry        = host.Services.GetRequiredService<DiscoveryRegistry>();
    var worker          = host.Services.GetRequiredService<DiscoveryWorker>();
    var progressManager = host.Services.GetRequiredService<ProgressManager>();
    var settings        = host.Services.GetRequiredService<IOptions<DiscoverySettings>>().Value;

    // Load the registry from disk before starting
    await registry.LoadAsync();

    // Check for saved progress and prompt user
    ScrapingProgress? progress = await progressManager.LoadProgressAsync();
    if (progress != null)
    {
      Console.WriteLine("Previous scraping progress found.");
      char choice = ' ';
      while (choice != 'c' && choice != 'r')
      {
        Console.Write("Do you want to (c)ontinue or (r)estart from scratch? ");
        var input = Console.ReadLine()?.Trim().ToLowerInvariant();
        if (!string.IsNullOrEmpty(input) && input.Length == 1)
        {
          choice = input[0];
        }

        if (choice != 'c' && choice != 'r')
        {
          Console.WriteLine("Invalid input. Please press 'c' to continue or 'r' to restart.");
        }
      }

      if (choice == 'r')
      {
        progressManager.DeleteProgress();
        progress = null; // Discard progress
        Console.WriteLine("Restarting from the beginning.");
      }
      else
      {
        if (progress.CurrentMarketIndex < settings.MarketsToScan.Length)
        {
          var market = settings.MarketsToScan[progress.CurrentMarketIndex];
          Console.WriteLine($"Resuming scan at Market '{market.SearchKey}', Page {progress.CurrentPageIndex + 1}.");
        }
        else
        {
          Console.WriteLine($"Resuming scan from market index {progress.CurrentMarketIndex}...");
        }
      }
    }

    // Run the discovery process
    var cts = new CancellationTokenSource();
    Console.CancelKeyPress += (_, e) =>
    {
      Console.WriteLine("\nCancellation requested...");
      cts.Cancel();
      e.Cancel = true;
    };

    await worker.RunAsync(progress, cts.Token);

    // Final save after the run (optional but good practice)
    await registry.SaveAsync();

    Console.WriteLine("Discovery Tool has finished. Press any key to exit.");
    Console.ReadKey(true);
  }

  // CHANGE: Method now accepts IServiceProvider to resolve the logger
  private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy(IServiceProvider serviceProvider)
  {
    // Resolve the logger from the service provider
    var logger = serviceProvider.GetRequiredService<ILogger<Program>>();

    return HttpPolicyExtensions
           .HandleTransientHttpError() // Handles HttpRequestException, 5xx, and 408
           .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                              (outcome, timespan, retryAttempt, context) =>
                              {
                                logger.LogWarning(
                                  "Request failed with {StatusCode}. Delaying for {Delay}s, then making retry {RetryAttempt}...",
                                  outcome.Result?.StatusCode,
                                  timespan.TotalSeconds,
                                  retryAttempt);
                              });
  }

  #endregion
}
