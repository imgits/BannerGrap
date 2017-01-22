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

    class Scanner
    {
        List<ScanItem> scan_list = new List<ScanItem>();
        int scan_index = 0;
        int Timeout = 1000;
        CancellationTokenSource cts = new CancellationTokenSource();
        CancellationToken ScanCancellationToken = new CancellationToken();
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
            while(item!=null)
            {
                using (TcpClient tcpClient = new TcpClient())
                {

                    IPAddress hostname = IPHelper.Address(item.ip);
                    if (tcpClient.ConnectAsync(hostname, item.port).Wait(Timeout, ScanCancellationToken))
                    {
                        using (NetworkStream ns = tcpClient.GetStream())
                        {
                            if (ns.ReadAsync())
                        }
                    }
                    tcpClient.Close();
                }
                if (ScanCancellationToken.IsCancellationRequested) break;
                item = GetNextItem();
            }
        }

    }
}
