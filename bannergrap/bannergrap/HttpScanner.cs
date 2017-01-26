using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Collections.Specialized;
using System.IO.Compression;
using System.Text.RegularExpressions;

namespace bannergrap
{
    class HttpScanner : WebClient
    {
        HttpWebResponse HttpResponse = null;
        HttpBanner banner = null;
        int timeout = 1000;
        public HttpBanner GetBanner(UInt32 ip, UInt16 port, int timeout)
        {
            //string url = "http://" + IPHelper.ntoa(ip) + ":" + port + "/";
            string url = "http://192.168.1.1:80/";
            this.timeout = timeout;
            byte[] body = null;
            try
            {
                body = DownloadData(url);
            }
            catch (Exception ex)
            {

            }
            banner = new HttpBanner(ip, port);
            banner.raw_data = body;
            foreach (string name in HttpResponse.Headers.AllKeys)
            {
                if (name.ToLower() == "set-cookie")
                {
                    banner.cookies = name + ": " + HttpResponse.Headers[name];
                }
                else
                {
                    banner.response_headers += name + ": " + HttpResponse.Headers[name] + "\r\n";
                }
            }
            return banner;

        }

        protected override WebRequest GetWebRequest(Uri address)
        {
            HttpWebRequest request = (HttpWebRequest)base.GetWebRequest(address);
            request.Timeout = timeout;
            request.ReadWriteTimeout = timeout;
            return request;
        }

        public HttpBanner GetBanner1(UInt32 ip, UInt16 port, int timeout)
        {
            //string url = "http://" + IPHelper.ntoa(ip) + ":" + port + "/";
            string url = "http://192.168.1.1:80/";
            try
            {
                Dispose();
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.UserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; SV1; .NET CLR 1.1.4322; .NET CLR 2.0.50727)";
                request.Method = "GET";
                request.Timeout = timeout;
                request.ReadWriteTimeout = timeout;
                //request.CookieContainer = new CookieContainer();
                HttpResponse = (HttpWebResponse)request.GetResponse();
            }
            catch (WebException ex)
            {
                HttpResponse = (HttpWebResponse)ex.Response;
            }
            catch (Exception ex) { }
            if (HttpResponse == null) return null;
            banner = new HttpBanner(ip, port);
            foreach (string name in HttpResponse.Headers.AllKeys)
            {
                if (name.ToLower() == "set-cookie")
                {
                    banner.cookies =  name + ": " + HttpResponse.Headers[name];
                }
                else
                {
                    banner.response_headers += name + ": " + HttpResponse.Headers[name] + "\r\n";
                }
            }
            Stream ResponseStream = null;
            try
            {
                ResponseStream = HttpResponse.GetResponseStream();
                if (HttpResponse.ContentEncoding.ToLower().Contains("gzip"))
                {
                    ResponseStream = new GZipStream(ResponseStream, CompressionMode.Decompress);
                }
                else if (HttpResponse.ContentEncoding.ToLower().Contains("deflate"))
                {
                    ResponseStream = new DeflateStream(ResponseStream, CompressionMode.Decompress);
                }
                int read_bytes = 0;
                byte[] buffer = new byte[ResponseStream.Length];
                while(read_bytes < ResponseStream.Length)
                {
                    int size = ResponseStream.Read(buffer, read_bytes, (int)ResponseStream.Length - read_bytes);
                    if (size <= 0) break;
                    read_bytes += size;
                }
                banner.body_raw = buffer;
                banner.body_text = Encoding.UTF8.GetString(buffer, 0, read_bytes);
            }
            catch (Exception ex)
            {
                
            }
            if (ResponseStream != null) ResponseStream.Close();
            return banner;
        }

        public void Dispose()
        {
            if (HttpResponse != null)
            {
                HttpResponse.Close();
                HttpResponse = null;
            }
        }


        /// <summary>
        /// 获取网页源代码方法
        /// </summary>
        /// <param name=”url”>地址</param>
        /// <param name=”charSet”>指定编码，如果为空，则自动判断</param>
        /// <param name=”out_str”>网页源代码</param>
        public static string GetHtml(string url, string charSet)
        {
            string strWebData = string.Empty;
            try
            {
                WebClient myWebClient = new WebClient(); //创建WebClient实例
                byte[] myDataBuffer = myWebClient.DownloadData(url);
                strWebData = System.Text.Encoding.Default.GetString(myDataBuffer);
                //获取网页字符编码描述信息
                if (string.IsNullOrEmpty(charSet))
                {
                    Match charSetMatch = Regex.Match(strWebData, "<meta([^>]*)charset =(\")?(.*)?\"", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                    string webCharSet = charSetMatch.Groups[3].Value.Trim().ToLower();
                    if (webCharSet != "gb2312")
                    {
                        webCharSet = "utf-8";
                    }
                    if (System.Text.Encoding.GetEncoding(webCharSet) != System.Text.Encoding.Default)
                    {
                        strWebData = System.Text.Encoding.GetEncoding(webCharSet).GetString(myDataBuffer);
                    }
                }
            }
            catch (Exception ex)
            {
                return null;
            }
            return strWebData;
        }

    }


}
