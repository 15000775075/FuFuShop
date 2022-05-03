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
using FuFuShop.Model.FromBody;
using FuFuShop.Model.ViewModels.Basics;
using FuFuShop.Model.ViewModels.DTO;
using FuFuShop.Model.ViewModels.UI;
using FuFuShop.Services.BaseServices;
using SqlSugar;
using System.Linq.Expressions;

namespace FuFuShop.Services
{
    /// <summary>
    ///     用户表 服务工厂接口
    /// </summary>
    public interface IUserServices : IBaseServices<FuFuShopUser>
    {
        /// <summary>
        ///     更新余额
        /// </summary>
        /// <param name="id"></param>
        /// <param name="money"></param>
        /// <returns></returns>
        Task<AdminUiCallBack> UpdateBalance(int id, decimal money);

        /// <summary>
        ///     更新积分
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<AdminUiCallBack> UpdatePiont(FMUpdateUserPoint entity);


        /// <summary>
        ///     获取用户的积分
        /// </summary>
        /// <param name="userId">用户序列</param>
        /// <param name="orderMoney">订单金额</param>
        /// <returns></returns>
        Task<GetUserPointResult> GetUserPoint(int userId, decimal orderMoney);


        /// <summary>
        ///     //修改用户密码，如果用户之前没有密码，那么就不校验原密码
        /// </summary>
        Task<WebApiCallBack> ChangePassword(int userId, string newPwd, string password = "");


        /// <summary>
        ///     绑定上级
        /// </summary>
        /// <param name="superiorId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<WebApiCallBack> SetMyInvite(int superiorId, int userId);


        /// <summary>
        ///     获取我的上级邀请人
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<WebApiCallBack> GetMyInvite(int userId);


        /// <summary>
        ///     获取下级推广用户数量
        /// </summary>
        /// <param name="parentId">父类序列</param>
        /// <param name="type">1获取1级，其他为2级</param>
        /// <param name="thisMonth">当月</param>
        /// <returns></returns>
        Task<int> QueryChildCountAsync(int parentId, int type = 1, bool thisMonth = false);

        /// <summary>
        ///     忘记密码，找回密码
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="newPwd"></param>
        /// <param name="cTime"></param>
        /// <returns></returns>
        Task<WebApiCallBack> ForgetPassword(string mobile, string code, string newPwd);



        /// <summary>
        ///     根据条件查询分页数据
        /// </summary>
        /// <param name="predicate">判断集合</param>
        /// <param name="orderByType">排序方式</param>
        /// <param name="pageIndex">当前页面索引</param>
        /// <param name="pageSize">分布大小</param>
        /// <param name="orderByExpression"></param>
        /// <returns></returns>
        Task<IPageList<User>> QueryPageAsync(Expression<Func<FuFuShopUser, bool>> predicate,
            Expression<Func<FuFuShopUser, object>> orderByExpression, OrderByType orderByType, int pageIndex = 1,
            int pageSize = 20);


        /// <summary>
        ///     按天统计新会员
        /// </summary>
        /// <returns></returns>
        Task<List<StatisticsOut>> Statistics(int day);


        /// <summary>
        ///     按天统计当天下单活跃会员
        /// </summary>
        /// <returns></returns>
        Task<List<StatisticsOut>> StatisticsOrder(int day);
    }
}