
using SqlSugar;
using System.ComponentModel.DataAnnotations;

namespace FuFuShop.Model.Entities
{
    /// <summary>
    /// 商品分类
    /// </summary>
    [SugarTable("FuFuShop_GoodsCategory", TableDescription = "商品分类")]
    public partial class GoodsCategory
    {
        /// <summary>
        /// 商品分类
        /// </summary>
        public GoodsCategory()
        {
        }

        /// <summary>
        /// 序列
        /// </summary>
        [Display(Name = "序列")]
        [SugarColumn(ColumnDescription = "序列", IsPrimaryKey = true, IsIdentity = true)]
        [Required(ErrorMessage = "请输入{0}")]
        public int id { get; set; }
        /// <summary>
        /// 上级分类id
        /// </summary>
        [Display(Name = "上级分类id")]
        [SugarColumn(ColumnDescription = "上级分类id")]
        [Required(ErrorMessage = "请输入{0}")]
        public int parentId { get; set; }
        /// <summary>
        /// 分类名称
        /// </summary>
        [Display(Name = "分类名称")]
        [SugarColumn(ColumnDescription = "分类名称")]
        [Required(ErrorMessage = "请输入{0}")]
        [StringLength(20, ErrorMessage = "【{0}】不能超过{1}字符长度")]
        public string name { get; set; }
        /// <summary>
        /// 类型ID 关联 goods_type.id
        /// </summary>
        [Display(Name = "类型ID 关联 goods_type.id")]
        [SugarColumn(ColumnDescription = "类型ID 关联 goods_type.id")]
        [Required(ErrorMessage = "请输入{0}")]
        public int typeId { get; set; }
        /// <summary>
        /// 分类排序
        /// </summary>
        [Display(Name = "分类排序")]
        [SugarColumn(ColumnDescription = "分类排序")]
        [Required(ErrorMessage = "请输入{0}")]
        public int sort { get; set; }
        /// <summary>
        /// 分类图片ID
        /// </summary>
        [Display(Name = "分类图片ID")]
        [SugarColumn(ColumnDescription = "分类图片ID", IsNullable = true)]
        [StringLength(255, ErrorMessage = "【{0}】不能超过{1}字符长度")]
        public string imageUrl { get; set; }
        /// <summary>
        /// 是否显示
        /// </summary>
        [Display(Name = "是否显示")]
        [SugarColumn(ColumnDescription = "是否显示")]
        [Required(ErrorMessage = "请输入{0}")]
        public System.Boolean isShow { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        [Display(Name = "创建时间")]
        [SugarColumn(ColumnDescription = "创建时间", IsNullable = true)]
        public System.DateTime? createTime { get; set; }
    }
}