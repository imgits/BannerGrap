using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace bannergrap
{
    class ScanItem
    {
        public UInt32 ip { get; set; }
        public UInt16 port { get; set; }
        public string result { get; set; }
    }

    class BannerScanner
    {
        List<ScanItem> scan_list = new List<ScanItem>();
        int scan_index = 0;
        int Timeout = 1000;
        CancellationToken ScanCancellationToken;
        public void AddItem(UInt32 ip, UInt16 port)
        {
            scan_list.Add(new ScanItem() { ip = ip, port = port });
        }

        public void AddItem(string strip, UInt16 port)
        {
            UInt32 ip = IPHelper.aton(strip);
            if (ip != 0)
            {
                scan_list.Add(new ScanItem() { ip = ip, port = port });
            }
        }

        ScanItem GetNextItem()
        {
            lock (scan_list)
            {
                if (scan_index >= scan_list.Count) return null;
                ScanItem item = scan_list[scan_index++];
                return item;
            }
        }

        public void StartScan(int threads, int timeout)
        {
            Timeout = timeout;
            CancellationTokenSource cts = new CancellationTokenSource();
            ScanCancellationToken = cts.Token;
            if (threads > scan_list.Count) threads = scan_list.Count;
            Thread[] ScanThreads = new Thread[threads];
            for (int i = 0; i < threads;i++)
            {
                ScanThreads[i] = new Thread(PortScanThread);
                ScanThreads[i].Start(i);
            }
            foreach(Thread ts in ScanThreads)
            {
                ts.Join();
            }
        }

        void PortScanThread(object oid)
        {
            int tid = (int)oid;
            ScanItem item = GetNextItem();
            while (item != null)
            {
                switch(item.port)
                {
                    case 21:    FtpScan(item.ip, item.port);    break;
                    case 22:    SshScan(item.ip, item.port);    break;
                    case 23:    TelnetScan(item.ip, item.port); break;
                    case 25:    SmtpScan(item.ip, item.port);   break;
                    case 80:    HttpScan(item.ip, item.port);   break;
                    case 110:   Pop3Scan(item.ip, item.port);   break;
                    case 443:   HttpsScan(item.ip, item.port);  break;
                    default:
                        TcpScan(item.ip, item.port);
                        break;
                }
                if (ScanCancellationToken.IsCancellationRequested) break;
                item = GetNextItem();
            }
        }

        void TcpScan(UInt32 ip, UInt16 port)
        {
            byte[] buffer = new byte[1024 * 16];
            using (TcpScanner scanner = new TcpScanner())
            {
                if (scanner.Connect(ip,port,Timeout))
                {
                    string banner = scanner.GetBanner(Timeout);
                    OutputBanner(ip, port, banner);
                }
                scanner.Close();
            }
        }

        void FtpScan(UInt32 ip, UInt16 port)
        {
            //TcpScan(ip, port);
            using (FtpScanner scanner = new FtpScanner())
            {
                if (scanner.Connect(ip, port, Timeout))
                {
                    string banner = scanner.GetBanner(Timeout);
                    OutputBanner(ip, port, banner);
                }
            }
        }

        void SshScan(UInt32 ip, UInt16 port)
        {

        }

        void TelnetScan(UInt32 ip, UInt16 port)
        {

        }

        void SmtpScan(UInt32 ip, UInt16 port)
        {

        }

        void HttpScan(UInt32 ip, UInt16 port)
        {
            //TcpScan(ip, port);
            using (HttpScanner scanner = new HttpScanner())
            {
                if (scanner.Connect(ip, port, Timeout))
                {
                    string banner = scanner.GetBanner(Timeout);
                    OutputBanner(ip, port, banner);
                }
            }
        }

        void Pop3Scan(UInt32 ip, UInt16 port)
        {

        }

        void HttpsScan(UInt32 ip, UInt16 port)
        {
            using (HttpScanner scanner = new HttpScanner())
            {
                if (scanner.Connect(ip, port, Timeout,true))
                {
                    string banner = scanner.GetBanner(Timeout);
                    OutputBanner(ip, port, banner);
                }
            }
        }



        void OutputBanner(UInt32 ip, UInt16 port, string banner)
        {
            string host = IPHelper.ntoa(ip);
            if (banner == null)
            {
                Console.Write(host + ":" + port + " opened\n");
            }
            else
            {
                Console.Write(host + ":" + port + "\n" + banner + "\n");
            }
        }


        void OutputBanner(UInt32 ip, UInt16 port, byte[] buffer, int size)
        {
            string host = IPHelper.ntoa(ip);
            if (buffer ==null || size ==0)
            {
                Console.Write(host + ":" + port + " opened\n");
            }
            else
            {
                string banner = Encoding.ASCII.GetString(buffer, 0, size);
                Console.Write(host + ":" + port + "\n" + banner + "\n");
            }
        }

    }
}
