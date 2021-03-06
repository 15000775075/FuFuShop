using SqlSugar;
using System.ComponentModel.DataAnnotations;

namespace FuFuShop.Model.Entities
{
    /// <summary>
    ///     用户地址表
    /// </summary>
    public partial class UserShip
    {
        /// <summary>
        ///     区域名称
        /// </summary>
        [Display(Name = "区域名称")]
        [SugarColumn(IsIgnore = true)]
        public string areaName { get; set; }
    }
}