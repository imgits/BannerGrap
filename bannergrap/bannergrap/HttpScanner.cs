using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;

namespace bannergrap
{
    class HttpScanner : IDisposable
    {
        HttpResponseMessage HttpResponse = null;
        HttpClient client = new HttpClient();
        public bool Connect(UInt32 ip, UInt16 port, int timeout)
        {
            string url = "ftp://" + IPHelper.ntoa(ip) + "/";
            client.BaseAddress = new Uri(url);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/html"));
            client.Timeout = new TimeSpan((long)timeout*10000); //There are 10,000 ticks in a millisecond
            try
            {
                HttpResponse = await client..GetAsync("/");
                return true;
            }
            catch (Exception ex)
            {
            }
            return false;
        }

        public string GetBanner(int timeout)
        {
            StringBuilder banner = new StringBuilder();
            using (StreamReader reader = new StreamReader(HttpResponse.GetResponseStream(), Encoding.Default))
            {
                string line = reader.ReadLine();
                while (line != null)
                {
                    banner.Append(line);
                    line = reader.ReadLine();
                }
            }
            return banner.ToString();
        }

        public void Dispose()
        {
            if (HttpResponse != null)
            {
                HttpResponse.Close();
                HttpResponse = null;
            }
        }

    }


}
