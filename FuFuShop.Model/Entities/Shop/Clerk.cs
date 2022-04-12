/***********************************************************************
 *            Project: 
 *        ProjectName: 核心内容管理系统                                
 *                Web: https://www..net                      
 *             Author: 大灰灰                                          
 *              Email: jianweie@163.com
 *         CreateTime: 2021-06-08 22:14:58
 *        Description: 暂无
***********************************************************************/
using SqlSugar;
using System.ComponentModel.DataAnnotations;

namespace FuFuShop.Model.Entities
{
    /// <summary>
    /// 店铺店员关联表
    /// </summary>
    [SugarTable("FuFuShop_Clerk", TableDescription = "店铺店员关联表")]
    public partial class Clerk
    {
        /// <summary>
        /// 店铺店员关联表
        /// </summary>
        public Clerk()
        {
        }

        /// <summary>
        /// 序列
        /// </summary>
        [Display(Name = "序列")]
        [SugarColumn(ColumnDescription = "序列", IsPrimaryKey = true, IsIdentity = true)]
        [Required(ErrorMessage = "请输入{0}")]
        public System.Int32 id { get; set; }
        /// <summary>
        /// 店铺ID
        /// </summary>
        [Display(Name = "店铺ID")]
        [SugarColumn(ColumnDescription = "店铺ID")]
        [Required(ErrorMessage = "请输入{0}")]
        public System.Int32 storeId { get; set; }
        /// <summary>
        /// 用户ID
        /// </summary>
        [Display(Name = "用户ID")]
        [SugarColumn(ColumnDescription = "用户ID")]
        [Required(ErrorMessage = "请输入{0}")]
        public System.Int32 userId { get; set; }
        /// <summary>
        /// 是否删除
        /// </summary>
        [Display(Name = "是否删除")]
        [SugarColumn(ColumnDescription = "是否删除")]
        [Required(ErrorMessage = "请输入{0}")]
        public System.Boolean isDel { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        [Display(Name = "创建时间")]
        [SugarColumn(ColumnDescription = "创建时间")]
        [Required(ErrorMessage = "请输入{0}")]
        public System.DateTime createTime { get; set; }
        /// <summary>
        /// 删除时间
        /// </summary>
        [Display(Name = "删除时间")]
        [SugarColumn(ColumnDescription = "删除时间", IsNullable = true)]
        public System.DateTime? updateTime { get; set; }
    }
}