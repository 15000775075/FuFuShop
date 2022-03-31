
namespace FuFuShop.Model.ViewModels.DTO
{
    /// <summary>
    ///     OrderToAftersales返回类
    /// </summary>
    public class OrderToAftersalesDto
    {
        public decimal refundMoney { get; set; } = 0;

        public Dictionary<int, reshipGoods> reshipGoods { get; set; } = null;

        // public List<BillAftersales> billAftersales { get; set; } = new();
    }
}