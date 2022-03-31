
using SqlSugar;
using System.ComponentModel.DataAnnotations;

namespace FuFuShop.Model.Entities
{
    /// <summary>
    ///     商品浏览记录
    /// </summary>
    public partial class GoodsBrowsing
    {
        /// <summary>
        ///     商品图片
        /// </summary>
        [Display(Name = "商品图片")]
        [SugarColumn(IsIgnore = true)]
        public string goodImage { get; set; }


        /// <summary>
        ///     是否收藏
        /// </summary>
        [Display(Name = "是否收藏")]
        [SugarColumn(IsIgnore = true)]
        public bool isCollection { get; set; }
    }
}