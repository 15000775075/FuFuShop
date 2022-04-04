

using SqlSugar;

namespace FuFuShop.Model.Entities
{
    /// <summary>
    ///     订单记录表
    /// </summary>
    public partial class OrderLog
    {
        /// <summary>
        ///     类型说明
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public string typeText { get; set; }
    }
}