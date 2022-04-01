namespace FuFuShop.Model.FromBody
{
    internal class FMProducts
    {
    }


    /// <summary>
    ///     获取子规格
    /// </summary>
    public class ItemSpesDesc
    {
        public string name { get; set; } = string.Empty;
        public bool isDefault { get; set; } = false;
        public int productId { get; set; } = 0;
    }


    /// <summary>
    ///     获取相应规格
    /// </summary>
    public class DefaultSpesDesc
    {
        public string name { get; set; } = string.Empty;
        public bool isDefault { get; set; } = false;
        public int productId { get; set; } = 0;
    }
}