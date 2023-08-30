namespace FBS.Scrapper
{
  /// <summary>Contains all the application's constants.</summary>
  public static class Const
  {
    #region Constants & Statics

    public const string AppName = "FinnStatistik";

    #endregion

    public static class DatabaseConfig
    {
      #region Constants & Statics

      public const string DatabaseType = nameof(DatabaseType);
      public const string PostgreSQL   = nameof(PostgreSQL);
      public const string SQLite       = nameof(SQLite);

      #endregion
    }

    public static class Scrapper
    {
      #region Constants & Statics

      public const string Id     = $"{{{nameof(Id)}}}";
      public const string Page   = $"{{{nameof(Page)}}}";
      public const string Market = $"{{{nameof(Market)}}}";

      #endregion
    }

    public static class Finn
    {
      public static class GatewayServices
      {
        #region Constants & Statics

        public const string NAM2               = "NAM2";
        public const string SearchQuest        = "Search-Quest";
        public const string CompanyProfile     = "COMPANY-PROFILE";
        public const string LoanCalculations   = "LOAN-CALCULATIONS";
        public const string FavoriteManagement = "FAVORITE-MANAGEMENT";

        #endregion
      }

      public static class AdTypes
      {
        #region Constants & Statics

        public const string BAP_CRISIS_OFFER       = "bap-crisis-offer";
        public const string BAP_CRISIS_WISH        = "bap-crisis-wish";
        public const string BAP_GIFT_OFFER         = "bap-gift-offer";
        public const string BAP_GIFT_WISH          = "bap-gift-wish";
        public const string BAP_GIVEAWAY           = "bap-giveaway";
        public const string BAP_SELL               = "bap-sell";
        public const string BAP_WANTED             = "bap-wanted";
        public const string BAP_WEBSTORE           = "webstore";
        public const string CAR_USED               = "car-used";
        public const string JOB_AGGREGATED         = "job-aggregated";
        public const string JOB_FULL_TIME          = "job-full-time";
        public const string JOB_MANAGEMENT         = "job-management";
        public const string JOB_PART_TIME          = "job-part-time";
        public const string REALESTATE_ABROAD_SALE = "realestate-abroad-sale";
        public const string REALESTATE_DEV_PROJECT = "realestate-development-project";
        public const string REALESTATE_HOME        = "realestate-home";
        public const string RECOMMERCE_SELL        = "recommerce-sell";

        #endregion
      }
    }
  }
}
