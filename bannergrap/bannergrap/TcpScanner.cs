using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace bannergrap
{
    class TcpScanner : TcpClient , IBannerScanner
    {
        protected bool Connect(UInt32 ip, UInt16 port, int timeout)
        {
            IPAddress hostname = IPHelper.AddressH(ip);
            try
            {
                var result = BeginConnect(hostname, port, null, null);
                result.AsyncWaitHandle.WaitOne(timeout);
                return (result.IsCompleted && Connected);
            }
            catch (Exception ex) { }
            return false;
        }

        virtual public BannerBase GetBanner(UInt32 ip, UInt16 port, int timeout)
        {
            if (!Connect(ip, port, timeout)) return null;

            TcpBanner banner = new TcpBanner(ip, port);
            try
            {
                this.ReceiveTimeout = timeout;
                using (NetworkStream ns = GetStream())
                {
                    byte[] buffer = new byte[1024 * 16];
                    int size = ns.Read(buffer, 0, buffer.Length);
                    if (size > 0)
                    {
                        banner.banner_data = new byte[size];
                        Buffer.BlockCopy(buffer, 0, banner.banner_data, 0, size);
                        banner.banner_text = Encoding.ASCII.GetString(buffer, 0, size);
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return banner;
        }
    }
}
