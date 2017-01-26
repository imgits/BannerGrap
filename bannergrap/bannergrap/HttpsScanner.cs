using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;

namespace bannergrap
{
    class HttpsScanner : WebClient
    {
        HttpsBanner banner = null;
        int timeout = 1000;
        public HttpsBanner GetBanner(UInt32 ip, UInt16 port, int timeout)
        {
            string url = "https://" + IPHelper.ntoa(ip) + ":" + port + "/";
            this.timeout = timeout;
            byte[] body = null;
            try
            {
                body = DownloadData(url);
            }
            catch (Exception ex)
            {
                Console.WriteLine(IPHelper.ntoa(ip) + ":" + port + " error:" + ex.Message);
            }
            if (ResponseHeaders == null)
            {
                return null;
            }

            banner = new HttpsBanner(ip, port);
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
            request.ServerCertificateValidationCallback = RemoteCertificateValidationCallback;
            return request;
        }

        bool RemoteCertificateValidationCallback(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }

    }
}
