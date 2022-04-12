
namespace FuFuShop.Model.ViewModels.DTO
{
    public class WxGoodCategoryDto
    {
        /// <summary>
        ///     序列
        /// </summary>
        public int id { get; set; }

        /// <summary>
        ///     标题
        /// </summary>
        public string name { get; set; }

        /// <summary>
        ///     排序
        /// </summary>
        public int sort { get; set; }

        /// <summary>
        ///     图片地址
        /// </summary>
        public string imageUrl { get; set; }

        public List<WxGoodCategoryChild> child { get; set; }
    }

    public class WxGoodCategoryChild
    {
        /// <summary>
        ///     序列
        /// </summary>
        public int id { get; set; }

        /// <summary>
        ///     标题
        /// </summary>
        public string name { get; set; }

        /// <summary>
        ///     排序
        /// </summary>
        public int sort { get; set; }

        /// <summary>
        ///     图片地址
        /// </summary>
        public string imageUrl { get; set; }
    }
}