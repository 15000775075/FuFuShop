

using SqlSugar;
using System.ComponentModel.DataAnnotations;

namespace FuFuShop.Model.Entities
{
    /// <summary>
    ///     评论
    /// </summary>
    public partial class GoodsComment
    {
        /// <summary>
        ///     图集数组
        /// </summary>
        [Display(Name = "图集数组")]
        [SugarColumn(IsIgnore = true)]
        public string[] imagesArr { get; set; }

        /// <summary>
        ///     用户头像
        /// </summary>
        [Display(Name = "用户头像")]
        [SugarColumn(IsIgnore = true)]
        public string avatarImage { get; set; }

        /// <summary>
        ///     用户昵称
        /// </summary>
        [Display(Name = "用户昵称")]
        [SugarColumn(IsIgnore = true)]
        public string nickName { get; set; }

        /// <summary>
        ///     用户手机
        /// </summary>
        [Display(Name = "用户手机")]
        [SugarColumn(IsIgnore = true)]
        public string mobile { get; set; }

        /// <summary>
        ///     商品名称
        /// </summary>
        [Display(Name = "商品名称")]
        [SugarColumn(IsIgnore = true)]
        public string goodName { get; set; }
    }
}