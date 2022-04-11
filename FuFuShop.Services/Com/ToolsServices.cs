/***********************************************************************
 *            Project: 
 *        ProjectName: 核心内容管理系统                                
 *                Web: https://www..net                      
 *             Author: 大灰灰                                          
 *              Email: jianweie@163.com                                
 *         CreateTime: 2021/1/31 21:45:10
 *        Description: 暂无
 ***********************************************************************/


using COSXML;
using FuFuShop.Common.AppSettings;
using FuFuShop.Common.Caching.Manual;
using FuFuShop.Model.ViewModels.Options;
using FuFuShop.Model.ViewModels.UI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.FileProviders;
using System.Globalization;
using ToolGood.Words;

namespace FuFuShop.Services
{
    /// <summary>
    ///     标签表 接口实现
    /// </summary>
    public class ToolsServices : IToolsServices
    {
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly IWebHostEnvironment _webHostEnvironment;



        public ToolsServices(IWebHostEnvironment hostEnvironment, IWebHostEnvironment webHostEnvironment)
        {
            _hostEnvironment = hostEnvironment;
            _webHostEnvironment = webHostEnvironment;
        }


        /// <summary>
        /// 查询是否存在违规内容并进行替换
        /// </summary>
        /// <returns></returns>
        public async Task<String> IllegalWordsReplace(string oldString, char symbol = '*')
        {
            var cache = ManualDataCache.Instance.Get<string>(ToolsVars.IllegalWordsCahceName);
            if (string.IsNullOrEmpty(cache))
            {
                IFileProvider fileProvider = this._hostEnvironment.ContentRootFileProvider;
                IFileInfo fileInfo = fileProvider.GetFileInfo("illegalWord/IllegalKeywords.txt");

                string fileContent = null;

                using (StreamReader readSteam = new StreamReader(fileInfo.CreateReadStream()))
                {
                    fileContent = await readSteam.ReadToEndAsync();
                }
                cache = fileContent;
                ManualDataCache.Instance.Set(ToolsVars.IllegalWordsCahceName, cache);
            }

            WordsMatch wordsSearch = new WordsMatch();
            wordsSearch.SetKeywords(cache.Split('|', StringSplitOptions.RemoveEmptyEntries));

            var t = wordsSearch.Replace(oldString, symbol);
            return t;
        }


        /// <summary>
        /// 查询是否存在违规内容
        /// </summary>
        /// <returns></returns>
        public async Task<bool> IllegalWordsContainsAny(string oldString)
        {
            var cache = ManualDataCache.Instance.Get<string>(ToolsVars.IllegalWordsCahceName);
            if (string.IsNullOrEmpty(cache))
            {
                IFileProvider fileProvider = this._hostEnvironment.ContentRootFileProvider;
                IFileInfo fileInfo = fileProvider.GetFileInfo("illegalWord/IllegalKeywords.txt");

                string fileContent = null;

                using (StreamReader readSteam = new StreamReader(fileInfo.CreateReadStream()))
                {
                    fileContent = await readSteam.ReadToEndAsync();
                }
                cache = fileContent;
                ManualDataCache.Instance.Set(ToolsVars.IllegalWordsCahceName, cache);
            }

            WordsMatch wordsSearch = new WordsMatch();
            wordsSearch.SetKeywords(cache.Split('|', StringSplitOptions.RemoveEmptyEntries));

            var bl = wordsSearch.ContainsAny(oldString);

            return bl;
        }


        #region 腾讯云存储上传方法（File）
        /// <summary>
        /// 腾讯云存储上传方法（File）
        /// </summary>
        /// <param name="options"></param>
        /// <param name="fileExt"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        public async Task<string> UpLoadFileForQCloudOSS(FilesStorageOptions options, string fileExt, IFormFile file)
        {
            var jm = new AdminUiCallBack();

            var newFileName = DateTime.Now.ToString("yyyyMMddHHmmss_ffff", DateTimeFormatInfo.InvariantInfo) + fileExt;
            var today = DateTime.Now.ToString("yyyyMMdd");

            var filePath = options.Path + today + "/" + newFileName; //云文件保存路径

            //上传到腾讯云OSS
            //初始化 CosXmlConfig
            string appid = options.AccountId;//设置腾讯云账户的账户标识 APPID
            string region = options.CosRegion; //设置一个默认的存储桶地域
            CosXmlConfig config = new CosXmlConfig.Builder()
                //.SetAppid(appid)
                .IsHttps(true)  //设置默认 HTTPS 请求
                .SetRegion(region)  //设置一个默认的存储桶地域
                .SetDebugLog(true)  //显示日志
                .Build();  //创建 CosXmlConfig 对象

            long durationSecond = 600;  //每次请求签名有效时长，单位为秒
            QCloudCredentialProvider qCloudCredentialProvider = new DefaultQCloudCredentialProvider(options.AccessKeyId, options.AccessKeySecret, durationSecond);


            byte[] bytes;
            await using (var ms = new MemoryStream())
            {
                await file.CopyToAsync(ms);
                bytes = ms.ToArray();
            }

            var cosXml = new CosXmlServer(config, qCloudCredentialProvider);
            COSXML.Model.Object.PutObjectRequest putObjectRequest = new COSXML.Model.Object.PutObjectRequest(options.TencentBucketName, filePath, bytes);
            cosXml.PutObject(putObjectRequest);

            return options.BucketBindUrl + filePath;
        }
        #endregion



        #region 腾讯云存储上传方法（Base64）

        /// <summary>
        /// 腾讯云存储上传方法（Base64）
        /// </summary>
        /// <param name="options"></param>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public string UpLoadBase64ForQCloudOSS(FilesStorageOptions options, byte[] bytes)
        {
            var jm = new AdminUiCallBack();

            var newFileName = DateTime.Now.ToString("yyyyMMddHHmmss_ffff", DateTimeFormatInfo.InvariantInfo) + ".jpg";
            var today = DateTime.Now.ToString("yyyyMMdd");

            //初始化 CosXmlConfig
            string appid = options.AccountId;//设置腾讯云账户的账户标识 APPID
            string region = options.CosRegion; //设置一个默认的存储桶地域
            CosXmlConfig config = new CosXmlConfig.Builder()
                //.SetAppid(appid)
                .IsHttps(true)  //设置默认 HTTPS 请求
                .SetRegion(region)  //设置一个默认的存储桶地域
                .SetDebugLog(true)  //显示日志
                .Build();  //创建 CosXmlConfig 对象

            long durationSecond = 600;  //每次请求签名有效时长，单位为秒
            QCloudCredentialProvider qCloudCredentialProvider = new DefaultQCloudCredentialProvider(options.AccessKeyId, options.AccessKeySecret, durationSecond);

            var cosXml = new CosXmlServer(config, qCloudCredentialProvider);

            var filePath = options.Path + today + "/" + newFileName; //云文件保存路径
            COSXML.Model.Object.PutObjectRequest putObjectRequest = new COSXML.Model.Object.PutObjectRequest(options.TencentBucketName, filePath, bytes);

            cosXml.PutObject(putObjectRequest);

            return options.BucketBindUrl + filePath;
        }
        #endregion



    }
}