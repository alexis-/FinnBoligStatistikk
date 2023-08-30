namespace FBS.Scrapper.Utilities
{
  using Microsoft.Extensions.Http;
  using Models.Config;
  using Polly;
  using Polly.Extensions.Http;
  using Polly.Retry;
  using Services;

  public static class HttpEx
  {
    #region Methods

    /// <summary>
    ///   Instantiates and configures a new <see cref="HttpClient" />. See
    ///   https://learn.microsoft.com/en-us/dotnet/fundamentals/networking/http/httpclient-guidelines
    ///   https://stackoverflow.com/a/73039478/596579
    /// </summary>
    /// <returns></returns>
    public static HttpClient CreateHttpClient(
      FinnConfig           finnConfig,
      FinnService          finnService,
      ScraperConfig        scraperConfig,
      ScraperGeneratedData scraperGenData)
    {
      var baseHandler = new SocketsHttpHandler
      {
        PooledConnectionLifetime = TimeSpan.FromMinutes(5) // Recreate every 5 minutes
      };

      var injectionHandler = new HttpRequestHeaderInjector(baseHandler, finnConfig, finnService, scraperGenData);
      var policyHandler    = new PolicyHttpMessageHandler(GetHttpRetryPolicy()) { InnerHandler = injectionHandler };

      var httpClient = new HttpClient(policyHandler)
      {
        Timeout = TimeSpan.FromMilliseconds(scraperConfig.WebRequestTimeout)
      };

      httpClient.DefaultRequestHeaders.Accept.Clear();
      httpClient.DefaultRequestHeaders.AcceptEncoding.Clear();
      httpClient.DefaultRequestHeaders.Add("Accept-Encoding", finnConfig.HeaderAcceptValue);

      foreach (var header in scraperConfig.WebRequestHeaders)
        httpClient.DefaultRequestHeaders.Add(header.Key, header.Value);

      return httpClient;
    }

    /// <summary>
    ///   See:
    ///   https://github.com/dotnet/aspnetcore/blob/main/src/HttpClientFactory/Polly/src/PolicyHttpMessageHandler.cs
    ///   https://github.com/dotnet/aspnetcore/blob/main/src/HttpClientFactory/Polly/src/DependencyInjection/PollyHttpClientBuilderExtensions.cs
    ///   https://github.com/App-vNext/Polly.Extensions.Http/blob/master/README.md
    /// </summary>
    /// <returns></returns>
    public static AsyncRetryPolicy<HttpResponseMessage> GetHttpRetryPolicy()
    {
      return HttpPolicyExtensions
             .HandleTransientHttpError()
             .WaitAndRetryAsync(5, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
    }

    /// <summary>
    ///   Extracts the body from the <paramref name="request" /> if available, returns
    ///   <paramref name="defaultValue" /> otherwise.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="defaultValue"></param>
    /// <returns></returns>
    public static async Task<string> GetBody(
      this HttpRequestMessage request,
      string                  defaultValue = "")
    {
      if (request.Content == null)
        return defaultValue;

      return await request.Content.ReadAsStringAsync().ConfigureAwait(false);
    }

    #endregion
  }
}
