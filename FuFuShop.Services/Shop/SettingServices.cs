using FuFuShop.Common.AppSettings;
using FuFuShop.Common.Caching.Manual;
using FuFuShop.Common.Extensions;
using FuFuShop.Common.Helper;
using FuFuShop.Model.Entities.Shop;
using FuFuShop.Model.FromBody;
using FuFuShop.Model.ViewModels.Basics;
using FuFuShop.Model.ViewModels.Options;
using FuFuShop.Model.ViewModels.Sms;
using FuFuShop.Model.ViewModels.UI;
using FuFuShop.Repository.Shop;
using FuFuShop.Repository.UnitOfWork;
using FuFuShop.Services.BaseServices;

namespace FuFuShop.Services.Shop
{
    /// <summary>
    /// 店铺设置表 接口实现
    /// </summary>
    public class SettingServices : BaseServices<Setting>, ISettingServices
    {
        private readonly ISettingRepository _dal;
        private readonly IUnitOfWork _unitOfWork;
        public SettingServices(IUnitOfWork unitOfWork, ISettingRepository dal)
        {
            _dal = dal;
            BaseDal = dal;
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// 重写异步更新方法方法
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<AdminUiCallBack> UpdateAsync(FMSettingDoSaveModel model)
        {
            var jm = new AdminUiCallBack();

            var entity = model.entity;
            if (!entity.Any())
            {
                jm.msg = "数据不能为空";
                return jm;
            }
            var oldList = await _dal.QueryAsync();
            var bl = false;
            if (oldList.Any())
            {
                var arr = entity.Select(p => p.sKey).ToList();
                var old = oldList.Where(p => arr.Contains(p.sKey)).ToList();
                if (old.Any())
                {
                    old.ForEach(p =>
                    {
                        var o = entity.Find(c => c.sKey == p.sKey);
                        p.sValue = o != null ? o.sValue : "";
                    });
                    bl = await base.UpdateAsync(old);
                }
                var arrOld = oldList.Select(p => p.sKey).ToList();
                var newData = entity.Where(p => !arrOld.Contains(p.sKey)).ToList();
                if (newData.Any())
                {
                    var settings = new List<Setting>();
                    newData.ForEach(p =>
                    {
                        settings.Add(new Setting() { sKey = p.sKey, sValue = p.sValue.ToString() });
                    });
                    bl = await InsertAsync(settings) > 0;
                }
            }
            else
            {
                var settings = new List<Setting>();
                entity.ForEach(p =>
                {
                    settings.Add(new Setting() { sKey = p.sKey, sValue = p.sValue.ToString() });
                });
                bl = await InsertAsync(settings) > 0;
            }

            await UpdateCache();


            jm.code = bl ? 0 : 1;
            jm.msg = bl ? GlobalConstVars.EditSuccess : GlobalConstVars.EditFailure;

            return jm;
        }


        /// <summary>
        /// 获取缓存数据
        /// </summary>
        /// <returns></returns>
        private async Task UpdateCache()
        {
            var list = await _dal.QueryAsync();
            ManualDataCache.Instance.Set(GlobalConstVars.CacheSettingList, list, 1440);

            var configs = SystemSettingDictionary.GetConfig();
            foreach (KeyValuePair<string, DictionaryKeyValues> kvp in configs)
            {
                var model = list.Find(p => p.sKey == kvp.Key);
                if (model != null)
                {
                    kvp.Value.sValue = model.sValue;
                }
            }
            ManualDataCache.Instance.Set(GlobalConstVars.CacheSettingByComparison, configs, 1440);
        }

        /// <summary>
        /// 获取缓存数据
        /// </summary>
        /// <returns></returns>
        private async Task<List<Setting>> GetDatas()
        {
            var cache = ManualDataCache.Instance.Get<List<Setting>>(GlobalConstVars.CacheSettingList);
            if (cache == null)
            {
                var list = await _dal.QueryAsync();
                ManualDataCache.Instance.Set(GlobalConstVars.CacheSettingList, list, 1440);
                return list;
            }
            return ManualDataCache.Instance.Get<List<Setting>>(GlobalConstVars.CacheSettingList);
        }

        /// <summary>
        /// 获取数据库整合后配置信息
        /// </summary>
        /// <returns></returns>
        public async Task<Dictionary<string, DictionaryKeyValues>> GetConfigDictionaries()
        {
            var configs = SystemSettingDictionary.GetConfig();
            var settings = await GetDatas();
            foreach (KeyValuePair<string, DictionaryKeyValues> kvp in configs)
            {
                var model = settings.Find(p => p.sKey == kvp.Key);
                if (model != null)
                {
                    kvp.Value.sValue = model.sValue;
                }
            }
            return configs;
        }


        /// <summary>
        /// 获取附件存储的配置信息
        /// </summary>
        /// <returns></returns>
        public async Task<FilesStorageOptions> GetFilesStorageOptions()
        {
            var filesStorageOptions = new FilesStorageOptions();

            var configs = SystemSettingDictionary.GetConfig();
            var settings = await GetDatas();

            filesStorageOptions.StorageType = GetValue(SystemSettingConstVars.FilesStorageType, configs, settings);
            filesStorageOptions.Path = GetValue(SystemSettingConstVars.FilesStoragePath, configs, settings);
            filesStorageOptions.FileTypes = GetValue(SystemSettingConstVars.FilesStorageFileSuffix, configs, settings);
            filesStorageOptions.MaxSize = GetValue(SystemSettingConstVars.FilesStorageFileMaxSize, configs, settings).ObjectToInt(10);

            //云基础
            filesStorageOptions.BucketBindUrl = GetValue(SystemSettingConstVars.FilesStorageBucketBindUrl, configs, settings);
            filesStorageOptions.AccessKeyId = GetValue(SystemSettingConstVars.FilesStorageAccessKeyId, configs, settings);
            filesStorageOptions.AccessKeySecret = GetValue(SystemSettingConstVars.FilesStorageAccessKeySecret, configs, settings);
            //腾讯云
            filesStorageOptions.AccountId = GetValue(SystemSettingConstVars.FilesStorageTencentAccountId, configs, settings);
            filesStorageOptions.CosRegion = GetValue(SystemSettingConstVars.FilesStorageTencentCosRegion, configs, settings);
            filesStorageOptions.TencentBucketName = GetValue(SystemSettingConstVars.FilesStorageTencentBucketName, configs, settings);
            //阿里云
            filesStorageOptions.BucketName = GetValue(SystemSettingConstVars.FilesStorageAliYunBucketName, configs, settings);
            filesStorageOptions.Endpoint = GetValue(SystemSettingConstVars.FilesStorageAliYunEndpoint, configs, settings);

            //七牛云
            filesStorageOptions.QiNiuBucketName = GetValue(SystemSettingConstVars.FilesStorageQiNiuBucketName, configs, settings);

            //格式化存储文件夹路径
            filesStorageOptions.Path = UpLoadHelper.PathFormat(filesStorageOptions.StorageType, filesStorageOptions.Path);

            return filesStorageOptions;
        }

        /// <summary>
        /// 获取短信配置实体
        /// </summary>
        /// <returns></returns>
        public async Task<SMSOptions> GetSmsOptions()
        {
            var sms = new SMSOptions();

            var configs = SystemSettingDictionary.GetConfig();
            var settings = await GetDatas();

            sms.Enabled = GetValue(SystemSettingConstVars.SmsEnabled, configs, settings).ObjectToInt(1) == 1;
            sms.UserId = GetValue(SystemSettingConstVars.SmsUserId, configs, settings);
            sms.Account = GetValue(SystemSettingConstVars.SmsAccount, configs, settings);
            sms.Password = GetValue(SystemSettingConstVars.SmsPassword, configs, settings);
            sms.Signature = GetValue(SystemSettingConstVars.SmsSignature, configs, settings);
            sms.ApiUrl = GetValue(SystemSettingConstVars.SmsApiUrl, configs, settings);

            return sms;
        }

        public string GetValue(string key, Dictionary<string, DictionaryKeyValues> configs, List<Setting> settings)
        {
            var objSetting = settings.Find(p => p.sKey == key);
            if (objSetting != null)
            {
                return objSetting.sValue;
            }
            configs.TryGetValue(key, out var di);
            return di?.sValue;
        }

    }
}
