
using SqlSugar;
using System.ComponentModel.DataAnnotations;

namespace FuFuShop.Model.Entities
{
    /// <summary>
    /// 退货单表
    /// </summary>
    [SugarTable("FuFuShop_BillReship", TableDescription = "退货单表")]
    public partial class BillReship
    {
        /// <summary>
        /// 退货单表
        /// </summary>
        public BillReship()
        {
        }

        /// <summary>
        /// 退货单号
        /// </summary>
        [Display(Name = "退货单号")]
        [SugarColumn(ColumnDescription = "退货单号", IsPrimaryKey = true)]
        [Required(ErrorMessage = "请输入{0}")]
        [StringLength(20, ErrorMessage = "【{0}】不能超过{1}字符长度")]
        public string reshipId { get; set; }
        /// <summary>
        /// 订单序列
        /// </summary>
        [Display(Name = "订单序列")]
        [SugarColumn(ColumnDescription = "订单序列")]
        [Required(ErrorMessage = "请输入{0}")]
        [StringLength(20, ErrorMessage = "【{0}】不能超过{1}字符长度")]
        public string orderId { get; set; }
        /// <summary>
        /// 售后单序列
        /// </summary>
        [Display(Name = "售后单序列")]
        [SugarColumn(ColumnDescription = "售后单序列")]
        [Required(ErrorMessage = "请输入{0}")]
        [StringLength(20, ErrorMessage = "【{0}】不能超过{1}字符长度")]
        public string aftersalesId { get; set; }
        /// <summary>
        /// 用户ID 关联user.id
        /// </summary>
        [Display(Name = "用户ID 关联user.id")]
        [SugarColumn(ColumnDescription = "用户ID 关联user.id")]
        [Required(ErrorMessage = "请输入{0}")]
        public Int32 userId { get; set; }
        /// <summary>
        /// 物流公司编码
        /// </summary>
        [Display(Name = "物流公司编码")]
        [SugarColumn(ColumnDescription = "物流公司编码", IsNullable = true)]
        [StringLength(50, ErrorMessage = "【{0}】不能超过{1}字符长度")]
        public string logiCode { get; set; }
        /// <summary>
        /// 物流单号
        /// </summary>
        [Display(Name = "物流单号")]
        [SugarColumn(ColumnDescription = "物流单号", IsNullable = true)]
        [StringLength(50, ErrorMessage = "【{0}】不能超过{1}字符长度")]
        public string logiNo { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        [Display(Name = "状态")]
        [SugarColumn(ColumnDescription = "状态")]
        [Required(ErrorMessage = "请输入{0}")]
        public Int32 status { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        [Display(Name = "备注")]
        [SugarColumn(ColumnDescription = "备注", IsNullable = true)]
        [StringLength(255, ErrorMessage = "【{0}】不能超过{1}字符长度")]
        public string memo { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        [Display(Name = "创建时间")]
        [SugarColumn(ColumnDescription = "创建时间")]
        [Required(ErrorMessage = "请输入{0}")]
        public DateTime createTime { get; set; }
        /// <summary>
        /// 更新时间
        /// </summary>
        [Display(Name = "更新时间")]
        [SugarColumn(ColumnDescription = "更新时间", IsNullable = true)]
        public DateTime? updateTime { get; set; }
    }
}