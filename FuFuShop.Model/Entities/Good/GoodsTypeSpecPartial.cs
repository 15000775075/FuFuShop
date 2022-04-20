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

namespace FuFuShop.Model.Entities
{
    /// <summary>
    ///     商品类型属性表
    /// </summary>
    public partial class GoodsTypeSpec
    {
        /// <summary>
        ///     子类
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public List<GoodsTypeSpecValue> specValues { get; set; } = new();
    }
}