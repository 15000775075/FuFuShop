using SqlSugar;

namespace FuFuShop.Model.Entities
{
    /// <summary>
    /// 退货单表
    /// </summary>
    public partial class BillAftersales
    {
        /// <summary>
        /// 商品子集
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public List<BillAftersalesItem> items { get; set; } = new List<BillAftersalesItem>();

        /// <summary>
        /// 图片子集
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public List<BillAftersalesImages> images { get; set; } = new List<BillAftersalesImages>();
        /// <summary>
        /// 退款单
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public BillRefund billRefund { get; set; }
        /// <summary>
        /// 退货单
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public BillReship billReship { get; set; }

        /// <summary>
        /// 状态说明
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public string statusName { get; set; }

        /// <summary>
        /// 用户昵称
        /// </summary>
        [SugarColumn(IsIgnore = true)]

        public string userNickName { get; set; }

        /// <summary>
        /// 关联订单数据
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public Order order { get; set; }

    }
}
