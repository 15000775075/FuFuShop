
using SqlSugar;
using System.ComponentModel.DataAnnotations;

namespace FuFuShop.Model.Entities
{
    /// <summary>
    /// 支付方式表
    /// </summary>
    [SugarTable("FuFuShop_Payments", TableDescription = "支付方式表")]
    public partial class Payments
    {
        /// <summary>
        /// 支付方式表
        /// </summary>
        public Payments()
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
        /// 支付类型名称
        /// </summary>
        [Display(Name = "支付类型名称")]
        [SugarColumn(ColumnDescription = "支付类型名称", IsNullable = true)]
        [StringLength(50, ErrorMessage = "【{0}】不能超过{1}字符长度")]
        public string name { get; set; }
        /// <summary>
        /// 支付类型编码
        /// </summary>
        [Display(Name = "支付类型编码")]
        [SugarColumn(ColumnDescription = "支付类型编码", IsNullable = true)]
        [StringLength(50, ErrorMessage = "【{0}】不能超过{1}字符长度")]
        public string code { get; set; }
        /// <summary>
        /// 是否线上支付
        /// </summary>
        [Display(Name = "是否线上支付")]
        [SugarColumn(ColumnDescription = "是否线上支付")]
        [Required(ErrorMessage = "请输入{0}")]
        public System.Boolean isOnline { get; set; }
        /// <summary>
        /// 参数
        /// </summary>
        [Display(Name = "参数")]
        [SugarColumn(ColumnDescription = "参数", IsNullable = true)]
        public string parameters { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        [Display(Name = "排序")]
        [SugarColumn(ColumnDescription = "排序")]
        [Required(ErrorMessage = "请输入{0}")]
        public int sort { get; set; }
        /// <summary>
        /// 方式描述
        /// </summary>
        [Display(Name = "方式描述")]
        [SugarColumn(ColumnDescription = "方式描述", IsNullable = true)]
        [StringLength(200, ErrorMessage = "【{0}】不能超过{1}字符长度")]
        public string memo { get; set; }
        /// <summary>
        /// 是否启用
        /// </summary>
        [Display(Name = "是否启用")]
        [SugarColumn(ColumnDescription = "是否启用")]
        [Required(ErrorMessage = "请输入{0}")]
        public System.Boolean isEnable { get; set; }
    }
}