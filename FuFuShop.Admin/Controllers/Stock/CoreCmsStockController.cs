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
using FuFuShop.Common.Auth.HttpContextUser;
using FuFuShop.Common.Extensions;
using FuFuShop.Common.Helper;
using FuFuShop.Model.Entities;
using FuFuShop.Model.FromBody;
using FuFuShop.Model.ViewModels.UI;
using FuFuShop.Services;
using FuFuShop.Services.Good;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using System.ComponentModel;
using System.Linq.Expressions;

namespace FuFuShop.Admin.Controllers
{
    /// <summary>
    /// 库存操作表
    ///</summary>
    [Description("库存操作表")]
    [Route("api/[controller]/[action]")]
    [ApiController]
    [RequiredErrorForAdmin]
    [Authorize(Permissions.Name)]
    public class StockController : ControllerBase
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IStockServices _stockServices;
        private readonly IProductsServices _productsServices;
        private readonly IStockLogServices _stockLogServices;
        private readonly ISysUserServices _sysUserServices;
        private readonly IHttpContextUser _user;

        /// <summary>
        /// 构造函数
        ///</summary>
        public StockController(IWebHostEnvironment webHostEnvironment
            , IStockServices stockServices, IProductsServices productsServices, IHttpContextUser user, IStockLogServices stockLogServices, ISysUserServices sysUserServices)
        {
            _webHostEnvironment = webHostEnvironment;
            _stockServices = stockServices;
            _productsServices = productsServices;
            _user = user;
            _stockLogServices = stockLogServices;
            _sysUserServices = sysUserServices;
        }

        #region 获取列表============================================================
        // POST: Api/Stock/GetPageList
        /// <summary>
        /// 获取列表
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Description("获取列表")]
        public async Task<AdminUiCallBack> GetPageList()
        {
            var jm = new AdminUiCallBack();
            var pageCurrent = Request.Form["page"].FirstOrDefault().ObjectToInt(1);
            var pageSize = Request.Form["limit"].FirstOrDefault().ObjectToInt(30);
            var where = PredicateBuilder.True<Stock>();

            where = where.And(p => p.type == (int)GlobalEnumVars.StockType.In || p.type == (int)GlobalEnumVars.StockType.Out);


            //获取排序字段
            var orderField = Request.Form["orderField"].FirstOrDefault();

            Expression<Func<Stock, object>> orderEx = orderField switch
            {
                "id" => p => p.id,
                "type" => p => p.type,
                "manager" => p => p.manager,
                "memo" => p => p.memo,
                "createTime" => p => p.createTime,
                _ => p => p.createTime
            };

            //设置排序方式
            var orderDirection = Request.Form["orderDirection"].FirstOrDefault();
            var orderBy = orderDirection switch
            {
                "asc" => OrderByType.Asc,
                "desc" => OrderByType.Desc,
                _ => OrderByType.Desc
            };
            //查询筛选

            //序列 nvarchar
            var id = Request.Form["id"].FirstOrDefault();
            if (!string.IsNullOrEmpty(id))
            {
                where = where.And(p => p.id.Contains(id));
            }
            //操作类型 int
            var type = Request.Form["type"].FirstOrDefault().ObjectToInt(0);
            if (type > 0)
            {
                where = where.And(p => p.type == type);
            }
            //操作员 int
            var manager = Request.Form["manager"].FirstOrDefault().ObjectToInt(0);
            if (manager > 0)
            {
                where = where.And(p => p.manager == manager);
            }
            //备注 nvarchar
            var memo = Request.Form["memo"].FirstOrDefault();
            if (!string.IsNullOrEmpty(memo))
            {
                where = where.And(p => p.memo.Contains(memo));
            }
            //创建时间 datetime
            var createTime = Request.Form["createTime"].FirstOrDefault();
            if (!string.IsNullOrEmpty(createTime))
            {
                if (createTime.Contains("到"))
                {
                    var dts = createTime.Split("到");
                    var dtStart = dts[0].Trim().ObjectToDate();
                    where = where.And(p => p.createTime > dtStart);
                    var dtEnd = dts[1].Trim().ObjectToDate();
                    where = where.And(p => p.createTime < dtEnd);
                }
                else
                {
                    var dt = createTime.ObjectToDate();
                    where = where.And(p => p.createTime > dt);
                }
            }
            //获取数据
            var list = await _stockServices.QueryPageAsync(where, orderEx, orderBy, pageCurrent, pageSize, true);
            //返回数据
            jm.data = list;
            jm.code = 0;
            jm.count = list.TotalCount;
            jm.msg = "数据调用成功!";
            return jm;
        }
        #endregion

        #region 首页数据============================================================
        // POST: Api/Stock/GetIndex
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

            var stockType = EnumHelper.EnumToList<GlobalEnumVars.StockType>();
            stockType = stockType.Where(p => p.value < 3).ToList();
            jm.data = new
            {
                stockType,
            };
            return jm;
        }
        #endregion

        #region 创建数据============================================================
        // POST: Api/Stock/GetCreate
        /// <summary>
        /// 创建数据
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Description("创建数据")]
        public async Task<AdminUiCallBack> GetCreate()
        {
            //返回数据
            var jm = new AdminUiCallBack { code = 0 };
            var stockType = EnumHelper.EnumToList<GlobalEnumVars.StockType>();
            stockType = stockType.Where(p => p.value < 3).ToList();

            var products = await _productsServices.GetProducts();

            jm.data = new
            {
                stockType,
                products,
            };

            return jm;
        }
        #endregion

        #region 创建提交============================================================
        // POST: Api/Stock/DoCreate
        /// <summary>
        /// 创建提交
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost]
        [Description("创建提交")]
        public async Task<AdminUiCallBack> DoCreate([FromBody] FMCreateStock entity)
        {
            if (_user != null)
            {
                entity.model.manager = _user.ID;
            }
            else
            {
                entity.model.manager = 0;
            }

            var jm = await _stockServices.InsertAsync(entity);
            return jm;
        }
        #endregion

        #region 预览数据============================================================
        // POST: Api/Stock/GetDetails/10
        /// <summary>
        /// 预览数据
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost]
        [Description("预览数据")]
        public async Task<AdminUiCallBack> GetDetails([FromBody] FMStringId entity)
        {
            var jm = new AdminUiCallBack();

            var model = await _stockServices.QueryByIdAsync(entity.id, false);
            if (model == null)
            {
                jm.msg = "不存在此信息";
                return jm;
            }

            var manager = await _sysUserServices.QueryByClauseAsync(p => p.id == model.manager);
            var logs = await _stockLogServices.QueryListByClauseAsync(p => p.stockId == model.id, p => p.id, OrderByType.Desc);


            jm.code = 0;

            jm.data = new
            {
                model,
                logs,
                manager
            };

            return jm;
        }
        #endregion

    }
}