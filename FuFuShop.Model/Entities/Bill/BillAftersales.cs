
using SqlSugar;
using System.ComponentModel.DataAnnotations;

namespace FuFuShop.Model.Entities
{
    /// <summary>
    /// 退货单表
    /// </summary>
    [SugarTable("FuFuShop_BillAftersales", TableDescription = "退货单表")]
    public partial class BillAftersales
    {
        /// <summary>
        /// 退货单表
        /// </summary>
        public BillAftersales()
        {
        }

        /// <summary>
        /// 售后单id
        /// </summary>
        [Display(Name = "售后单id")]
        [SugarColumn(ColumnDescription = "售后单id", IsPrimaryKey = true)]
        [Required(ErrorMessage = "请输入{0}")]
        [StringLength(20, ErrorMessage = "【{0}】不能超过{1}字符长度")]
        public string aftersalesId { get; set; }
        /// <summary>
        /// 订单ID
        /// </summary>
        [Display(Name = "订单ID")]
        [SugarColumn(ColumnDescription = "订单ID")]
        [Required(ErrorMessage = "请输入{0}")]
        [StringLength(20, ErrorMessage = "【{0}】不能超过{1}字符长度")]
        public string orderId { get; set; }
        /// <summary>
        /// 用户ID
        /// </summary>
        [Display(Name = "用户ID")]
        [SugarColumn(ColumnDescription = "用户ID")]
        [Required(ErrorMessage = "请输入{0}")]
        public int userId { get; set; }
        /// <summary>
        /// 售后类型
        /// </summary>
        [Display(Name = "售后类型")]
        [SugarColumn(ColumnDescription = "售后类型")]
        [Required(ErrorMessage = "请输入{0}")]
        public int type { get; set; }
        /// <summary>
        /// 退款金额
        /// </summary>
        [Display(Name = "退款金额")]
        [SugarColumn(ColumnDescription = "退款金额")]
        [Required(ErrorMessage = "请输入{0}")]
        public System.Decimal refundAmount { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        [Display(Name = "状态")]
        [SugarColumn(ColumnDescription = "状态")]
        [Required(ErrorMessage = "请输入{0}")]
        public int status { get; set; }
        /// <summary>
        /// 退款原因
        /// </summary>
        [Display(Name = "退款原因")]
        [SugarColumn(ColumnDescription = "退款原因")]
        [Required(ErrorMessage = "请输入{0}")]
        [StringLength(255, ErrorMessage = "【{0}】不能超过{1}字符长度")]
        public string reason { get; set; }
        /// <summary>
        /// 卖家备注，如果审核失败了，会显示到前端
        /// </summary>
        [Display(Name = "卖家备注，如果审核失败了，会显示到前端")]
        [SugarColumn(ColumnDescription = "卖家备注，如果审核失败了，会显示到前端", IsNullable = true)]
        [StringLength(255, ErrorMessage = "【{0}】不能超过{1}字符长度")]
        public string mark { get; set; }
        /// <summary>
        /// 提交时间
        /// </summary>
        [Display(Name = "提交时间")]
        [SugarColumn(ColumnDescription = "提交时间")]
        [Required(ErrorMessage = "请输入{0}")]
        public System.DateTime createTime { get; set; }
        /// <summary>
        /// 更新时间
        /// </summary>
        [Display(Name = "更新时间")]
        [SugarColumn(ColumnDescription = "更新时间", IsNullable = true)]
        public System.DateTime? updateTime { get; set; }
    }
}