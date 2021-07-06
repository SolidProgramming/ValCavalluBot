using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Shares
{
    public static class Connection
    {
        static CookieCollection Cookies = new CookieCollection();

        public static string Post(string URL, string Post)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL);
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/63.0.3239.132 Safari/537.36";//"Mozilla/5.0 (Windows; U; WIndows NT 6.1; ru; rv:1.9.2.3) Gecko/20100401 Firefox/4.0 (.NET CLR 3.5.30729)";
                request.CookieContainer = new CookieContainer();
                foreach (Cookie entry in Cookies)
                {
                    request.CookieContainer.Add(entry);
                }
                request.AllowAutoRedirect = true;
                // Wenn wir request sagen, das wir KEINEN proxy haben, sucht er keinen und die Anfrage geht um einiges schneller
                request.Proxy = null;

                // Post-Daten definieren und abschicken
                string PostData = Post;
                byte[] byteArray = Encoding.Default.GetBytes(PostData);
                request.ContentLength = byteArray.Length;
                Stream DataStream = request.GetRequestStream();
                DataStream.Write(byteArray, 0, byteArray.Length);
                DataStream.Close();

                // Rückgabe holen
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                DataStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(DataStream);
                string ServerResponse = reader.ReadToEnd();
                reader.Close();
                DataStream.Close();

                foreach (Cookie cook in response.Cookies)
                {
                    //MessageBox.Show(Convert.ToString(cook));
                    Cookies.Add(cook);
                }

                response.Close();

                return ServerResponse;
            }
            catch (Exception)
            {
                return null;
            }


        }

        public static string Get(string URL)
        {

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL);
                request.Method = "GET";
                request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/63.0.3239.132 Safari/537.36";//"Mozilla/5.0 (Windows; U; WIndows NT 6.1; ru; rv:1.9.2.3) Gecko/20100401 Firefox/4.0 (.NET CLR 3.5.30729)";
                request.CookieContainer = new CookieContainer();
                foreach (Cookie entry in Cookies)
                {
                    request.CookieContainer.Add(entry);
                }
                request.AllowAutoRedirect = true;
                request.Proxy = null;
                Stream DataStream = default(Stream);
                // Rückgabe holen
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                DataStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(DataStream);
                string ServerResponse = reader.ReadToEnd();

                foreach (Cookie cook in response.Cookies)
                {
                    Cookies.Add(cook);
                }

                reader.Close();
                DataStream.Close();
                response.Close();


                return ServerResponse;
            }
            catch (Exception)
            {
                return null;
            }

        }

        public static string Get(string URL, string cookName, out Cookie outCook)
        {
            outCook = null;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL);
            request.Method = "GET";
            request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/63.0.3239.132 Safari/537.36";//"Mozilla/5.0 (Windows; U; WIndows NT 6.1; ru; rv:1.9.2.3) Gecko/20100401 Firefox/4.0 (.NET CLR 3.5.30729)";
            request.CookieContainer = new CookieContainer();
            foreach (Cookie entry in Cookies)
            {
                if (entry.Name == cookName && entry.Value != "deleted")
                {
                    outCook = entry;
                }
                request.CookieContainer.Add(entry);
            }
            request.AllowAutoRedirect = false;
            request.Proxy = null;
            Stream DataStream = default(Stream);
            // Rückgabe holen
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            DataStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(DataStream);
            string ServerResponse = reader.ReadToEnd();

            foreach (Cookie cook in response.Cookies)
            {
                Cookies.Add(cook);
            }

            reader.Close();
            DataStream.Close();
            response.Close();


            return ServerResponse;

        }
    }
}
