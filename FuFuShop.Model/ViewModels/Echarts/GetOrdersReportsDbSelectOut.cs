

namespace FuFuShop.Model.ViewModels.Echarts
{
    /// <summary>
    ///     获取订单销量统计查询数据库返回sql组合后的结果集
    /// </summary>
    public class GetOrdersReportsDbSelectOut
    {
        /// <summary>
        ///     排序
        /// </summary>
        public int number { get; set; }

        /// <summary>
        ///     金额
        /// </summary>
        public decimal val { get; set; }

        /// <summary>
        ///     数量
        /// </summary>
        public int num { get; set; }
    }
}