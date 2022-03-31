using FuFuShop.Common.AppSettings;

namespace FuFuShop.Common.Caching.Manual
{
    /// <summary>
    /// 手动缓存调用
    /// </summary>
    public static partial class ManualDataCache
    {
        private static IManualCacheManager _instance = null;
        /// <summary>
        /// 静态实例，外部可直接调用
        /// </summary>
        public static IManualCacheManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    if (AppSettingsConstVars.RedisUseCache)
                    {
                        _instance = new RedisCacheManager();
                    }
                }
                return _instance;
            }
        }
    }
}
