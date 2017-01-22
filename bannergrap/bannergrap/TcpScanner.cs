using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace bannergrap
{
    class TcpScanner : TcpClient
    {
        public bool Connect(UInt32 ip, UInt16 port, int timeout)
        {
            IPAddress hostname = IPHelper.AddressH(ip);
            var result = BeginConnect(hostname, port, null, null);
            result.AsyncWaitHandle.WaitOne(timeout);
            return (result.IsCompleted && Connected);
        }

        public string GetBanner(int timeout)
        {
            this.ReceiveTimeout = timeout;
            using (NetworkStream ns = GetStream())
            {
                try
                {
                    byte[] buffer = new byte[1024 * 16];
                    int size = ns.Read(buffer, 0, buffer.Length);
                    if (size > 0)
                    {
                        string banner = Encoding.ASCII.GetString(buffer, 0, size);
                        return banner;
                    }
                }
                catch (Exception ex)
                {
                }
            }
            return null;
        }
    }
}
