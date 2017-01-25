using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bannergrap
{
    class SshBanner :BannerBase
    {
        public List<string> CompressionAlgorithms=new List<string>();
        public List<string> EncryptionAlgorithms = new List<string>();
        public List<string> HostKeyAlgorithmsAlgorithms = new List<string>();
        public List<string> HmacAlgorithms = new List<string>();
        public List<string> KeyExchangeAlgorithms = new List<string>();

        public string ServerVersion;
        public string message;

        public string HostKeyName;
        public int KeyLength;
        public byte[] HostKey;
        public byte[] FingerPrint;
        public SshBanner(UInt32 ip, UInt16 port) : base(ip,port)
        {
            
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(base.ToString() + "\r\n");
            sb.Append(ServerVersion + "\r\n");
            AddListString(sb, "Compression Algorithms" , CompressionAlgorithms);
            AddListString(sb, "Encryption Algorithms" , EncryptionAlgorithms);
            AddListString(sb, "Hmac Algorithms" , HmacAlgorithms);
            AddListString(sb, "Host KeyAlgorithms Algorithms" , HostKeyAlgorithmsAlgorithms);
            AddListString(sb, "KeyExchange Algorithms" , KeyExchangeAlgorithms);
            return sb.ToString();
        }

        void AddListString(StringBuilder sb, string name, List<string>list)
        {
            sb.Append(name + ":\r\n");
            foreach(string item in list)
            {
                sb.Append("\t" + item + "\r\n");
            }
        }

    }
}
