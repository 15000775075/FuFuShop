
using SqlSugar;
using System.ComponentModel.DataAnnotations;

namespace FuFuShop.Model.Entities
{
    /// <summary>
    /// 提货单表
    /// </summary>
    [SugarTable("FuFuShop_BillLading", TableDescription = "提货单表")]
    public partial class BillLading
    {
        /// <summary>
        /// 提货单表
        /// </summary>
        public BillLading()
        {
        }

        /// <summary>
        /// 提货单号
        /// </summary>
        [Display(Name = "提货单号")]
        [SugarColumn(ColumnDescription = "提货单号", IsPrimaryKey = true)]
        [Required(ErrorMessage = "请输入{0}")]
        [StringLength(20, ErrorMessage = "【{0}】不能超过{1}字符长度")]
        public string id { get; set; }
        /// <summary>
        /// 订单号
        /// </summary>
        [Display(Name = "订单号")]
        [SugarColumn(ColumnDescription = "订单号")]
        [Required(ErrorMessage = "请输入{0}")]
        [StringLength(20, ErrorMessage = "【{0}】不能超过{1}字符长度")]
        public string orderId { get; set; }
        /// <summary>
        /// 提货门店ID
        /// </summary>
        [Display(Name = "提货门店ID")]
        [SugarColumn(ColumnDescription = "提货门店ID")]
        [Required(ErrorMessage = "请输入{0}")]
        public int storeId { get; set; }
        /// <summary>
        /// 提货人姓名
        /// </summary>
        [Display(Name = "提货人姓名")]
        [SugarColumn(ColumnDescription = "提货人姓名", IsNullable = true)]
        [StringLength(30, ErrorMessage = "【{0}】不能超过{1}字符长度")]
        public string name { get; set; }
        /// <summary>
        /// 提货手机号
        /// </summary>
        [Display(Name = "提货手机号")]
        [SugarColumn(ColumnDescription = "提货手机号", IsNullable = true)]
        [StringLength(15, ErrorMessage = "【{0}】不能超过{1}字符长度")]
        public string mobile { get; set; }
        /// <summary>
        /// 处理店员ID
        /// </summary>
        [Display(Name = "处理店员ID")]
        [SugarColumn(ColumnDescription = "处理店员ID")]
        [Required(ErrorMessage = "请输入{0}")]
        public int clerkId { get; set; }
        /// <summary>
        /// 提货时间
        /// </summary>
        [Display(Name = "提货时间")]
        [SugarColumn(ColumnDescription = "提货时间", IsNullable = true)]
        public System.DateTime? pickUpTime { get; set; }
        /// <summary>
        /// 是否提货
        /// </summary>
        [Display(Name = "是否提货")]
        [SugarColumn(ColumnDescription = "是否提货")]
        [Required(ErrorMessage = "请输入{0}")]
        public System.Boolean status { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        [Display(Name = "创建时间")]
        [SugarColumn(ColumnDescription = "创建时间")]
        [Required(ErrorMessage = "请输入{0}")]
        public System.DateTime createTime { get; set; }
        /// <summary>
        /// 更新时间
        /// </summary>
        [Display(Name = "更新时间")]
        [SugarColumn(ColumnDescription = "更新时间", IsNullable = true)]
        public System.DateTime? updateTime { get; set; }
        /// <summary>
        /// 删除时间
        /// </summary>
        [Display(Name = "删除时间")]
        [SugarColumn(ColumnDescription = "删除时间")]
        [Required(ErrorMessage = "请输入{0}")]
        public System.Boolean isDel { get; set; }
    }
}