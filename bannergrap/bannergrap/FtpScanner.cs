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
        FtpWebResponse FtpResponse = null;
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
                ftp.Timeout = timeout;
                ftp.ReadWriteTimeout = timeout;
                FtpResponse = (FtpWebResponse)ftp.GetResponse();
                return true;
            }
            catch(WebException ex)
            {
                if (ex.Status == WebExceptionStatus.ProtocolError && ex.Response!=null)
                {
                    FtpResponse = (FtpWebResponse)ex.Response;
                    return true;
                }
            }
            catch(InvalidOperationException ex)
            {

            }
            catch (Exception ex)
            {
                return false;
            }
            return false;
        }

        public string GetBanner(int timeout)
        {
            StringBuilder banner = new StringBuilder();
            if (FtpResponse.BannerMessage!=null)
            {
                banner.Append(FtpResponse.BannerMessage);
            }
            if (FtpResponse.StatusDescription != null)
            {
                banner.Append(FtpResponse.StatusDescription);
            }
            using (StreamReader reader = new StreamReader(FtpResponse.GetResponseStream(), Encoding.Default))
            {
                try
                {
                    string line = reader.ReadLine();
                    while (line != null)
                    {
                        banner.Append(line);
                        line = reader.ReadLine();
                    }
                }
                catch(Exception ex)
                {
                    banner.Append(ex.Message);
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
