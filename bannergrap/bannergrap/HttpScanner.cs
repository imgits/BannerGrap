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
    class HttpScanner : IDisposable
    {
        HttpWebResponse HttpResponse = null;
        public bool Connect(UInt32 ip, UInt16 port, int timeout, bool is_https=false)
        {
            string url = null;
            if (is_https)
            {
                url = "https://" + IPHelper.ntoa(ip) + ":" + port + "/";
                ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            }
            else
            {
                url = "http://" + IPHelper.ntoa(ip) + ":" + port + "/";
            }
            try
            {
                Dispose();
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.UserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; SV1; .NET CLR 1.1.4322; .NET CLR 2.0.50727)";
                request.Method = "GET";
                request.Timeout = timeout;
                request.ReadWriteTimeout = timeout;
                request.CookieContainer = new CookieContainer();
                HttpResponse = (HttpWebResponse)request.GetResponse();
                return true;
            }
            catch (WebException ex)
            {
                if (ex.Status == WebExceptionStatus.ProtocolError && ex.Response != null)
                {
                    HttpResponse = (HttpWebResponse)ex.Response;
                    return true;
                }
            }
            catch (InvalidOperationException ex)
            {

            }
            catch (Exception ex)
            {
            }
            return false;
        }

        public string GetBanner(int timeout)
        {

            StringBuilder banner = new StringBuilder();
            foreach (string name in HttpResponse.Headers.AllKeys)
            {
                banner.Append(name + ": " + HttpResponse.Headers[name] + "\r\n");
            }
            if (HttpResponse.Cookies.Count > 0) banner.Append("Set-Cookies:\r\n");
            foreach (Cookie cookie in HttpResponse.Cookies)
            {
                banner.Append(cookie.Name + "=" + cookie.Value + ";\r\n");
            }
            try
            {
                Stream ResponseStream = HttpResponse.GetResponseStream();
                if (HttpResponse.ContentEncoding.ToLower().Contains("gzip"))
                {
                    ResponseStream = new GZipStream(ResponseStream, CompressionMode.Decompress);
                }
                else if (HttpResponse.ContentEncoding.ToLower().Contains("deflate"))
                {
                    ResponseStream = new DeflateStream(ResponseStream, CompressionMode.Decompress);
                }
                Encoding encoding = Encoding.UTF8;
                if (HttpResponse.CharacterSet != null)
                {
                    //encoding = Encoding.GetEncoding(HttpResponse.CharacterSet);
                    banner.Append("HttpResponse.CharacterSet=" + HttpResponse.CharacterSet + "\n");
                }
                else
                {
                    //encoding = Encoding.UTF8;
                    banner.Append("HttpResponse.CharacterSet=null\n");
                }
                using (System.IO.StreamReader reader = new System.IO.StreamReader(ResponseStream, encoding))
                {
                    string body = reader.ReadToEnd();
                    banner.Append(body);
                }
            }
            catch (Exception ex)
            {
                banner.Append(ex.Message);
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
