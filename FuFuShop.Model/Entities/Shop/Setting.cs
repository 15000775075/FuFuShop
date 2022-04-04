using SqlSugar;
using System.ComponentModel.DataAnnotations;

namespace FuFuShop.Model.Entities.Shop
{
    /// <summary>
    /// 店铺设置表
    /// </summary>
    [SugarTable("FuFuShop_Setting", TableDescription = "店铺设置表")]
    public partial class Setting
    {
        /// <summary>
        /// 店铺设置表
        /// </summary>
        public Setting()
        {
        }

        /// <summary>
        /// 键
        /// </summary>
        [Display(Name = "键")]
        [SugarColumn(ColumnDescription = "键", IsPrimaryKey = true)]
        [Required(ErrorMessage = "请输入{0}")]
        [StringLength(50, ErrorMessage = "【{0}】不能超过{1}字符长度")]
        public string sKey { get; set; }
        /// <summary>
        /// 值
        /// </summary>
        [Display(Name = "值")]
        [SugarColumn(ColumnDescription = "值", IsNullable = true)]
        public string sValue { get; set; }
    }
}