
using SqlSugar;
using System.ComponentModel.DataAnnotations;

namespace FuFuShop.Model.Entities
{
    /// <summary>
    /// 品牌表
    /// </summary>
    [SugarTable("FuFuShop_Brand", TableDescription = "品牌表")]
    public partial class Brand
    {
        /// <summary>
        /// 品牌表
        /// </summary>
        public Brand()
        {
        }

        /// <summary>
        /// 品牌ID
        /// </summary>
        [Display(Name = "品牌ID")]
        [SugarColumn(ColumnDescription = "品牌ID", IsPrimaryKey = true, IsIdentity = true)]
        [Required(ErrorMessage = "请输入{0}")]
        public int id { get; set; }
        /// <summary>
        /// 品牌名称
        /// </summary>
        [Display(Name = "品牌名称")]
        [SugarColumn(ColumnDescription = "品牌名称", IsNullable = true)]
        [StringLength(50, ErrorMessage = "【{0}】不能超过{1}字符长度")]
        public string name { get; set; }
        /// <summary>
        /// 品牌LOGO
        /// </summary>
        [Display(Name = "品牌LOGO")]
        [SugarColumn(ColumnDescription = "品牌LOGO", IsNullable = true)]
        [StringLength(255, ErrorMessage = "【{0}】不能超过{1}字符长度")]
        public string logoImageUrl { get; set; }
        /// <summary>
        /// 品牌排序
        /// </summary>
        [Display(Name = "品牌排序")]
        [SugarColumn(ColumnDescription = "品牌排序", IsNullable = true)]
        public int? sort { get; set; }
        /// <summary>
        /// 是否显示
        /// </summary>
        [Display(Name = "是否显示")]
        [SugarColumn(ColumnDescription = "是否显示")]
        [Required(ErrorMessage = "请输入{0}")]
        public System.Boolean isShow { get; set; }
        /// <summary>
        /// 更新时间
        /// </summary>
        [Display(Name = "更新时间")]
        [SugarColumn(ColumnDescription = "更新时间", IsNullable = true)]
        public System.DateTime? createTime { get; set; }
    }
}