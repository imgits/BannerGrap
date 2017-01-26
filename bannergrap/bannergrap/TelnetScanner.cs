using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace bannergrap
{
    enum Verbs
    {
        WILL = 251,
        WONT = 252,
        DO = 253,
        DONT = 254,
        IAC = 255
    }

    enum Options
    {
        SGA = 3
    }
    class TelnetScanner : TcpScanner
    {
        // Telnet commands
        const byte TNC_SE = 240; // End of subnegotiation parameters
        const byte TNC_NOP = 241; // No operation
        const byte TNC_DATAMARK = 242; // F2 The data stream portion of a Synch. This should always be accompanied by a TCP Urgent notification. 
        const byte TNC_BRK = 243; // F3 Break NVT character BRK.
        const byte TNC_IP = 244; // F4 Interrupt Process The function IP. 
        const byte TNC_AO = 245; // F5 The function AO. Abort output
        const byte TNC_AYT = 246; // F6 Are You There The function AYT. 
        const byte TNC_EC = 247; // F7 Erase character. The function EC. 
        const byte TNC_EL = 248; // F8 Erase line. The function EL.
        const byte TNC_GA = 249; // F9 Go ahead The GA signal. 
        const byte TNC_SB = 250; // FA Option code: Indicates that what follows is subnegotiation of the indicated option.
        const byte TNC_WILL = 251; // FB Option code: Indicates the desire to begin performing, or confirmation that you are now performing, the indicated option.
        const byte TNC_WONT = 252; // FC Option code: Indicates the refusal to perform, or continue performing, the indicated option.
        const byte TNC_DO = 253; // FD Option code: Indicates the request that the other party perform, or confirmation that you are expecting the other party to perform, the indicated option.
        const byte TNC_DONT = 254; // FE Option code: Indicates the demand that the other party stop performing, or confirmation that you are no longer expecting the other party to perform, the indicated option.
        const byte TNC_IAC = 255; // FF Data Byte 255
                                  // Telnet options
        const byte TNO_TRANSBIN = 0;  // 00 transmit binary
        const byte TNO_ECHO = 1;  // 00 echo
        const byte TNO_LOGOUT = 18; // 12 Logout
        const byte TNO_TERMTYPE = 24; // 18 Terminal size
        const byte TNO_NAWS = 31; // 1F Window size
        const byte TNO_TERMSPEED = 32; // 20 Terminal speed
        const byte TNO_REMOTEFLOW = 33; // 21 Remote flow control
        const byte TNO_XDISPLAY = 35; // 23 X-Display location
        const byte TNO_NEWENV = 39; // 27 New environment option
                                    // TELNET others
        const byte TNX_SEND = 1;  // 01 send, e.g. used with SB terminal type
        const byte TNX_IS = 0;  // 00 is, e.g. used with SB terminal type
                                //#endregion
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

        TelnetBanner banner = null;
        public new TelnetBanner GetBanner(UInt32 ip, UInt16 port, int timeout)
        {
            if (!Connect(ip, port, timeout)) return null;
            banner = new TelnetBanner(ip, port);
            try
            {
                this.ReceiveTimeout = timeout;
                using (NetworkStream stream = this.GetStream())
                {
                    byte[] buffer = new byte[1024 * 4];
                    int size = stream.Read(buffer, 0, buffer.Length);
                    if (size > 0)
                    {
                        banner.raw_data = new byte[size];
                        Buffer.BlockCopy(buffer, 0, banner.raw_data, 0, size);
                    }
                    ParseNVT(buffer, size);
                    return banner;
                }
            }
            catch (Exception ex) { }
            return null;
        }

        void ParseNVT(byte[] buffer, int size)
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
            banner.text = Encoding.UTF8.GetString(buffer, i, size - i);
        }
    }
}
