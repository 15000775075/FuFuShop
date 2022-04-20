namespace FuFuShop.Model.FromBody
{
    /// <summary>
    ///     前端提交标准json键值对内容
    /// </summary>
    public class FmSerializeArray
    {
        public List<FormSerializeArray> entity { get; set; }
    }

    public class FormSerializeArray
    {
        public string name { get; set; }
        public string value { get; set; }
    }
}