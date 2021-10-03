using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TDiary.Web.Models
{
    public class Oidc
    {
        public string Authority { get; set; }
        public string ClientId { get; set; }
        public string[] DefaultScopes { get; set; }
        public string PostLogoutRedirectUri { get; set; }
        public string RedirectUri { get; set; }

    }
}
