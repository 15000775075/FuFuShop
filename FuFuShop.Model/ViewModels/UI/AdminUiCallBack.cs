
namespace FuFuShop.Model.ViewModels.UI
{
    /// <summary>
    ///     LayUIAdmin后端UI回调Json实体
    /// </summary>
    public class AdminUiCallBack
    {
        /// <summary>
        ///     状态码(ok = 0, error = 1, timeout = 401)
        /// </summary>
        public int code { get; set; } = 1;

        /// <summary>
        ///     可选。信息内容。
        /// </summary>
        public string msg { get; set; } = "空数据返回";

        public object data { get; set; }

        public object otherData { get; set; }

        public int count { get; set; }
    }
}