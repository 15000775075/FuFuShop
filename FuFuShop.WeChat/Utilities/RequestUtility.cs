using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using System.Text;

namespace FuFuShop.WeChat.Utilities
{
    /// <summary>HTTP 请求工具类</summary>
    public static class RequestUtility
    {
        /// <summary>【异步方法】从 Request.Body 中读取流，并复制到一个独立的 MemoryStream 对象中</summary>
        /// <param name="request"></param>
        /// <param name="allowSynchronousIO"></param>
        /// <returns></returns>
        public static async Task<Stream> GetRequestMemoryStreamAsync(
          this HttpRequest request,
          bool? allowSynchronousIO = true)
        {
            IHttpBodyControlFeature bodyControlFeature = request.HttpContext.Features.Get<IHttpBodyControlFeature>();
            if (bodyControlFeature != null && allowSynchronousIO.HasValue)
                bodyControlFeature.AllowSynchronousIO = allowSynchronousIO.Value;
            return (Stream)new MemoryStream(Encoding.UTF8.GetBytes(await new StreamReader(request.Body).ReadToEndAsync()));
        }

        /// <summary>从 Request.Body 中读取流，并复制到一个独立的 MemoryStream 对象中</summary>
        /// <param name="request"></param>
        /// <param name="allowSynchronousIO"></param>
        /// <returns></returns>
        public static Stream GetRequestStream(
            this HttpRequest request,
            bool? allowSynchronousIO = true)
        {
            IHttpBodyControlFeature bodyControlFeature = request.HttpContext.Features.Get<IHttpBodyControlFeature>();
            if (bodyControlFeature != null && allowSynchronousIO.HasValue)
                bodyControlFeature.AllowSynchronousIO = allowSynchronousIO.Value;
            return (Stream)new MemoryStream(Encoding.UTF8.GetBytes(new StreamReader(request.Body).ReadToEnd()));
        }

        /// <summary>从 Request.Body 中读取流，并复制到一个独立的 MemoryStream 对象中</summary>
        /// <param name="request"></param>
        /// <param name="allowSynchronousIO"></param>
        /// <returns></returns>
        public static MemoryStream GetRequestMemoryStream(
            this HttpRequest request,
            bool? allowSynchronousIO = true)
        {
            IHttpBodyControlFeature bodyControlFeature = request.HttpContext.Features.Get<IHttpBodyControlFeature>();
            if (bodyControlFeature != null && allowSynchronousIO.HasValue)
                bodyControlFeature.AllowSynchronousIO = allowSynchronousIO.Value;
            return new MemoryStream(Encoding.UTF8.GetBytes(new StreamReader(request.Body).ReadToEnd()));
        }


    }

}
