using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TDiary.Web.Models
{
    public class AppSettings
    {
        public Oidc Oidc { get; set; }
        public string Environment { get; set; }
        public string ResponseType { get; set; }
        public Api Api { get; set; }
    }
}
