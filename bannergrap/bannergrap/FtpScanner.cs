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
        FtpBanner banner = null;
        public FtpBanner GetBanner(UInt32 ip, UInt16 port, int timeout)
        {
            banner = new FtpBanner(ip, port);
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
            }
            catch (WebException ex)
            {
                FtpResponse = (FtpWebResponse)ex.Response;
            }
            catch (Exception ex) { }
            if (FtpResponse == null) return null;
            banner.text = FtpResponse.BannerMessage;
            switch(FtpResponse.StatusCode)
            {
                case FtpStatusCode.DataAlreadyOpen:
                case FtpStatusCode.OpeningData:
                    banner.anonymous = true;
                    break;
                default:
                    banner.anonymous = false;
                    break;
            }
            banner.text += FtpResponse.StatusDescription + FtpResponse.WelcomeMessage;
            return banner;
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
