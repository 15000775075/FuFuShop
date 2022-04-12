
using FuFuShop.Model.ViewModels.Options;
using Microsoft.AspNetCore.Http;

namespace FuFuShop.Services
{
    /// <summary>
    ///     标签表 服务工厂接口
    /// </summary>
    public interface IToolsServices
    {


        /// <summary>
        /// 查询是否存在违规内容并进行替换
        /// </summary>
        /// <returns></returns>
        Task<String> IllegalWordsReplace(string oldString, char symbol = '*');


        /// <summary>
        /// 查询是否存在违规内容
        /// </summary>
        /// <returns></returns>
        Task<bool> IllegalWordsContainsAny(string oldString);

        #region FIle文件上传处理


        /// <summary>
        /// QCloudOSS-腾讯云存储上传方法（File）
        /// </summary>
        /// <returns></returns>
        Task<string> UpLoadFileForQCloudOSS(FilesStorageOptions options, string fileExt, IFormFile file);

        #endregion



        #region Base64文件上传处理


        /// <summary>
        /// QCloudOSS-腾讯云存储上传方法（Base64）
        /// </summary>
        /// <returns></returns>
        string UpLoadBase64ForQCloudOSS(FilesStorageOptions options, byte[] bytes);


        #endregion




    }
}