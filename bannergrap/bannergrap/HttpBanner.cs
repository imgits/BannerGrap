using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bannergrap
{
    class HttpBanner :BannerBase
    {
        public string response_headers;
        public string cookies;
        public byte[] body_raw;
        public string body_text;

        public HttpBanner(UInt32 ip, UInt16 port) : base(ip, port,"HTTP")
        {

        }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}
