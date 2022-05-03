/***********************************************************************
 *            Project: 
 *        ProjectName: 核心内容管理系统                                
 *                Web: https://www..net                      
 *             Author: 大灰灰                                          
 *              Email: jianweie@163.com                                
 *         CreateTime: 2021/1/31 21:45:10
 *        Description: 暂无
 ***********************************************************************/

using SqlSugar;
using System.ComponentModel.DataAnnotations;

namespace FuFuShop.Model.Entities
{
    /// <summary>
    ///     用户余额表
    /// </summary>
    public partial class UserBalance
    {
        /// <summary>
        ///     操作类型说明
        /// </summary>
        [Display(Name = "操作类型说明")]
        [SugarColumn(IsIgnore = true)]
        public string typeName { get; set; }

        /// <summary>
        ///     用户昵称
        /// </summary>
        [Display(Name = "用户昵称")]
        [SugarColumn(IsIgnore = true)]
        public string userNickName { get; set; }
    }
}