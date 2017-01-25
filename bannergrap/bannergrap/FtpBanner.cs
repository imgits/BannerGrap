using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bannergrap
{
    class FtpBanner : BannerBase
    {
        public string welcome;
        public string server;
        public bool anonymous;
        public string message;
        public FtpBanner(UInt32 ip, UInt16 port) : base(ip,port)
        {

        }

        override public string ToString()
        {
            return grap_time.ToLongDateString() + " " + ip_str + ":" + port + "\r\n" 
                + welcome + message + "anonymous:" + anonymous + "\r\n";
        }
    }
}
