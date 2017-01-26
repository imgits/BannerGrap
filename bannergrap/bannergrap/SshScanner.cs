using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Renci.SshNet;
using Renci.SshNet.Security;

namespace bannergrap
{
    class SshScanner : IBannerScanner, IDisposable
    {
        SshBanner banner = null;
        public BannerBase GetBanner(UInt32 ip, UInt16 port, int timeout)
        {
            banner = new SshBanner(ip, port);
            string host = IPHelper.ntoa(ip);
            var client = new SshClient(host, port, "user", "pass");
            try
            {
                client.ConnectionInfo.Timeout = TimeSpan.FromMilliseconds(timeout);
                client.HostKeyReceived += OnHostKeyReceived;
                client.Connect();
            }
            catch(Exception ex)
            {
            }
            ConnectionInfo ci = client.ConnectionInfo;
            if (ci.ServerVersion != null)
            {
                banner.CompressionAlgorithms.AddRange(ci.CompressionAlgorithms.Keys);
                banner.KeyExchangeAlgorithms.AddRange(ci.KeyExchangeAlgorithms.Keys);
                banner.EncryptionAlgorithms.AddRange(ci.Encryptions.Keys);
                banner.HmacAlgorithms.AddRange(ci.HmacAlgorithms.Keys);
                banner.HostKeyAlgorithmsAlgorithms.AddRange(ci.HostKeyAlgorithms.Keys);
                
                banner.ServerVersion = ci.ServerVersion;
            }
            else
            {
                banner = null;
            }
            client.Disconnect();
            return banner;
        }

        //接受公钥
        void OnHostKeyReceived(Object sender,Renci.SshNet.Common.HostKeyEventArgs args)
        {
            banner.HostKeyName = args.HostKeyName;
            banner.KeyLength = args.KeyLength;
            banner.HostKey = args.HostKey;
            banner.FingerPrint =args.FingerPrint;
        }

        public void Dispose()
        {
            banner = null;
        }
    }
}
