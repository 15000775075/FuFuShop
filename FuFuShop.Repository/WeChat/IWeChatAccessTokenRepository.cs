using FuFuShop.Model.Entities;
using FuFuShop.Repository.BaseRepository;

namespace FuFuShop.Repository
{
    /// <summary>
    ///     微信授权交互 工厂接口
    /// </summary>
    public interface IWeChatAccessTokenRepository : IBaseRepository<WeChatAccessToken>
    {
    }
}