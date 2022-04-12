
using SqlSugar;

namespace FuFuShop.Model.Entities
{
    /// <summary>
    /// 发货单表
    /// </summary>
    public partial class BillDelivery
    {

        /// <summary>
        /// 物流名称
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public string logiName { get; set; }


        /// <summary>
        /// 所属区域三级全名
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public string shipAreaIdName { get; set; }

    }
}
