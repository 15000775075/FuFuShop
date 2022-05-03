/***********************************************************************
 *            Project: 
 *        ProjectName: 核心内容管理系统                                
 *                Web: https://www..net                      
 *             Author: 大灰灰                                          
 *              Email: jianweie@163.com                                
 *         CreateTime: 2021/1/31 21:45:10
 *        Description: 暂无
 ***********************************************************************/

using FuFuShop.Model.Entities;
using FuFuShop.Model.ViewModels.UI;
using FuFuShop.Services.BaseServices;

namespace FuFuShop.Services
{
    /// <summary>
    ///     银行卡信息 服务工厂接口
    /// </summary>
    public interface IUserBankCardServices : IBaseServices<UserBankCard>
    {
        /// <summary>
        ///     我的银行卡列表
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<WebApiCallBack> GetMyBankcardsList(int userId);


        /// <summary>
        ///     我的银行卡列表
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<WebApiCallBack> AddBankCards(UserBankCard entity);


        /// <summary>
        ///     删除银行卡
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<WebApiCallBack> Removebankcard(int id, int userId);

        /// <summary>
        ///     获取用户默认银行卡信息
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<WebApiCallBack> GetDefaultBankCard(int userId);


        /// <summary>
        ///     获取银行卡组织信息
        /// </summary>
        /// <param name="cardCode"></param>
        /// <returns></returns>
        WebApiCallBack BankCardsOrganization(string cardCode);


        /// <summary>
        ///     设置默认的银行卡
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<WebApiCallBack> SetDefault(int userId, int id);


        /// <summary>
        ///     获取银行卡信息
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<WebApiCallBack> GetBankcardInfo(int userId, int id);
    }
}