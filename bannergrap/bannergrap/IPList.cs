using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
namespace bannergrap
{
    class IPRange
    {
        public UInt32 start { get; set; } 
        public UInt32 end { get; set; }
        public UInt32 size { get; set; }
        public IPRange(UInt32 ip)
        {
            start = ip;
            size = 1;
            end = ip + size;
        }

        public IPRange(UInt32 ip, UInt32 count)
        {
            start = ip;
            size = count;
            end = ip + size;
        }
    }

    class IPList
    {

        List<IPRange> iplist = new List<IPRange>();
        /// <summary>
        /// 将IPv4格式的字符串转换为int型表示
        /// </summary>
        /// <param name="strIPAddress">IPv4格式的字符</param>
        /// <returns></returns>
        UInt32 aton(string strIPAddress)
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
            UInt32 ip32=(ip1 << 24) + (ip2 << 16) + (ip3 << 8) + ip4;
            return ip32;
        }

        /// <summary>
        /// 将int型表示的IP还原成正常IPv4格式。
        /// </summary>
        /// <param name="intIPAddress">int型表示的IP</param>
        /// <returns></returns>
        string ntoa(UInt32 ip)
        {
            UInt32 s1 = (ip >> 24) & 0xff;
            UInt32 s2 = (ip >> 16) & 0xff;
            UInt32 s3 = (ip >> 8) & 0xff;
            UInt32 s4 = (ip >> 0) & 0xff;
            string strIPAddress = s1.ToString() + "." + s2.ToString() + "." + s3.ToString() + "." + s4.ToString();
            return strIPAddress;
        }

        public bool AddIP(string IPString)
        {
            UInt32 ip32 = aton(IPString);
            if (ip32 == 0) return false;
            iplist.Add(new IPRange(ip32));
            return true;
        }

        public bool AddRange(string strIPRange)
        {
            string[] ips = strIPRange.Split('/');
            if (ips.Length == 2)
            {
                UInt32 ip32 = aton(ips[0]);
                if (ip32 == 0) return false;
            }
            else
            {
                ips = strIPRange.Split('-');
                if (ips.Length == 2)
                {
                    UInt32 ip32 = aton(ips[0]);
                    if (ip32 == 0) return false;
                }
                else
                {
                    return false;
                }
            }
            return false;
        }
    }
}
