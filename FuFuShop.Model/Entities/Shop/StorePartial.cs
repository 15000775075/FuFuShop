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
    ///     门店表
    /// </summary>
    public partial class Store
    {
        /// <summary>
        ///     全名详细地址
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public string allAddress { get; set; }


        /// <summary>
        ///     距离说明
        /// </summary>
        [Display(Name = "距离说明")]
        [SugarColumn(IsIgnore = true)]
        public string distanceStr { get; set; }
    }
}