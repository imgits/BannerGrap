using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bannergrap
{
    class TcpBanner : BannerBase
    {
        public string tcp_message;
        public TcpBanner(UInt32 ip, UInt16 port) :base(ip,port,"TCP")
        {
            tcp_message = "This is tcp message";
        }
    }
}
