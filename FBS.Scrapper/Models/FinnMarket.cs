namespace FBS.Scrapper.Models
{
  public class FinnMarket
  {
    #region Constants & Statics

    public static readonly FinnMarket AgricultureThreshing     = new("SEARCH_ID_AGRICULTURE_THRESHING");
    public static readonly FinnMarket AgricultureTool          = new("SEARCH_ID_AGRICULTURE_TOOL");
    public static readonly FinnMarket AgricultureTractor       = new("SEARCH_ID_AGRICULTURE_TRACTOR");
    public static readonly FinnMarket Autocomplete             = new("SEARCH_ID_AUTOCOMPLETE");
    public static readonly FinnMarket BapAll                   = new("SEARCH_ID_BAP_ALL");
    public static readonly FinnMarket BapCommon                = new("SEARCH_ID_BAP_COMMON");
    public static readonly FinnMarket BapFree                  = new("SEARCH_ID_BAP_FREE");
    public static readonly FinnMarket BapSale                  = new("SEARCH_ID_BAP_SALE");
    public static readonly FinnMarket BapWanted                = new("SEARCH_ID_BAP_WANTED");
    public static readonly FinnMarket BapWebstoreShop          = new("SEARCH_ID_BAP_WEBSTORE_SHOP");
    public static readonly FinnMarket BoatCommon               = new("SEARCH_ID_BOAT_COMMON");
    public static readonly FinnMarket BoatDock                 = new("SEARCH_ID_BOAT_DOCK");
    public static readonly FinnMarket BoatDockWanted           = new("SEARCH_ID_BOAT_DOCK_WANTED");
    public static readonly FinnMarket BoatMotor                = new("SEARCH_ID_BOAT_MOTOR");
    public static readonly FinnMarket BoatNew                  = new("SEARCH_ID_BOAT_NEW");
    public static readonly FinnMarket BoatParts                = new("SEARCH_ID_BOAT_PARTS");
    public static readonly FinnMarket BoatPartsMotorWanted     = new("SEARCH_ID_BOAT_PARTS_MOTOR_WANTED");
    public static readonly FinnMarket BoatRent                 = new("SEARCH_ID_BOAT_RENT");
    public static readonly FinnMarket BoatRentWanted           = new("SEARCH_ID_BOAT_RENT_WANTED");
    public static readonly FinnMarket BoatUsed                 = new("SEARCH_ID_BOAT_USED");
    public static readonly FinnMarket BoatUsedWanted           = new("SEARCH_ID_BOAT_USED_WANTED");
    public static readonly FinnMarket CarAgri                  = new("SEARCH_ID_CAR_AGRI");
    public static readonly FinnMarket CarBus                   = new("SEARCH_ID_CAR_BUS");
    public static readonly FinnMarket CarCaravan               = new("SEARCH_ID_CAR_CARAVAN");
    public static readonly FinnMarket CarMobileHome            = new("SEARCH_ID_CAR_MOBILE_HOME");
    public static readonly FinnMarket CarParallelImport        = new("SEARCH_ID_CAR_PARALLEL_IMPORT");
    public static readonly FinnMarket CarTruck                 = new("SEARCH_ID_CAR_TRUCK");
    public static readonly FinnMarket CarTruckAbroad           = new("SEARCH_ID_CAR_TRUCK_ABROAD");
    public static readonly FinnMarket CarUsed                  = new("SEARCH_ID_CAR_USED");
    public static readonly FinnMarket CarVan                   = new("SEARCH_ID_CAR_VAN");
    public static readonly FinnMarket CarVanAbroad             = new("SEARCH_ID_CAR_VAN_ABROAD");
    public static readonly FinnMarket CommercialPlots          = new("SEARCH_ID_COMMERCIAL_PLOTS");
    public static readonly FinnMarket CommercialRent           = new("SEARCH_ID_COMMERCIAL_RENT");
    public static readonly FinnMarket CommercialSale           = new("SEARCH_ID_COMMERCIAL_SALE");
    public static readonly FinnMarket CompanySale              = new("SEARCH_ID_COMPANY_SALE");
    public static readonly FinnMarket DemoVertical             = new("SEARCH_ID_DEMO_VERTICAL");
    public static readonly FinnMarket EstateProject            = new("SEARCH_ID_ESTATE_PROJECT");
    public static readonly FinnMarket FollowArea               = new("SEARCH_ID_FOLLOW_AREA");
    public static readonly FinnMarket JobAggregated            = new("SEARCH_ID_JOB_AGGREGATED");
    public static readonly FinnMarket JobFulltime              = new("SEARCH_ID_JOB_FULLTIME");
    public static readonly FinnMarket JobManagement            = new("SEARCH_ID_JOB_MANAGEMENT");
    public static readonly FinnMarket JobParttime              = new("SEARCH_ID_JOB_PARTTIME");
    public static readonly FinnMarket McAtv                    = new("SEARCH_ID_MC_ATV");
    public static readonly FinnMarket McCommon                 = new("SEARCH_ID_MC_COMMON");
    public static readonly FinnMarket McScooter                = new("SEARCH_ID_MC_SCOOTER");
    public static readonly FinnMarket McSnowmobile             = new("SEARCH_ID_MC_SNOWMOBILE");
    public static readonly FinnMarket McUsed                   = new("SEARCH_ID_MC_USED");
    public static readonly FinnMarket MotorCommon              = new("SEARCH_ID_MOTOR_COMMON");
    public static readonly FinnMarket Odin                     = new("SEARCH_ID_ODIN");
    public static readonly FinnMarket RealestateAbroadHomes    = new("SEARCH_ID_REALESTATE_ABROAD_HOMES");
    public static readonly FinnMarket RealestateCommon         = new("SEARCH_ID_REALESTATE_COMMON");
    public static readonly FinnMarket RealestateHomes          = new("SEARCH_ID_REALESTATE_HOMES");
    public static readonly FinnMarket RealestateHomesBanner    = new("SEARCH_ID_REALESTATE_HOMES_BANNER");
    public static readonly FinnMarket RealestateLeisurePlots   = new("SEARCH_ID_REALESTATE_LEISURE_PLOTS");
    public static readonly FinnMarket RealestateLeisureSale    = new("SEARCH_ID_REALESTATE_LEISURE_SALE");
    public static readonly FinnMarket RealestateLettings       = new("SEARCH_ID_REALESTATE_LETTINGS");
    public static readonly FinnMarket RealestateLettingsWanted = new("SEARCH_ID_REALESTATE_LETTINGS_WANTED");
    public static readonly FinnMarket RealestateNewbuildings   = new("SEARCH_ID_REALESTATE_NEWBUILDINGS");
    public static readonly FinnMarket RealestatePlannedproject = new("SEARCH_ID_REALESTATE_PLANNEDPROJECT");
    public static readonly FinnMarket RealestatePlots          = new("SEARCH_ID_REALESTATE_PLOTS");
    public static readonly FinnMarket TravelFlight             = new("SEARCH_ID_TRAVEL_FLIGHT");
    public static readonly FinnMarket TravelHolidayrentals     = new("SEARCH_ID_TRAVEL_HOLIDAYRENTALS");
    public static readonly FinnMarket TravelLastminute         = new("SEARCH_ID_TRAVEL_LASTMINUTE");
    public static readonly FinnMarket TravelPrepackage         = new("SEARCH_ID_TRAVEL_PREPACKAGE");

    #endregion

    #region Constructors

    protected FinnMarket(string name)
    {
      Name = name;
    }

    #endregion

    #region Properties & Fields - Public

    public string Name { get; init; }

    #endregion
  }
}
