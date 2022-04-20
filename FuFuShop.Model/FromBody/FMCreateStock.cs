using FuFuShop.Model.Entities;

namespace FuFuShop.Model.FromBody
{
    public class FMCreateStock
    {
        /// <summary>
        ///     广告位置
        /// </summary>
        public Stock model { get; set; }


        public List<FMCreateStockItem> items { get; set; }
    }

    public class FMCreateStockItem
    {
        public int nums { get; set; }
        public int productId { get; set; }
    }
}