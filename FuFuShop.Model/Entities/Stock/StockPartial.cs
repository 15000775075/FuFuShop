
using SqlSugar;
using System.ComponentModel.DataAnnotations;

namespace FuFuShop.Model.Entities
{
    /// <summary>
    ///     库存操作表
    /// </summary>
    public partial class Stock
    {
        /// <summary>
        ///     操作员昵称
        /// </summary>
        [Display(Name = "操作员昵称")]
        [SugarColumn(IsIgnore = true)]
        public string managerName { get; set; }
    }
}