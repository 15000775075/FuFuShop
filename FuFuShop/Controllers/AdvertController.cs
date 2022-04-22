using FuFuShop.Common.Auth.HttpContextUser;
using FuFuShop.Model.Entities;
using FuFuShop.Model.FromBody;
using FuFuShop.Model.ViewModels.UI;
using FuFuShop.Services;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;

namespace FuFuShop.Controllers
{
    /// <summary>
    /// 广告api控制器
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AdvertController : ControllerBase
    {

        private readonly IHttpContextUser _user;
        private readonly IAdvertPositionServices _advertPositionServices;
        private readonly IAdvertisementServices _advertisementServices;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="user"></param>
        /// <param name="articleServices"></param>
        /// <param name="advertPositionServices"></param>
        /// <param name="advertisementServices"></param>
        public AdvertController(IHttpContextUser user
            , IAdvertPositionServices advertPositionServices
            , IAdvertisementServices advertisementServices
            )
        {
            _user = user;
            _advertPositionServices = advertPositionServices;
            _advertisementServices = advertisementServices;
        }

        #region 获取广告列表=============================================================================
        /// <summary>
        /// 获取广告列表
        /// </summary>
        /// <param name="entity">FMPageByIntId</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<WebApiCallBack> GetAdvertList([FromBody] FMPageByIntId entity)
        {
            var jm = new WebApiCallBack();

            var list = await _advertisementServices.QueryPageAsync(p => p.code == entity.where, p => p.createTime, OrderByType.Desc,
                entity.page, entity.limit);
            jm.status = true;
            jm.data = list;

            return jm;

        }
        #endregion

        #region 获取广告位置信息=============================================================================
        /// <summary>
        /// 获取广告位置信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<WebApiCallBack> GetPositionList([FromBody] WxAdvert entity)
        {
            var jm = new WebApiCallBack();

            var position = await _advertPositionServices.QueryListByClauseAsync(p => p.isEnable && p.code == entity.codes);
            if (!position.Any())
            {
                return jm;
            }
            var ids = position.Select(p => p.id).ToList();
            var isement = await _advertisementServices.QueryListByClauseAsync(p => ids.Contains(p.positionId));

            Dictionary<string, List<Advertisement>> list = new Dictionary<string, List<Advertisement>>();
            list.Add(entity.codes, isement);

            jm.status = true;
            jm.data = list;

            return jm;

        }
        #endregion


    }
}
