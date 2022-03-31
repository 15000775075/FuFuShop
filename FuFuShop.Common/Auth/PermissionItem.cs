namespace FuFuShop.Common.Auth
{
    /// <summary>
    /// 用户或角色或其他凭据实体
    /// </summary>
    public class PermissionItem
    {
        /// <summary>
        /// 用户或角色或其他凭据名称
        /// </summary>
        public virtual string Role { get; set; }
        /// <summary>
        /// 请求Url
        /// </summary>
        public virtual string Url { get; set; }
        /// <summary>
        /// 权限标识
        /// </summary>
        public virtual string Authority { get; set; }
        /// <summary>
        /// 路由标识Url
        /// </summary>
        public virtual string RouteUrl { get; set; }
    }
}
