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
    ///     用户对表的提交记录
    /// </summary>
    public partial class FormSubmit
    {
        /// <summary>
        ///     用户昵称
        /// </summary>
        [Display(Name = "微信昵称")]
        [SugarColumn(IsIgnore = true)]
        public string userName { get; set; }

        /// <summary>
        ///     用户头像
        /// </summary>
        [Display(Name = "用户头像")]
        [SugarColumn(IsIgnore = true)]
        public string avatarImage { get; set; }
    }
}