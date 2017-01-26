using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace bannergrap
{
    class TelnetScanner : TcpScanner
    {
        const byte NVT_EOF = 236; //文件结束符
        const byte NVT_SUSP = 237;//挂起当前进程
        const byte NVT_ABORT = 238;//中止进程
        const byte NVT_EOR = 239;//记录结束符
        const byte NVT_SE = 240;//子选项结束
        const byte NVT_NOP = 241;// 空操作
        const byte NVT_DM = 242;// 数据标记
        const byte NVT_BRK = 243;//终止符（break）
        const byte NVT_IP = 244;// 终止进程
        const byte NVT_AO = 245;//终止输出
        const byte NVT_AYT = 246;//请求应答
        const byte NVT_EC = 247;//终止符
        const byte NVT_EL = 248;//擦除一行
        const byte NVT_GA = 249;//继续
        const byte NVT_SB = 250;//子选项开始
        const byte NVT_WILL = 251;//选项协商
        const byte NVT_WONT = 252;//选项协商
        const byte NVT_DO = 253;//选项协商
        const byte NVT_DONT = 254;//选项协商
        const byte NVT_IAC = 255;//字符0XFF

        override public BannerBase GetBanner(UInt32 ip, UInt16 port, int timeout)
        {
            if (!Connect(ip, port, timeout)) return null;
            TelnetBanner banner = new TelnetBanner(ip, port);
            try
            {
                this.ReceiveTimeout = timeout;
                using (NetworkStream stream = this.GetStream())
                {
                    byte[] buffer = new byte[1024 * 4];
                    int size = stream.Read(buffer, 0, buffer.Length);
                    if (size > 0)
                    {
                        banner.banner_data = new byte[size];
                        Buffer.BlockCopy(buffer, 0, banner.banner_data, 0, size);
                    }
                    banner.banner_text=ParseNVT(buffer, size);
                    return banner;
                }
            }
            catch (Exception ex) { }
            return null;
        }

        string ParseNVT(byte[] buffer, int size)
        {
            int i = 0;
            while (i < size)
            {
                if (buffer[i] != NVT_IAC) break;
                switch (buffer[i + 1])
                {
                    case 236:
                    case 237:
                    case 238:
                    case 239:
                    case 240:
                    case 241:
                    case 242:
                    case 243:
                    case 244:
                    case 245:
                    case 246:
                    case 247:
                    case 248:
                    case 249:
                        i += 2;
                        break;
                    case 250:
                        i += 4;
                        break;
                    //TELNET选项协商
                    case 251:
                    case 252:
                    case 253:
                    case 254:
                        i += 3;
                        break;
                    default:
                        i += 2;
                        break;
                }
            }
            return  Encoding.UTF8.GetString(buffer, i, size - i);
        }
    }
}
