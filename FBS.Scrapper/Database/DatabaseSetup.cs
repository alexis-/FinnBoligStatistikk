namespace FBS.Scrapper.Database
{
  using Microsoft.EntityFrameworkCore;
  using Throw;

  public static class DatabaseSetup
  {
    #region Methods

    public static void ConfigureDatabase(this IServiceCollection services, HostBuilderContext hostContext)
    {
      var databaseType = hostContext.Configuration.GetValue<string>(Const.DatabaseConfig.DatabaseType);
      databaseType.ThrowIfNull().IfWhiteSpace();

      var connectionString = hostContext.Configuration.GetConnectionString(databaseType);

      switch (databaseType)
      {
        case "PostgreSQL":
          services.AddDbContext<FBSDbContext>(
            options => options.UseNpgsql(connectionString));
          break;

        case "SQLite":
          services.AddDbContext<FBSDbContext>(
            options => options.UseSqlite(connectionString));
          break;

        default:
          throw new ArgumentException("Invalid database type");
      }
    }

    public static void CreateDatabaseIfNotExists(this IHost host)
    {
      using var scope    = host.Services.CreateScope();
      var       services = scope.ServiceProvider;

      try
      {
        var context = services.GetRequiredService<FBSDbContext>();

        context.Database.EnsureCreated();

        // Populate data here if necessary
      }
      catch (Exception ex)
      {
        var logger = services.GetRequiredService<ILogger<Program>>();

        logger.LogError(ex, "An error occurred while creating the database.");
      }
    }

    #endregion
  }
}
