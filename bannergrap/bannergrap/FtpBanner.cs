using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bannergrap
{
    class FtpBanner : BannerBase
    {
        public bool  anonymous;
        public FtpBanner(UInt32 ip, UInt16 port) : base(ip,port,"FTP")
        {

        }

        override public string ToString()
        {
            return base.ToString() + "anonymous:" + anonymous + "\r\n";
        }
    }
}
