
using SqlSugar;
using System.ComponentModel.DataAnnotations;

namespace FuFuShop.Model.Entities
{
    /// <summary>
    /// 订单记录表
    /// </summary>
    [SugarTable("FuFuShop_OrderLog", TableDescription = "订单记录表")]
    public partial class OrderLog
    {
        /// <summary>
        /// 订单记录表
        /// </summary>
        public OrderLog()
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
        /// 订单ID
        /// </summary>
        [Display(Name = "订单ID")]
        [SugarColumn(ColumnDescription = "订单ID", IsNullable = true)]
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
        /// 类型
        /// </summary>
        [Display(Name = "类型")]
        [SugarColumn(ColumnDescription = "类型")]
        [Required(ErrorMessage = "请输入{0}")]
        public int type { get; set; }
        /// <summary>
        /// 描述介绍
        /// </summary>
        [Display(Name = "描述介绍")]
        [SugarColumn(ColumnDescription = "描述介绍", IsNullable = true)]
        [StringLength(100, ErrorMessage = "【{0}】不能超过{1}字符长度")]
        public string msg { get; set; }
        /// <summary>
        /// 请求的数据json
        /// </summary>
        [Display(Name = "请求的数据json")]
        [SugarColumn(ColumnDescription = "请求的数据json", IsNullable = true)]
        public string data { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        [Display(Name = "创建时间")]
        [SugarColumn(ColumnDescription = "创建时间")]
        [Required(ErrorMessage = "请输入{0}")]
        public System.DateTime createTime { get; set; }
    }
}