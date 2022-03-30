/***********************************************************************
 *            Project: CoreCms
 *        ProjectName: 核心内容管理系统                                
 *                Web: https://www.corecms.net                      
 *             Author: 大灰灰                                          
 *              Email: jianweie@163.com                                
 *         CreateTime: 2021/8/19 15:38:54
 *        Description: 暂无
 ***********************************************************************/

namespace FuFuShop.Model.ViewModels.Excel
{
    /// <summary>
    /// 导出excel头字段名称信息
    /// </summary>
    public class CellValueItem
    {
        public string name { get; set; }
        public int width { get; set; } = 10 * 256;


    }
}
