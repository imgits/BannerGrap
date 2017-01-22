using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bannergrap
{
    enum  SeviceName
    {
        FTP,TELNET,SMTP,HTTP,POP3,
    }
    class BannerGrapper
    {
        public string Grap(UInt32 ip, UInt32 port, string service=null)
        {
            if (service!=null)
            {
                switch(service)
                {
                    default:
                        break;
                }
            }
            else
            {
                switch(port)
                {
                    default:
                        break;
                }
            }
            return null;
        }
    }
}
