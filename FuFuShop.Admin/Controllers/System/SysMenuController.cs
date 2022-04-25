

using FuFuShop.Admin.Filter;
using FuFuShop.Common.AppSettings;
using FuFuShop.Common.Extensions;
using FuFuShop.Model.Entities;
using FuFuShop.Model.FromBody;
using FuFuShop.Model.ViewModels.UI;
using FuFuShop.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using System.ComponentModel;

namespace FuFuShop.Admin.Controllers.System
{
    /// <summary>
    /// 菜单表
    ///</summary>
    [Description("菜单表")]
    [Route("api/[controller]/[action]")]
    [ApiController]
    [RequiredErrorForAdmin]
    [Authorize(Permissions.Name)]
    public class SysMenuController : ControllerBase
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ISysMenuServices _sysMenuServices;

        /// <summary>
        /// 构造函数
        ///</summary>
        public SysMenuController(IWebHostEnvironment webHostEnvironment
            , ISysMenuServices sysMenuServices
            )
        {
            _webHostEnvironment = webHostEnvironment;
            _sysMenuServices = sysMenuServices;
        }

        #region 获取列表============================================================
        // POST: Api/SysMenu/GetPageList
        /// <summary>
        /// 获取列表
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Description("获取列表")]
        public async Task<AdminUiCallBack> GetPageList()
        {
            var jm = new AdminUiCallBack();
            var where = PredicateBuilder.True<SysMenu>();
            where = where.And(p => p.deleted == false);
            //查询筛选

            ////菜单名称 nvarchar
            //var menuName = Request.Form["menuName"].FirstOrDefault();
            //if (!string.IsNullOrEmpty(menuName))
            //{
            //    where = where.And(p => p.menuName.Contains(menuName));
            //}

            ////菜单组件地址 nvarchar
            //var component = Request.Form["component"].FirstOrDefault();
            //if (!string.IsNullOrEmpty(component))
            //{
            //    where = where.And(p => p.component.Contains(component));
            //}

            ////权限标识 nvarchar
            //var authority = Request.Form["authority"].FirstOrDefault();
            //if (!string.IsNullOrEmpty(authority))
            //{
            //    where = where.And(p => p.authority.Contains(authority));
            //}

            //获取数据
            var list = await _sysMenuServices.QueryListByClauseAsync(where, p => p.sortNumber, OrderByType.Asc);
            //返回数据
            jm.data = list;
            jm.code = 0;
            jm.count = list.Count;
            jm.msg = "数据调用成功!";
            return jm;
        }
        #endregion

        #region 首页数据============================================================
        // POST: Api/SysMenu/GetIndex
        /// <summary>
        /// 首页数据
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Description("首页数据")]
        public AdminUiCallBack GetIndex()
        {
            //返回数据
            var jm = new AdminUiCallBack { code = 0 };
            return jm;
        }
        #endregion

        #region 创建数据============================================================
        // POST: Api/SysMenu/GetCreate
        /// <summary>
        /// 创建数据
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Description("创建数据")]
        public AdminUiCallBack GetCreate()
        {
            //返回数据
            var jm = new AdminUiCallBack { code = 0 };

            return jm;
        }
        #endregion

        #region 创建提交============================================================
        // POST: Api/SysMenu/DoCreate
        /// <summary>
        /// 创建提交
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost]
        [Description("创建提交")]
        public async Task<AdminUiCallBack> DoCreate([FromBody] SysMenu entity)
        {
            var jm = new AdminUiCallBack();

            entity.createTime = DateTime.Now; ;
            jm = await _sysMenuServices.InsertAsync(entity);

            return jm;
        }
        #endregion

        #region 编辑数据============================================================
        // POST: Api/SysMenu/GetEdit
        /// <summary>
        /// 编辑数据
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost]
        [Description("编辑数据")]
        public async Task<AdminUiCallBack> GetEdit([FromBody] FMIntId entity)
        {
            var jm = new AdminUiCallBack();

            var model = await _sysMenuServices.QueryByIdAsync(entity.id);
            if (model == null)
            {
                jm.msg = "不存在此信息";
                return jm;
            }
            jm.code = 0;
            jm.data = model;

            return jm;
        }
        #endregion

        #region 编辑提交============================================================
        // POST: Api/SysMenu/Edit
        /// <summary>
        /// 编辑提交
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost]
        [Description("编辑提交")]
        public async Task<AdminUiCallBack> DoEdit([FromBody] SysMenu entity)
        {
            var jm = await _sysMenuServices.UpdateAsync(entity);
            return jm;
        }
        #endregion

        #region 删除数据============================================================
        // POST: Api/SysMenu/DoDelete/10
        /// <summary>
        /// 单选删除
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost]
        [Description("单选删除")]
        public async Task<AdminUiCallBack> DoDelete([FromBody] FMIntId entity)
        {
            var jm = new AdminUiCallBack();

            jm = await _sysMenuServices.DeleteByIdAsync(entity.id);




            return jm;




        }
        #endregion


    }
}
