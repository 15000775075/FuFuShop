namespace FuFuShop.Model.FromBody
{
    /// <summary>
    ///     用户登录验证实体
    /// </summary>
    public class FMLogin
    {
        public string userName { get; set; }
        public string password { get; set; }
    }


    /// <summary>
    ///     用户登录验证实体
    /// </summary>
    public class FMEditLoginUserPassWord
    {
        public string oldPassword { get; set; }
        public string password { get; set; }
        public string repassword { get; set; }
    }
}