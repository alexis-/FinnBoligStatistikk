using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FBS.Scrapper.Utilities
{
  public class FinnHttpClient : HttpClient
  {
    /// <inheritdoc />
    public FinnHttpClient(HttpMessageHandler handler) : base(handler) { }
  }
}
