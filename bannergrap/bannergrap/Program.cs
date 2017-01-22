using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

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
            Scanner scanner = new Scanner();
            scanner.AddItem(0x7f000001, 80);
            scanner.AddItem("127.0.0.1", 80);
        }
    }
}
