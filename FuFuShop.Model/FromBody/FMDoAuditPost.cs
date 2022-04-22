namespace FuFuShop.Model.FromBody
{
    /// <summary>
    /// 后台审核退款单提交参数
    /// </summary>
    public class FMDoAuditPost
    {
        /// <summary>
        /// 状态
        /// </summary>
        public int status { get; set; }

        /// <summary>
        /// 退款方式
        /// </summary>
        public string paymentCode { get; set; }

        /// <summary>
        /// 退款单号
        /// </summary>
        public string refundId { get; set; }

    }
}