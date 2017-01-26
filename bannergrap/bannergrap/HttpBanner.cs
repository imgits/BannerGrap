using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bannergrap
{
    class HttpBanner :BannerBase
    {
        public IDictionary<string, string> headers;
        public byte[] body_data;
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
