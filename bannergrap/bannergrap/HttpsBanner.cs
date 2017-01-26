using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bannergrap
{
    class HttpsBanner : BannerBase
    {
        public IDictionary<string,string>headers;
        public byte[] body_data;
        public string body_text;

        public HttpsBanner(UInt32 ip, UInt16 port) : base(ip, port,"HTTPS")
        {

        }

        override public string Text
        {
            get
            {
                return base.ToString();
            }
        }

        override public string ToString()
        {
            return base.ToString();
        }
    }
}
