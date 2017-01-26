using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bannergrap
{
    interface IBannerScanner
    {
        BannerBase GetBanner(UInt32 ip, UInt16 port, int timeout);
    }
}
