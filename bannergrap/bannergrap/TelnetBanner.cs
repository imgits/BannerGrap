using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bannergrap
{
    class TelnetBanner : BannerBase
    {
        public string Welcome;

        public TelnetBanner(UInt32 ip, UInt16 port) : base(ip, port,"telnet")
        {

        }

        public override string ToString()
        {
            return base.ToString() + "\r\n" + Welcome;
        }
    }
}
