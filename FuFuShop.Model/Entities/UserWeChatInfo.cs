using SqlSugar;
using System.ComponentModel.DataAnnotations;

namespace FuFuShop.Model.Entities
{
    /// 用户表
    /// </summary>
    [SugarTable("FuFuShop_UserWeChatInfo", TableDescription = "用户表")]
    public partial class UserWeChatInfo
    {
        /// <summary>
        /// 用户表
        /// </summary>
        public UserWeChatInfo()
        {
        }

        /// <summary>
        /// 用户ID
        /// </summary>
        [Display(Name = "用户ID")]
        [SugarColumn(ColumnDescription = "用户ID", IsPrimaryKey = true, IsIdentity = true)]
        [Required(ErrorMessage = "请输入{0}")]
        public int id { get; set; }
        /// <summary>
        /// 第三方登录类型
        /// </summary>
        [Display(Name = "第三方登录类型")]
        [SugarColumn(ColumnDescription = "第三方登录类型", IsNullable = true)]
        public int? type { get; set; }
        /// <summary>
        /// 关联用户表
        /// </summary>
        [Display(Name = "关联用户表")]
        [SugarColumn(ColumnDescription = "关联用户表")]
        [Required(ErrorMessage = "请输入{0}")]
        public int userId { get; set; }
        /// <summary>
        /// openId
        /// </summary>
        [Display(Name = "openId")]
        [SugarColumn(ColumnDescription = "openId", IsNullable = true)]
        [StringLength(50, ErrorMessage = "【{0}】不能超过{1}字符长度")]
        public string openid { get; set; }
        /// <summary>
        /// 缓存key
        /// </summary>
        [Display(Name = "缓存key")]
        [SugarColumn(ColumnDescription = "缓存key", IsNullable = true)]
        [StringLength(255, ErrorMessage = "【{0}】不能超过{1}字符长度")]
        public string sessionKey { get; set; }
        /// <summary>
        /// unionid
        /// </summary>
        [Display(Name = "unionid")]
        [SugarColumn(ColumnDescription = "unionid", IsNullable = true)]
        [StringLength(50, ErrorMessage = "【{0}】不能超过{1}字符长度")]
        public string unionId { get; set; }
        /// <summary>
        /// 头像
        /// </summary>
        [Display(Name = "头像")]
        [SugarColumn(ColumnDescription = "头像", IsNullable = true)]
        [StringLength(255, ErrorMessage = "【{0}】不能超过{1}字符长度")]
        public string avatar { get; set; }
        /// <summary>
        /// 昵称
        /// </summary>
        [Display(Name = "昵称")]
        [SugarColumn(ColumnDescription = "昵称", IsNullable = true)]
        [StringLength(50, ErrorMessage = "【{0}】不能超过{1}字符长度")]
        public string nickName { get; set; }
        /// <summary>
        /// 性别
        /// </summary>
        [Display(Name = "性别")]
        [SugarColumn(ColumnDescription = "性别")]
        [Required(ErrorMessage = "请输入{0}")]
        public int gender { get; set; }
        /// <summary>
        /// 语言
        /// </summary>
        [Display(Name = "语言")]
        [SugarColumn(ColumnDescription = "语言", IsNullable = true)]
        [StringLength(50, ErrorMessage = "【{0}】不能超过{1}字符长度")]
        public string language { get; set; }
        /// <summary>
        /// 城市
        /// </summary>
        [Display(Name = "城市")]
        [SugarColumn(ColumnDescription = "城市", IsNullable = true)]
        [StringLength(80, ErrorMessage = "【{0}】不能超过{1}字符长度")]
        public string city { get; set; }
        /// <summary>
        /// 省
        /// </summary>
        [Display(Name = "省")]
        [SugarColumn(ColumnDescription = "省", IsNullable = true)]
        [StringLength(80, ErrorMessage = "【{0}】不能超过{1}字符长度")]
        public string province { get; set; }
        /// <summary>
        /// 国家
        /// </summary>
        [Display(Name = "国家")]
        [SugarColumn(ColumnDescription = "国家", IsNullable = true)]
        [StringLength(80, ErrorMessage = "【{0}】不能超过{1}字符长度")]
        public string country { get; set; }
        /// <summary>
        /// 手机号码国家编码
        /// </summary>
        [Display(Name = "手机号码国家编码")]
        [SugarColumn(ColumnDescription = "手机号码国家编码", IsNullable = true)]
        [StringLength(20, ErrorMessage = "【{0}】不能超过{1}字符长度")]
        public string countryCode { get; set; }
        /// <summary>
        /// 手机号码
        /// </summary>
        [Display(Name = "手机号码")]
        [SugarColumn(ColumnDescription = "手机号码", IsNullable = true)]
        [StringLength(20, ErrorMessage = "【{0}】不能超过{1}字符长度")]
        public string mobile { get; set; }
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