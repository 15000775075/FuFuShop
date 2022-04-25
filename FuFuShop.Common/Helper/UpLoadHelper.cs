using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuFuShop.Common.Helper
{
    /// <summary>
    /// 上传帮助类
    /// </summary>
    public static class UpLoadHelper
    {
        /// <summary>
        /// 上传路径格式化操作，防止不同类型下上传路径写入失败问题。
        /// </summary>
        /// <param name="storageType">上传类型</param>
        /// <param name="oldFilePath">原始路径</param>
        /// <returns></returns>
        public static string PathFormat(string storageType, string oldFilePath)
        {
            string newPath;
            switch (storageType)
            {
                case "LocalStorage":
                    newPath = oldFilePath.StartsWith("/") ? oldFilePath : "/" + oldFilePath;
                    break;
                case "AliYunOSS":
                    newPath = oldFilePath.StartsWith("/") ? oldFilePath.Substring(1) : oldFilePath;
                    break;
                case "QCloudOSS":
                    newPath = oldFilePath.StartsWith("/") ? oldFilePath.Substring(1) : oldFilePath;
                    break;
                default:
                    newPath = "/upload/";
                    break;
            }
            newPath = newPath.EndsWith("/") ? newPath : newPath + "/";
            return newPath;
        }
    }
}
