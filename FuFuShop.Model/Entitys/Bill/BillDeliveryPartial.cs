
using SqlSugar;

namespace CoreCms.Net.Model.Entities
{
    /// <summary>
    /// 发货单表
    /// </summary>
    public partial class CoreCmsBillDelivery
    {

        /// <summary>
        /// 物流名称
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public System.String logiName { get; set; }


        /// <summary>
        /// 所属区域三级全名
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public System.String shipAreaIdName { get; set; }

    }
}
