namespace Playground
{
  using System.Security.Cryptography;
  using System.Text;

  internal class Program
  {
    #region Constants & Statics

    private const char   CR      = '\r';
    private const string HmacKey = "MQD1MzLjZ2ZgLwp4Zl00ATD5YJV5ATRgLzVlBTEvLmNkAzR2";

    #endregion

    #region Methods

    private static void Main(string[] args)
    {
      var msg = "GET;/search/SEARCH_ID_BAP_COMMON?client=ANDROID&include_results=true;Search-Quest;";
      var key = Decode(HmacKey);

      Console.WriteLine(GenerateHMACSHA512(msg, key));

      Console.ReadKey();
    }

    public static string GenerateHMACSHA512(string message, string key)
    {
      // Convert key and message to byte arrays
      byte[] keyBytes     = Encoding.UTF8.GetBytes(key);
      byte[] messageBytes = Encoding.UTF8.GetBytes(message);

      using (var hashAlgorithm = new HMACSHA512(keyBytes))
      {
        // Compute hash
        byte[] hash = hashAlgorithm.ComputeHash(messageBytes);

        // Convert result to hexadecimal string
        return Convert.ToBase64String(hash);
      }
    }

    private static string Obfuscate(string str)
    {
      var sb         = new StringBuilder();
      var hashLength = str.Length;

      for (int i = 0; i < hashLength; i++)
      {
        var c    = str[i];
        var newc = c;

        if (c is >= 'a' and <= 'm' or >= 'A' and <= 'M')
          newc = (char)(c + CR);

        else if (c is >= 'n' and <= 'z' or >= 'N' and <= 'Z')
          newc = (char)(c - CR);

        sb.Append(newc);
      }

      return sb.ToString();
    }

    private static string Decode(string str)
    {
      var obfstr = Obfuscate(str);

      byte[] base64EncodedBytes = Convert.FromBase64String(obfstr);

      return Encoding.UTF8.GetString(base64EncodedBytes);
    }

    //private static string CalculateHmac(Uri url, string method)
    //{
    //  method = method.ToUpperInvariant();

    //  var segments = url.Segments.Where(
    //    s => string.IsNullOrWhiteSpace(s) == false && s.Length > 0);
    //  var encodedQuery = url.Query;

    //  var path         = CreatePath(segments);

    //  if (encodedQuery.Length <= 1)
    //    encodedQuery = string.Empty;
      
    //  // Write body

    //  return "";
    //}

    #endregion
  }
}
