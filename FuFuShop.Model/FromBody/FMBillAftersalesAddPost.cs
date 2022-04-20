using Newtonsoft.Json.Linq;

namespace FuFuShop.Model.FromBody
{
    /// <summary>
    /// 后台审核售后单提交参数
    /// </summary>
    public class FMBillAftersalesAddPost
    {
        public string aftersalesId { get; set; }
        public int status { get; set; }
        public int type { get; set; }
        public decimal refund { get; set; }
        public string mark { get; set; }
        public JArray items { get; set; }

    }
}
