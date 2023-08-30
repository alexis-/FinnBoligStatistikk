namespace FBS.Scrapper.Database
{
  using FBS.Scrapper.Models.Db;
  using Microsoft.EntityFrameworkCore;

  public class FBSDbContext : DbContext
  {
    #region Constructors

    public FBSDbContext(DbContextOptions<FBSDbContext> options) : base(options) { }

    #endregion

    #region Properties & Fields - Public

    public required DbSet<HousingAd> HousingAds { get; set; }

    #endregion
  }
}
