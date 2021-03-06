﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using MongoDB.Driver;
namespace bannergrap
{
    class ScanItem
    {
        public UInt32 ip { get; set; }
        public UInt16 port { get; set; }
    }

    class BannerScanner
    {
        List<ScanItem> scan_list = new List<ScanItem>();
        int scan_index = 0;
        int Timeout = 1000;
        CancellationToken ScanCancellationToken;
        MongoClient mongo_client = new MongoClient();
        IMongoDatabase mongodb_banners=null;
        IMongoCollection<FtpBanner> table_ftp = null;
        IMongoCollection<SshBanner> table_ssh = null;
        IMongoCollection<BannerBase> table_tcp = null;
        DateTime task_id = DateTime.Now;
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
            mongodb_banners = mongo_client.GetDatabase("banners");
            table_ftp = mongodb_banners.GetCollection<FtpBanner>("ftp");
            table_ssh = mongodb_banners.GetCollection<SshBanner>("ssh");
            table_tcp = mongodb_banners.GetCollection<BannerBase>("tcp");
            //ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

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
            Dictionary<int, IBannerScanner> Scanners = new Dictionary<int, IBannerScanner>();
            Scanners[21]  = new TcpScanner();
            Scanners[22]  = new SshScanner();
            Scanners[23]  = new TelnetScanner();
            //Scanners[25]  = new SnmpScanner();
            //Scanners[53]  = new DnsScanner();
            //Scanners[80]  = new HttpScanner();
            //Scanners[110]  = new Pop3Scanner();
            //Scanners[443]  = new HttpsScanner();
            //Scanners[1436]  = new MssqlScanner();
            //Scanners[3306]  = new MysqlScanner();
            //Scanners[3389]  = new RdpScanner();
            //Scanners[4899]  = new RadminScanner();

            IBannerScanner scanner = null;

            ScanItem item = GetNextItem();
            while (item != null)
            {
                Console.WriteLine("Scanning " + IPHelper.ntoa(item.ip) + ":" + item.port);
                switch(item.port)
                {
                    //case 21: scanner = ftp_scanner;break;
                    //case 21:    FtpScan(item.ip, item.port);    break;
                    //case 22:    SshScan(item.ip, item.port);    break;
                    //case 23:    TelnetScan(item.ip, item.port); break;
                    //case 25:    SmtpScan(item.ip, item.port);   break;
                    //case 80:    HttpScan(item.ip, item.port);   break;
                    //case 110:   Pop3Scan(item.ip, item.port);   break;
                    //case 443:   HttpsScan(item.ip, item.port);  break;
                    //case 3306:  MysqlScan(item.ip, item.port); break;
                        //TcpScan(item.ip, item.port); break;
                    default:      TcpScan(item.ip, item.port);    break;
                }
                if (ScanCancellationToken.IsCancellationRequested) break;
                item = GetNextItem();
            }
        }

         void TcpScan(UInt32 ip, UInt16 port)
        {
            using (TcpScanner scanner = new TcpScanner())
            {
                BannerBase banner = scanner.GetBanner(ip,port,Timeout);
                if (banner !=null)
                {
                    banner.task_id = task_id;
                    table_tcp.InsertOneAsync(banner);
                    OutputBanner(ip, port, banner);
                }
            }
        }

        void FtpScan(UInt32 ip, UInt16 port)
        {
            using (FtpScanner scanner = new FtpScanner())
            {
                BannerBase banner  = scanner.GetBanner(ip, port, Timeout);
                if (banner != null)
                {
                    banner.task_id = task_id;
                    //table_ftp.InsertOneAsync(banner);
                    OutputBanner(ip, port, banner);
                }
            }
        }

        void SshScan(UInt32 ip, UInt16 port)
        {
            using (SshScanner scanner = new SshScanner())
            {
                BannerBase banner = scanner.GetBanner(ip, port, Timeout);
                if (banner != null)
                {
                    banner.task_id = task_id;
                    //table_ssh.InsertOneAsync(banner);
                    OutputBanner(ip, port, banner);
                }
            }
        }

        void TelnetScan(UInt32 ip, UInt16 port)
        {
            using (TelnetScanner scanner = new TelnetScanner())
            {
                BannerBase banner = scanner.GetBanner(ip, port, Timeout);
                if (banner != null)
                {
                    banner.task_id = task_id;
                    //table_ssh.InsertOneAsync(banner);
                    OutputBanner(ip, port, banner);
                }
            }
        }

        void SmtpScan(UInt32 ip, UInt16 port)
        {

        }

        void HttpScan(UInt32 ip, UInt16 port)
        {
            using (HttpScanner scanner = new HttpScanner())
            {
                HttpBanner banner = scanner.GetBanner(ip, port, Timeout);
                if (banner != null)
                {
                    banner.task_id = task_id;
                    //table_ssh.InsertOneAsync(banner);
                    OutputBanner(ip, port, banner);
                }
            }
        }

        void Pop3Scan(UInt32 ip, UInt16 port)
        {

        }

        void HttpsScan(UInt32 ip, UInt16 port)
        {
            using (HttpsScanner scanner = new HttpsScanner())
            {
                HttpsBanner banner = scanner.GetBanner(ip, port, Timeout);
                if (banner != null)
                {
                    banner.task_id = task_id;
                    //table_ssh.InsertOneAsync(banner);
                    OutputBanner(ip, port, banner);
                }
            }
        }


        void MysqlScan(UInt32 ip, UInt16 port)
        {
            using (MysqlScanner client = new MysqlScanner())
            {
                MysqlBanner banner = client.GetBanner(ip,port,Timeout);
                if (banner != null)
                {
                    banner.task_id = task_id;
                    //table_mysql.InsertOneAsync(banner);
                    OutputBanner(ip, port, banner);
                }
            }
        }

        void OutputBanner(UInt32 ip, UInt16 port, BannerBase banner)
        {
            if (banner!=null)
            {
                Console.WriteLine(banner.ToString());
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
