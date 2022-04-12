
namespace FuFuShop.Model.ViewModels.QueryMuch
{
    /// <summary>
    ///     根据订单号查询已经售后的内容.算退货商品明细
    /// </summary>
    public class QMAftersalesItems
    {
        public int orderItemsId { get; set; }
        public int nums { get; set; }
        public int status { get; set; }
        public int type { get; set; }
    }
}