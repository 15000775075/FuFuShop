
using SqlSugar;
using System.ComponentModel.DataAnnotations;

namespace FuFuShop.Model.Entities
{
    /// <summary>
    /// 商品收藏表
    /// </summary>
    [SugarTable("FuFuShop_GoodsCollection", TableDescription = "商品收藏表")]
    public partial class GoodsCollection
    {
        /// <summary>
        /// 商品收藏表
        /// </summary>
        public GoodsCollection()
        {

        }

        /// <summary>
        /// ID
        /// </summary>
        [Display(Name = "ID")]
        [SugarColumn(ColumnDescription = "ID", IsPrimaryKey = true, IsIdentity = true)]
        [Required(ErrorMessage = "请输入{0}")]
        public int id { get; set; }
        /// <summary>
        /// 商品id 关联goods.id
        /// </summary>
        [Display(Name = "商品id 关联goods.id")]
        [SugarColumn(ColumnDescription = "商品id 关联goods.id")]
        [Required(ErrorMessage = "请输入{0}")]
        public int goodsId { get; set; }
        /// <summary>
        /// 用户id
        /// </summary>
        [Display(Name = "用户id")]
        [SugarColumn(ColumnDescription = "用户id")]
        [Required(ErrorMessage = "请输入{0}")]
        public int userId { get; set; }
        /// <summary>
        /// 商品名称
        /// </summary>
        [Display(Name = "商品名称")]
        [SugarColumn(ColumnDescription = "商品名称")]
        [Required(ErrorMessage = "请输入{0}")]
        [StringLength(200, ErrorMessage = "【{0}】不能超过{1}字符长度")]
        public string goodsName { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        [Display(Name = "创建时间")]
        [SugarColumn(ColumnDescription = "创建时间")]
        [Required(ErrorMessage = "请输入{0}")]
        public System.DateTime createTime { get; set; }
    }
}