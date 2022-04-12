
using SqlSugar;

namespace FuFuShop.Model.Entities
{
    /// <summary>
    /// 提货单表
    /// </summary>
    public partial class BillLading
    {

        /// <summary>
        /// 关联门店名称
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public string storeName { get; set; }


        /// <summary>
        /// 状态中文描述
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public string statusName { get; set; }



        /// <summary>
        /// 店员昵称
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public string clerkIdName { get; set; }


        /// <summary>
        /// 关联订单项目
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public List<OrderItem> orderItems { get; set; }

    }
}
