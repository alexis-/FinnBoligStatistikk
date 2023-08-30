namespace FBS.Scrapper.Models.Db
{
  using System.ComponentModel.DataAnnotations.Schema;

  public class HousingAd
  {
    #region Properties & Fields - Public

    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    #endregion
  }
}
