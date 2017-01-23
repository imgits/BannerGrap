using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace bannergrap
{
    class HttpsScanner : WebClient
    {
        string banner = null;
        public bool Connect(UInt32 ip, UInt16 port, int timeout)
        {
            string url = "https://" + IPHelper.ntoa(ip) + ":" + port + "/";
            try
            {
                ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                banner = this.DownloadString(url);
                return true;
            }
            catch (Exception ex)
            {
            }
            return false;
        }

        public string GetBanner(int timeout)
        {
            return banner;
        }
    }
}
