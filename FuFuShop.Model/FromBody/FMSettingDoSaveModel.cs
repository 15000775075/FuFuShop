using FuFuShop.Model.ViewModels.Basics;

namespace FuFuShop.Model.FromBody
{
    /// <summary>
    ///     配置文件更新类
    /// </summary>
    public class FMSettingDoSaveModel
    {
        /// <summary>
        ///     列表
        /// </summary>
        public List<DictionaryKeyValues> entity { get; set; }
    }
}