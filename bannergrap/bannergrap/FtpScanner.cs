using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;

namespace bannergrap
{
    class FtpScanner : IBannerScanner , IDisposable
    {
        public BannerBase GetBanner(UInt32 ip, UInt16 port, int timeout)
        {
            string url = "ftp://" + IPHelper.ntoa(ip) + "/";
            FtpWebResponse FtpResponse = null;
            try
            {
                FtpWebRequest ftp = (FtpWebRequest)FtpWebRequest.Create(new Uri(url));
                ftp.UseBinary = true;
                ftp.Credentials = new NetworkCredential("anonymous", "janeDoe@contoso.com");
                ftp.Method = WebRequestMethods.Ftp.ListDirectory;
                ftp.Timeout = timeout;
                ftp.ReadWriteTimeout = timeout;
                FtpResponse = (FtpWebResponse)ftp.GetResponse();
            }
            catch (WebException ex)
            {
                FtpResponse = (FtpWebResponse)ex.Response;
            }
            catch (Exception ex) { }
            if (FtpResponse == null) return null;

            FtpBanner banner = new FtpBanner(ip, port);
            banner.banner_text = FtpResponse.BannerMessage;
            banner.status_description = FtpResponse.StatusDescription;
            banner.welcome_message = FtpResponse.WelcomeMessage;

            switch (FtpResponse.StatusCode)
            {
                case FtpStatusCode.DataAlreadyOpen:
                case FtpStatusCode.OpeningData:
                    banner.anonymous = true;
                    break;
                default:
                    banner.anonymous = false;
                    break;
            }
            FtpResponse.Close();
            return banner;
        }

        public void Dispose()
        {
        }
    }
}
