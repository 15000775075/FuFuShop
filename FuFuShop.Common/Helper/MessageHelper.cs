using FuFuShop.Common.AppSettings;
using Newtonsoft.Json.Linq;

namespace FuFuShop.Common.Helper
{
    public static class MessageHelper
    {
        /// <summary>
        /// 根据编码获取消息内容
        /// </summary>
        /// <param name="code"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static string GetTemp(string code, JObject parameters)
        {
            string msg = string.Empty;
            if (code == GlobalEnumVars.PlatformMessageTypes.CreateOrder.ToString())
            {
                msg = "订单创建成功。";
            }
            else if (code == GlobalEnumVars.PlatformMessageTypes.OrderPayed.ToString())
            {
                msg = "恭喜您，订单支付成功，祝您购物愉快。";
            }
            else if (code == GlobalEnumVars.PlatformMessageTypes.RemindOrderPay.ToString())
            {
                msg = "您的订单还有3个小时就要取消了，请立即进行支付。";
            }
            else if (code == GlobalEnumVars.PlatformMessageTypes.DeliveryNotice.ToString())
            {
                msg = "你好，你的订单已经发货。";
            }
            else if (code == GlobalEnumVars.PlatformMessageTypes.AfterSalesPass.ToString())
            {
                msg = "你好，您的售后已经通过。";
            }
            else if (code == GlobalEnumVars.PlatformMessageTypes.RefundSuccess.ToString())
            {
                msg = "用户你好，你的退款已经处理，请确认。";
            }
            else if (code == GlobalEnumVars.ShopMessageTypes.AftersalesAdd.ToString())
            {
                msg = "你好，有新的售后订单了，请及时处理。";
            }
            else if (code == GlobalEnumVars.PlatformMessageTypes.SellerOrderNotice.ToString())
            {
                msg = "您有新的订单了，请及时处理。";
            }
            return msg;
        }



    }
}
