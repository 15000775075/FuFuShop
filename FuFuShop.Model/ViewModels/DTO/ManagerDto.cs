

using System.ComponentModel;

namespace FuFuShop.Model.ViewModels.DTO
{
    /// <summary>
    /// </summary>
    public class ManagerDto
    {
        /// <summary>
        ///     序列
        /// </summary>
        [DisplayName("序列")]
        public int Id { get; set; }

        /// <summary>
        ///     用户名
        /// </summary>
        [DisplayName("用户名")]
        public string UserName { get; set; }

        /// <summary>
        ///     昵称
        /// </summary>
        [DisplayName("昵称")]
        public string NickName { get; set; }

        /// <summary>
        ///     描述
        /// </summary>
        [DisplayName("描述")]
        public string Description { get; set; }
    }
}