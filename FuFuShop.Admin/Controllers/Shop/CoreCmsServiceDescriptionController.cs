

using FuFuShop.Admin.Filter;
using FuFuShop.Common.AppSettings;
using FuFuShop.Common.Extensions;
using FuFuShop.Common.Helper;
using FuFuShop.Model.Entities;
using FuFuShop.Model.FromBody;
using FuFuShop.Model.ViewModels.UI;
using FuFuShop.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using System.ComponentModel;
using System.Linq.Expressions;

namespace FuFuShop.Admin.Controllers.Shop
{
    /// <summary>
    /// 商城服务说明
    ///</summary>
    [Description("商城服务说明")]
    [Route("api/[controller]/[action]")]
    [ApiController]
    [RequiredErrorForAdmin]
    [Authorize(Permissions.Name)]
    public class CoreCmsServiceDescriptionController : ControllerBase
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IServiceDescriptionServices _ServiceDescriptionServices;

        /// <summary>
        /// 构造函数
        ///</summary>
        public CoreCmsServiceDescriptionController(IWebHostEnvironment webHostEnvironment
            , IServiceDescriptionServices ServiceDescriptionServices
            )
        {
            _webHostEnvironment = webHostEnvironment;
            _ServiceDescriptionServices = ServiceDescriptionServices;
        }

        #region 获取列表============================================================
        // POST: Api/ServiceDescription/GetPageList
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
            var where = PredicateBuilder.True<ServiceDescription>();
            //获取排序字段
            var orderField = Request.Form["orderField"].FirstOrDefault();

            Expression<Func<ServiceDescription, object>> orderEx = orderField switch
            {
                "id" => p => p.id,
                "title" => p => p.title,
                "type" => p => p.type,
                "description" => p => p.description,
                "isShow" => p => p.isShow,
                "sortId" => p => p.sortId,
                _ => p => p.id
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

            //序列 int
            var id = Request.Form["id"].FirstOrDefault().ObjectToInt(0);
            if (id > 0)
            {
                where = where.And(p => p.id == id);
            }
            //名称 nvarchar
            var title = Request.Form["title"].FirstOrDefault();
            if (!string.IsNullOrEmpty(title))
            {
                where = where.And(p => p.title.Contains(title));
            }
            //类型 int
            var type = Request.Form["type"].FirstOrDefault().ObjectToInt(0);
            if (type > 0)
            {
                where = where.And(p => p.type == type);
            }
            //描述 nvarchar
            var description = Request.Form["description"].FirstOrDefault();
            if (!string.IsNullOrEmpty(description))
            {
                where = where.And(p => p.description.Contains(description));
            }
            //是否展示 bit
            var isShow = Request.Form["isShow"].FirstOrDefault();
            if (!string.IsNullOrEmpty(isShow) && isShow.ToLowerInvariant() == "true")
            {
                where = where.And(p => p.isShow == true);
            }
            else if (!string.IsNullOrEmpty(isShow) && isShow.ToLowerInvariant() == "false")
            {
                where = where.And(p => p.isShow == false);
            }
            //排序 int
            var sortId = Request.Form["sortId"].FirstOrDefault().ObjectToInt(0);
            if (sortId > 0)
            {
                where = where.And(p => p.sortId == sortId);
            }
            //获取数据
            var list = await _ServiceDescriptionServices.QueryPageAsync(where, orderEx, orderBy, pageCurrent, pageSize, true);
            //返回数据
            jm.data = list;
            jm.code = 0;
            jm.count = list.TotalCount;
            jm.msg = "数据调用成功!";
            return jm;
        }
        #endregion

        #region 首页数据============================================================
        // POST: Api/ServiceDescription/GetIndex
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

            var serviceNoteType = EnumHelper.EnumToList<GlobalEnumVars.ShopServiceNoteType>();

            jm.data = new
            {
                serviceNoteType
            };

            return jm;
        }
        #endregion

        #region 创建数据============================================================
        // POST: Api/ServiceDescription/GetCreate
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

            var serviceNoteType = EnumHelper.EnumToList<GlobalEnumVars.ShopServiceNoteType>();

            jm.data = new
            {
                serviceNoteType
            };

            return jm;
        }
        #endregion

        #region 创建提交============================================================
        // POST: Api/ServiceDescription/DoCreate
        /// <summary>
        /// 创建提交
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost]
        [Description("创建提交")]
        public async Task<AdminUiCallBack> DoCreate([FromBody] ServiceDescription entity)
        {
            var jm = await _ServiceDescriptionServices.InsertAsync(entity);
            return jm;
        }
        #endregion

        #region 编辑数据============================================================
        // POST: Api/ServiceDescription/GetEdit
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

            var model = await _ServiceDescriptionServices.QueryByIdAsync(entity.id, false);
            if (model == null)
            {
                jm.msg = "不存在此信息";
                return jm;
            }
            jm.code = 0;

            var serviceNoteType = EnumHelper.EnumToList<GlobalEnumVars.ShopServiceNoteType>();

            jm.data = new
            {
                model,
                serviceNoteType
            };

            return jm;
        }
        #endregion

        #region 编辑提交============================================================
        // POST: Api/ServiceDescription/Edit
        /// <summary>
        /// 编辑提交
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost]
        [Description("编辑提交")]
        public async Task<AdminUiCallBack> DoEdit([FromBody] ServiceDescription entity)
        {
            var jm = await _ServiceDescriptionServices.UpdateAsync(entity);
            return jm;
        }
        #endregion

        #region 删除数据============================================================
        // POST: Api/ServiceDescription/DoDelete/10
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

            var model = await _ServiceDescriptionServices.ExistsAsync(p => p.id == entity.id, true);
            if (!model)
            {
                jm.msg = GlobalConstVars.DataisNo;
                return jm;
            }
            jm = await _ServiceDescriptionServices.DeleteByIdAsync(entity.id);

            return jm;
        }
        #endregion

        #region 预览数据============================================================
        // POST: Api/ServiceDescription/GetDetails/10
        /// <summary>
        /// 预览数据
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost]
        [Description("预览数据")]
        public async Task<AdminUiCallBack> GetDetails([FromBody] FMIntId entity)
        {
            var jm = new AdminUiCallBack();

            var model = await _ServiceDescriptionServices.QueryByIdAsync(entity.id, false);
            if (model == null)
            {
                jm.msg = "不存在此信息";
                return jm;
            }
            jm.code = 0;

            var serviceNoteType = EnumHelper.EnumToList<GlobalEnumVars.ShopServiceNoteType>();

            jm.data = new
            {
                model,
                serviceNoteType
            };


            return jm;
        }
        #endregion

        #region 设置是否展示============================================================
        // POST: Api/ServiceDescription/DoSetisShow/10
        /// <summary>
        /// 设置是否展示
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost]
        [Description("设置是否展示")]
        public async Task<AdminUiCallBack> DoSetisShow([FromBody] FMUpdateBoolDataByIntId entity)
        {
            var jm = new AdminUiCallBack();

            var oldModel = await _ServiceDescriptionServices.QueryByIdAsync(entity.id, false);
            if (oldModel == null)
            {
                jm.msg = "不存在此信息";
                return jm;
            }
            oldModel.isShow = entity.data;

            jm = await _ServiceDescriptionServices.UpdateAsync(oldModel);

            return jm;
        }
        #endregion


    }
}
