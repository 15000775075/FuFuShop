using System.Net;
using System.Text;

namespace FuFuShop.Common.Helper
{
    /// <summary>
    /// 模拟标准表单Post提交
    /// </summary>
    public static class HttpHelper
    {
        /// <summary>
        /// 模拟标准表单Post提交
        /// </summary>
        /// <param name="url"></param>
        /// <param name="postdate"></param>
        /// <returns></returns>
        public static string PostSend(string url, string postdate)
        {
            HttpWebRequest myHttpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            myHttpWebRequest.ContentType = "application/x-www-form-urlencoded;charset=UTF-8";
            myHttpWebRequest.Method = "POST";
            Stream myRequestStream = myHttpWebRequest.GetRequestStream();
            StreamWriter myStreamWriter = new StreamWriter(myRequestStream);
            myStreamWriter.Write(postdate);
            myStreamWriter.Flush();
            myStreamWriter.Close();
            myRequestStream.Close();

            HttpWebResponse myHttpWebResponse = (HttpWebResponse)myHttpWebRequest.GetResponse();
            Stream myResponseStream = myHttpWebResponse.GetResponseStream();
            StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));
            String outdata = myStreamReader.ReadToEnd();
            myStreamReader.Close();
            myResponseStream.Close();
            return outdata;
        }

    }
}
