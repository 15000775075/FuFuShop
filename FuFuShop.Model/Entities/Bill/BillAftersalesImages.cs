
using SqlSugar;
using System.ComponentModel.DataAnnotations;

namespace FuFuShop.Model.Entities
{
    /// <summary>
    /// 商品图片关联表
    /// </summary>
    [SugarTable("FuFuShop_BillAftersalesImages", TableDescription = "商品图片关联表")]
    public partial class BillAftersalesImages
    {
        /// <summary>
        /// 商品图片关联表
        /// </summary>
        public BillAftersalesImages()
        {
        }

        /// <summary>
        /// 售后单id
        /// </summary>
        [Display(Name = "售后单id")]
        [SugarColumn(ColumnDescription = "售后单id")]
        [Required(ErrorMessage = "请输入{0}")]
        [StringLength(20, ErrorMessage = "【{0}】不能超过{1}字符长度")]
        public string aftersalesId { get; set; }
        /// <summary>
        /// 图片地址
        /// </summary>
        [Display(Name = "图片地址")]
        [SugarColumn(ColumnDescription = "图片地址")]
        [Required(ErrorMessage = "请输入{0}")]
        [StringLength(255, ErrorMessage = "【{0}】不能超过{1}字符长度")]
        public string imageUrl { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        [Display(Name = "排序")]
        [SugarColumn(ColumnDescription = "排序")]
        [Required(ErrorMessage = "请输入{0}")]
        public int sortId { get; set; }
    }
}