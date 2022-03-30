using FuFuShop.WeChat.Models;
using System.Security.Cryptography;
using System.Text;

namespace FuFuShop.WeChat.Utilities
{
    /// <summary>签名验证类</summary>
    public class CheckSignature
    {
        /// <summary>在网站没有提供Token（或传入为null）的情况下的默认Token，建议在网站中进行配置。</summary>
        public const string Token = "weixin";

        /// <summary>检查签名是否正确</summary>
        /// <param name="signature"></param>
        /// <param name="postModel">需要提供：Timestamp、Nonce、Token</param>
        /// <returns></returns>
        public static bool Check(string signature, PostModel postModel) => Check(signature, postModel.Timestamp, postModel.Nonce, postModel.Token);

        /// <summary>检查签名是否正确</summary>
        /// <param name="signature"></param>
        /// <param name="timestamp"></param>
        /// <param name="nonce"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static bool Check(string signature, string timestamp, string nonce, string token = null) => signature == GetSignature(timestamp, nonce, token);

        /// <summary>返回正确的签名</summary>
        /// <param name="postModel">需要提供：Timestamp、Nonce、Token</param>
        /// <returns></returns>
        public static string GetSignature(PostModel postModel) => GetSignature(postModel.Timestamp, postModel.Nonce, postModel.Token);

        /// <summary>返回正确的签名</summary>
        /// <param name="timestamp"></param>
        /// <param name="nonce"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static string GetSignature(string timestamp, string nonce, string token = null)
        {
            token = token ?? "weixin";
            string s = string.Join("", (new string[3]
            {
        token,
        timestamp,
        nonce
            }).OrderBy(z => z).ToArray());
            byte[] hash = SHA1.Create().ComputeHash(Encoding.UTF8.GetBytes(s));
            StringBuilder stringBuilder = new StringBuilder();
            foreach (byte num in hash)
                stringBuilder.AppendFormat("{0:x2}", num);
            return stringBuilder.ToString();
        }
    }
}
