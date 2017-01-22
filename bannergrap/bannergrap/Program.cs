using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;

namespace bannergrap
{
    class Program
    {
        static void Help()
        {
            Console.Write(
                "PortScan ip|ips port|prtlist output\n" +
                "-ip single ip\n" +
                "-ir ip_range\n"  +
                "-if ip form file\n");
        }

        static void Main(string[] args)
        {
            BannerScanner scanner = new BannerScanner();
            TextReader file = File.OpenText(args[0]);
            do
            {
                string line = file.ReadLine();
                if (line == null) break;
                string[] items = line.Split(' ');
                if (items.Length==5 && items[0]=="open" && items[1] =="tcp")
                {
                    UInt32 ip = IPHelper.aton(items[3]);
                    UInt16 port = UInt16.Parse(items[2]);
                    scanner.AddItem(ip, port);
                }
            }while (true);
            int threads = int.Parse(args[1]);
            int timeout = int.Parse(args[2]);
            scanner.StartScan(threads, timeout);
        }
    }
}
