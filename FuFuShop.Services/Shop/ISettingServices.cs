using FuFuShop.Model.Entities.Shop;
using FuFuShop.Model.ViewModels.Basics;
using FuFuShop.Model.ViewModels.Options;
using FuFuShop.Model.ViewModels.Sms;
using FuFuShop.Services.BaseServices;

namespace FuFuShop.Services.Shop
{
    /// <summary>
    ///     店铺设置表 服务工厂接口
    /// </summary>
    public interface ISettingServices : IBaseServices<Setting>
    {
        /// <summary>
        ///     重写异步更新方法方法
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
     //   Task<AdminUiCallBack> UpdateAsync(FMSettingDoSaveModel entity);

        /// <summary>
        ///     获取数据库整合后配置信息
        /// </summary>
        /// <returns></returns>
        Task<Dictionary<string, DictionaryKeyValues>> GetConfigDictionaries();


        /// <summary>
        ///     获取附件存储的配置信息
        /// </summary>
        /// <returns></returns>
        Task<FilesStorageOptions> GetFilesStorageOptions();

        /// <summary>
        ///     获取短信配置实体
        /// </summary>
        /// <returns></returns>
        Task<SMSOptions> GetSmsOptions();
    }
}