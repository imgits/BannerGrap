using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Collections.Specialized;
using System.IO.Compression;

namespace bannergrap
{
    class HttpScanner : WebClient
    {
        HttpBanner banner = null;
        int timeout = 1000;
        public HttpBanner GetBanner(UInt32 ip, UInt16 port, int timeout)
        {
            string url = "http://" + IPHelper.ntoa(ip) + ":" + port + "/";
            this.timeout = timeout;
            byte[] body = null;
            try
            {
                body = DownloadData(url);
            }
            catch (Exception ex)
            {
            }
            if (ResponseHeaders==null)
            {
                return null;
            }
            banner = new HttpBanner(ip, port);
            if (ResponseHeaders != null)
            {
                foreach (string name in ResponseHeaders.AllKeys)
                {
                    banner.headers[name] = ResponseHeaders[name];
                }
            }
            if (body != null)
            {
                banner.body_data = body;
                banner.body_text = this.Encoding.GetString(body);
            }
            return banner;
        }

        protected override WebRequest GetWebRequest(Uri address)
        {
            HttpWebRequest request = (HttpWebRequest)base.GetWebRequest(address);
            request.Timeout = timeout;
            request.ReadWriteTimeout = timeout;
            return request;
        }

    }


}
