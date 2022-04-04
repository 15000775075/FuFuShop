using SqlSugar;
using System.ComponentModel.DataAnnotations;

namespace FuFuShop.Model.Entities.User
{
    /// <summary>
    /// 用户地址表
    /// </summary>
    [SugarTable("FuFuShop_UserShip", TableDescription = "用户地址表")]
    public partial class UserShip
    {
        /// <summary>
        /// 用户地址表
        /// </summary>
        public UserShip()
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
        /// 用户id 关联user.id
        /// </summary>
        [Display(Name = "用户id 关联user.id")]
        [SugarColumn(ColumnDescription = "用户id 关联user.id")]
        [Required(ErrorMessage = "请输入{0}")]
        public int userId { get; set; }
        /// <summary>
        /// 收货地区ID
        /// </summary>
        [Display(Name = "收货地区ID")]
        [SugarColumn(ColumnDescription = "收货地区ID")]
        [Required(ErrorMessage = "请输入{0}")]
        public int areaId { get; set; }
        /// <summary>
        /// 收货详细地址
        /// </summary>
        [Display(Name = "收货详细地址")]
        [SugarColumn(ColumnDescription = "收货详细地址", IsNullable = true)]
        [StringLength(200, ErrorMessage = "【{0}】不能超过{1}字符长度")]
        public string address { get; set; }
        /// <summary>
        /// 收货人姓名
        /// </summary>
        [Display(Name = "收货人姓名")]
        [SugarColumn(ColumnDescription = "收货人姓名", IsNullable = true)]
        [StringLength(50, ErrorMessage = "【{0}】不能超过{1}字符长度")]
        public string name { get; set; }
        /// <summary>
        /// 收货电话
        /// </summary>
        [Display(Name = "收货电话")]
        [SugarColumn(ColumnDescription = "收货电话", IsNullable = true)]
        [StringLength(50, ErrorMessage = "【{0}】不能超过{1}字符长度")]
        public string mobile { get; set; }
        /// <summary>
        /// 是否默认
        /// </summary>
        [Display(Name = "是否默认")]
        [SugarColumn(ColumnDescription = "是否默认")]
        [Required(ErrorMessage = "请输入{0}")]
        public bool isDefault { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        [Display(Name = "创建时间")]
        [SugarColumn(ColumnDescription = "创建时间", IsNullable = true)]
        public DateTime? createTime { get; set; }
        /// <summary>
        /// 更新时间
        /// </summary>
        [Display(Name = "更新时间")]
        [SugarColumn(ColumnDescription = "更新时间", IsNullable = true)]
        public DateTime? updateTime { get; set; }
    }
}