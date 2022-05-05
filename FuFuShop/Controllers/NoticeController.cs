


using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FuFuShop.Common.Auth.HttpContextUser;
using FuFuShop.Model.FromBody;
using FuFuShop.Model.ViewModels.UI;
using FuFuShop.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;

namespace FuFuShop.Controllers
{
    /// <summary>
    /// 公告控制器
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class NoticeController : ControllerBase
    {
        private IHttpContextUser _user;
        private INoticeServices _noticeServices;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="user"></param>
        /// <param name="noticeServices"></param>
        public NoticeController(IHttpContextUser user, INoticeServices noticeServices)
        {
            _user = user;
            _noticeServices = noticeServices;
        }


        #region 列表
        /// <summary>
        /// 列表
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<WebApiCallBack> NoticeList([FromBody] FMPageByIntId entity)
        {
            var jm = new WebApiCallBack();

            var list = await _noticeServices.QueryPageAsync(p => p.isDel == false, p => p.createTime, OrderByType.Desc,
                entity.page, entity.limit);
            jm.status = true;
            jm.data = list;

            return jm;

        }

        #endregion



        /// <summary>
        /// 获取单个公告内容
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<WebApiCallBack> NoticeInfo([FromBody] FMIntId entity)
        {
            var jm = new WebApiCallBack();

            var model = await _noticeServices.QueryByIdAsync(entity.id);
            if (model == null)
            {
                jm.msg = "数据获取失败";
                return jm;
            }
            jm.status = true;
            jm.data = model;
            return jm;
        }

    }
}
