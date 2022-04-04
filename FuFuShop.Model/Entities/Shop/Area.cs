using SqlSugar;
using System.ComponentModel.DataAnnotations;

namespace FuFuShop.Model.Entities.Shop
{
    /// <summary>
    /// 地区表
    /// </summary>
    [SugarTable("FuFuShop_Area", TableDescription = "地区表")]
    public partial class Area
    {
        /// <summary>
        /// 地区表
        /// </summary>
        public Area()
        {
        }

        /// <summary>
        /// 地区ID
        /// </summary>
        [Display(Name = "地区ID")]
        [SugarColumn(ColumnDescription = "地区ID", IsPrimaryKey = true, IsIdentity = true)]
        [Required(ErrorMessage = "请输入{0}")]
        public int id { get; set; }
        /// <summary>
        /// 父级ID
        /// </summary>
        [Display(Name = "父级ID")]
        [SugarColumn(ColumnDescription = "父级ID", IsNullable = true)]
        public int? parentId { get; set; }
        /// <summary>
        /// 地区深度
        /// </summary>
        [Display(Name = "地区深度")]
        [SugarColumn(ColumnDescription = "地区深度", IsNullable = true)]
        public int? depth { get; set; }
        /// <summary>
        /// 地区名称
        /// </summary>
        [Display(Name = "地区名称")]
        [SugarColumn(ColumnDescription = "地区名称", IsNullable = true)]
        [StringLength(50, ErrorMessage = "【{0}】不能超过{1}字符长度")]
        public string name { get; set; }
        /// <summary>
        /// 邮编
        /// </summary>
        [Display(Name = "邮编")]
        [SugarColumn(ColumnDescription = "邮编", IsNullable = true)]
        [StringLength(10, ErrorMessage = "【{0}】不能超过{1}字符长度")]
        public string postalCode { get; set; }
        /// <summary>
        /// 地区排序
        /// </summary>
        [Display(Name = "地区排序")]
        [SugarColumn(ColumnDescription = "地区排序")]
        [Required(ErrorMessage = "请输入{0}")]
        public int sort { get; set; }
    }
}