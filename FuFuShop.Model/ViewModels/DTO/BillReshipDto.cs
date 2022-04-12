
namespace FuFuShop.Model.ViewModels.DTO
{
    /// <summary>
    /// </summary>
    public class reshipGoods
    {
        /// <summary>
        ///     售后商品数量，包含申请中和审核通过的
        /// </summary>
        public int reshipNums { get; set; } = 0;

        /// <summary>
        ///     已发货的商品进行退货的数量
        /// </summary>
        public int reshipedNums { get; set; } = 0;
    }
}