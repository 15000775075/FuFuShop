

using FuFuShop.Admin.Filter;
using FuFuShop.Common.AppSettings;
using FuFuShop.Common.Helper;
using FuFuShop.Model.FromBody;
using FuFuShop.Model.ViewModels.UI;
using FuFuShop.Services.Shop;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;

namespace FuFuShop.Admin.Controllers.Shop
{
    /// <summary>
    /// 平台设置表
    ///</summary>
    [Description("平台设置表")]
    [Route("api/[controller]/[action]")]
    [ApiController]
    [RequiredErrorForAdmin]
    [Authorize(Permissions.Name)]
    public class CoreCmsSettingController : ControllerBase
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ISettingServices _SettingServices;

        /// <summary>
        /// 构造函数
        ///</summary>
        ///  <param name="webHostEnvironment"></param>
        ///<param name="SettingServices"></param>
        public CoreCmsSettingController(IWebHostEnvironment webHostEnvironment, ISettingServices SettingServices)
        {
            _webHostEnvironment = webHostEnvironment;
            _SettingServices = SettingServices;
        }

        #region 首页数据============================================================
        // POST: Api/Setting/GetIndex
        /// <summary>
        /// 首页数据
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Description("首页数据")]
        public async Task<AdminUiCallBack> GetIndex()
        {
            //返回数据
            var jm = new AdminUiCallBack { code = 0 };
            var configs = await _SettingServices.GetConfigDictionaries();
            var filesStorageOptionsType = EnumHelper.EnumToList<GlobalEnumVars.FilesStorageOptionsType>();

            jm.data = new
            {
                configs,
                filesStorageOptionsType
            };

            return jm;
        }
        #endregion


    }



}
