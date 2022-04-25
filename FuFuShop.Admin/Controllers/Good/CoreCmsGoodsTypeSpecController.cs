/***********************************************************************
 *            Project: 
 *        ProjectName: 核心内容管理系统                                
 *                Web: https://www..net                      
 *             Author: 大灰灰                                          
 *              Email: jianweie@163.com                                
 *         CreateTime: 2021/1/31 21:45:10
 *        Description: 暂无
 ***********************************************************************/

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
using System.Linq.Expressions;

namespace FuFuShop.Admin.Controllers.Good
{
    /// <summary>
    ///     商品类型属性表
    /// </summary>
    [Description("商品类型属性表")]
    [Route("api/[controller]/[action]")]
    [ApiController]
    [RequiredErrorForAdmin]
    [Authorize(Permissions.Name)]
    public class CoreCmsGoodsTypeSpecController : ControllerBase
    {
        private readonly IGoodsTypeSpecServices _GoodsTypeSpecServices;
        private readonly IGoodsTypeSpecValueServices _valueServices;
        private readonly IWebHostEnvironment _webHostEnvironment;

        /// <summary>
        ///     构造函数
        /// </summary>
        public CoreCmsGoodsTypeSpecController(IWebHostEnvironment webHostEnvironment
            , IGoodsTypeSpecServices GoodsTypeSpecServices
            , IGoodsTypeSpecValueServices GoodsTypeSpecValueServices)
        {
            _webHostEnvironment = webHostEnvironment;
            _GoodsTypeSpecServices = GoodsTypeSpecServices;
            _valueServices = GoodsTypeSpecValueServices;
        }

        #region 获取列表============================================================

        // POST: Api/GoodsTypeSpec/GetPageList
        /// <summary>
        ///     获取列表
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Description("获取列表")]
        public async Task<AdminUiCallBack> GetPageList()
        {
            var jm = new AdminUiCallBack();
            var pageCurrent = Request.Form["page"].FirstOrDefault().ObjectToInt(1);
            var pageSize = Request.Form["limit"].FirstOrDefault().ObjectToInt(30);
            var where = PredicateBuilder.True<GoodsTypeSpec>();
            //获取排序字段
            var orderField = Request.Form["orderField"].FirstOrDefault();
            Expression<Func<GoodsTypeSpec, object>> orderEx;
            switch (orderField)
            {
                case "id":
                    orderEx = p => p.id;
                    break;
                case "name":
                    orderEx = p => p.name;
                    break;
                case "sort":
                    orderEx = p => p.sort;
                    break;
                default:
                    orderEx = p => p.id;
                    break;
            }

            //设置排序方式
            var orderDirection = Request.Form["orderDirection"].FirstOrDefault();
            var orderBy = orderDirection switch
            {
                "asc" => OrderByType.Asc,
                "desc" => OrderByType.Desc,
                _ => OrderByType.Desc
            };
            //查询筛选

            //序列 int
            var id = Request.Form["id"].FirstOrDefault().ObjectToInt(0);
            if (id > 0) @where = @where.And(p => p.id == id);
            //属性名称 nvarchar
            var name = Request.Form["name"].FirstOrDefault();
            if (!string.IsNullOrEmpty(name)) @where = @where.And(p => p.name.Contains(name));
            //属性排序 int
            var sort = Request.Form["sort"].FirstOrDefault().ObjectToInt(0);
            if (sort > 0) @where = @where.And(p => p.sort == sort);
            //获取数据
            var list = await _GoodsTypeSpecServices.QueryPageAsync(where, orderEx, orderBy, pageCurrent,
                pageSize);

            if (list != null && list.Any())
            {
                var ids = list.Select(p => p.id).ToList();
                if (ids.Any())
                {
                    var values = await _valueServices.QueryListByClauseAsync(p => ids.Contains(p.specId));
                    foreach (var item in list)
                        item.specValues = values.Where(p => p.specId == item.id).OrderBy(p => p.sort).ToList();
                }
            }

            //返回数据
            jm.data = list;
            jm.code = 0;
            jm.count = list.TotalCount;
            jm.msg = "数据调用成功!";
            return jm;
        }

        #endregion

        #region 首页数据============================================================

        // POST: Api/GoodsTypeSpec/GetIndex
        /// <summary>
        ///     首页数据
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

        // POST: Api/GoodsTypeSpec/GetCreate
        /// <summary>
        ///     创建数据
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

        // POST: Api/GoodsTypeSpec/DoCreate
        /// <summary>
        ///     创建提交
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost]
        [Description("创建提交")]
        public async Task<AdminUiCallBack> DoCreate([FromBody] FmGoodsTypeSpecInsert entity)
        {
            var jm = await _GoodsTypeSpecServices.InsertAsync(entity);

            if (jm.code == 0)
            {
                //获取SKU列表
                var skuList = await _GoodsTypeSpecServices.QueryListByClauseAsync(p => p.id > 0, p => p.id, OrderByType.Desc, true);
                jm.data = new
                {
                    skuList
                };

            }
            return jm;
        }

        #endregion

        #region 编辑数据============================================================

        // POST: Api/GoodsTypeSpec/GetEdit
        /// <summary>
        ///     编辑数据
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost]
        [Description("编辑数据")]
        public async Task<AdminUiCallBack> GetEdit([FromBody] FMIntId entity)
        {
            var jm = new AdminUiCallBack();
            var model = await _GoodsTypeSpecServices.QueryByIdAsync(entity.id);
            if (model == null)
            {
                jm.msg = "不存在此信息";
                return jm;
            }

            var values =
                await _valueServices.QueryListByClauseAsync(p => p.specId == model.id, p => p.sort,
                    OrderByType.Asc);
            model.specValues = values;
            jm.code = 0;
            jm.data = model;

            return jm;
        }

        #endregion

        #region 编辑提交============================================================

        // POST: Admins/GoodsTypeSpec/Edit
        /// <summary>
        ///     编辑提交
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost]
        [Description("编辑提交")]
        public async Task<AdminUiCallBack> DoEdit([FromBody] FmGoodsTypeSpecUpdate entity)
        {
            var jm = await _GoodsTypeSpecServices.UpdateAsync(entity);
            return jm;
        }

        #endregion

        #region 删除数据============================================================

        // POST: Api/GoodsTypeSpec/DoDelete/10
        /// <summary>
        ///     单选删除
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost]
        [Description("单选删除")]
        public async Task<AdminUiCallBack> DoDelete([FromBody] FMIntId entity)
        {
            var jm = new AdminUiCallBack();

            var model = await _GoodsTypeSpecServices.QueryByIdAsync(entity.id);
            if (model == null)
            {
                jm.msg = GlobalConstVars.DataisNo;
                return jm;
            }
            jm = await _GoodsTypeSpecServices.DeleteByIdAsync(entity.id);

            return jm;
        }

        #endregion
    }
}