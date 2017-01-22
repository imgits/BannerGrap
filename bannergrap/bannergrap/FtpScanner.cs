using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;

namespace bannergrap
{
    class FtpScanner : IDisposable
    {
        WebResponse   FtpResponse = null;
        public bool   Connect(UInt32 ip, UInt16 port, int timeout)
        {
            string url = "ftp://" + IPHelper.ntoa(ip) + "/";
            try
            {
                Dispose();
                FtpWebRequest ftp = (FtpWebRequest)FtpWebRequest.Create(new Uri(url));
                ftp.UseBinary = true;
                ftp.Credentials = new NetworkCredential("anonymous", "janeDoe@contoso.com");
                ftp.Method = WebRequestMethods.Ftp.ListDirectory;
                FtpResponse = ftp.GetResponse();
                return true;
            }
            catch(Exception ex)
            {
            }
            return false;
        }

        public string GetBanner(int timeout)
        {
            StringBuilder banner = new StringBuilder();
            using (StreamReader reader = new StreamReader(FtpResponse.GetResponseStream(), Encoding.Default))
            {
                string line = reader.ReadLine();
                while (line != null)
                {
                    banner.Append(line);
                    line = reader.ReadLine();
                }
            }
            return banner.ToString();
        }

        public void Dispose()
        {
            if (FtpResponse!=null)
            {
                FtpResponse.Close();
                FtpResponse = null;
            }
        }
    }
}
