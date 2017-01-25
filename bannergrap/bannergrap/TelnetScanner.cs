using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace bannergrap
{
    class TelnetScanner : TcpClient
    {
        TelnetBanner banner = null;
        public TelnetBanner GetBanner(UInt32 ip, UInt16 port, int timeout)
        {
            banner = new TelnetBanner(ip, port);
            //ip = 0x7f000001;
            IPAddress hostname = IPHelper.AddressH(ip);
            IAsyncResult result=null;
            NetworkStream stream = null;
            try
            {
                result = BeginConnect(hostname, port, null, null);
                result.AsyncWaitHandle.WaitOne(timeout);
                if (result.IsCompleted && Connected)
                {
                    stream = this.GetStream();
                    byte[] buffer=new byte[1024 * 4];
                    int size = stream.Read(buffer, 0, buffer.Length);
                    for (int i=0; i < size; )
                    {
                        if (buffer[i]==255)
                        {
                            switch (buffer[i+1])
                            {
                                case 236: break;
                                case 250:
                                //TELNET选项协商
                                case 251:
                                case 252:
                                case 253:
                                case 254:
                                    i += 3;
                                    break;

                            }
                        }
                        
                    }
                    banner.Welcome = Encoding.ASCII.GetString(buffer, 0, size);
                    return banner;
                }
                
            }
            catch (Exception ex) { }
            if (stream != null) stream.Close();
            return null;
        }
    }
}
