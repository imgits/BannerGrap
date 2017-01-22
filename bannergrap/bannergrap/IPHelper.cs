using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace bannergrap
{
    class IPHelper
    {
        /// <summary>
        /// 将IPv4格式的字符串转换为int型表示
        /// </summary>
        /// <param name="strIPAddress">IPv4格式的字符</param>
        /// <returns></returns>
        public static UInt32 aton(string strIPAddress)
        {
            //将目标IP地址字符串strIPAddress转换为数字
            IPAddress ip = null;
            if (!IPAddress.TryParse(strIPAddress, out ip))
            {
                return 0;
            }
            string[] arrayIP = strIPAddress.Split('.');
            UInt32 ip1 = UInt32.Parse(arrayIP[0]);
            UInt32 ip2 = UInt32.Parse(arrayIP[1]);
            UInt32 ip3 = UInt32.Parse(arrayIP[2]);
            UInt32 ip4 = UInt32.Parse(arrayIP[3]);
            UInt32 ip32 = (ip1 << 24) + (ip2 << 16) + (ip3 << 8) + ip4;
            return ip32;
        }

        /// <summary>
        /// 将int型表示的IP还原成正常IPv4格式。
        /// </summary>
        /// <param name="intIPAddress">int型表示的IP</param>
        /// <returns></returns>
        public static string ntoa(UInt32 ip)
        {
            UInt32 s1 = (ip >> 24) & 0xff;
            UInt32 s2 = (ip >> 16) & 0xff;
            UInt32 s3 = (ip >> 8) & 0xff;
            UInt32 s4 = (ip >> 0) & 0xff;
            string strIPAddress = s1.ToString() + "." + s2.ToString() + "." + s3.ToString() + "." + s4.ToString();
            return strIPAddress;
        }

        public static UInt32 ntohl(UInt32 ip)
        {
            UInt32 ip1 = (ip >> 24) & 0xff;
            UInt32 ip2 = (ip >> 16) & 0xff;
            UInt32 ip3 = (ip >> 8) & 0xff;
            UInt32 ip4 = (ip >> 0) & 0xff;
            UInt32 ip32 = (ip4 << 24) + (ip3 << 16) + (ip2 << 8) + ip1;
            return ip32;
        }

        public static UInt32 htonl(UInt32 ip)
        {
            UInt32 ip1 = (ip >> 24) & 0xff;
            UInt32 ip2 = (ip >> 16) & 0xff;
            UInt32 ip3 = (ip >> 8) & 0xff;
            UInt32 ip4 = (ip >> 0) & 0xff;
            UInt32 ip32 = (ip4 << 24) + (ip3 << 16) + (ip2 << 8) + ip1;
            return ip32;
        }

        public static IPAddress AddressN(UInt32 ip)
        {
            IPAddress ipaddr = new IPAddress(ip);
            return ipaddr;
        }

        public static IPAddress AddressH(UInt32 ip)
        {
            ip = htonl(ip);
            IPAddress ipaddr = new IPAddress(ip);
            return ipaddr;
        }
    }
}
