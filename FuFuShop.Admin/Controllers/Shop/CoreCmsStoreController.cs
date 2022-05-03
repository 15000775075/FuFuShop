

using FuFuShop.Admin.Filter;
using FuFuShop.Common.AppSettings;
using FuFuShop.Common.Extensions;
using FuFuShop.Model.Entities;
using FuFuShop.Model.Entities.Shop;
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
    ///     门店表
    /// </summary>
    [Description("门店表")]
    [Route("api/[controller]/[action]")]
    [ApiController]
    [RequiredErrorForAdmin]
    [Authorize(Permissions.Name)]
    public class StoreController : ControllerBase
    {
        private readonly IClerkServices _ClerkServices;
        private readonly IStoreServices _StoreServices;
        private readonly IUserServices _UserServices;
        private readonly IWebHostEnvironment _webHostEnvironment;

        /// <summary>
        ///     构造函数
        /// </summary>
        public StoreController(IWebHostEnvironment webHostEnvironment,
            IStoreServices StoreServices, IClerkServices ClerkServices,
            IUserServices UserServices)
        {
            _webHostEnvironment = webHostEnvironment;
            _StoreServices = StoreServices;
            _ClerkServices = ClerkServices;
            _UserServices = UserServices;
        }

        #region 获取列表============================================================

        // POST: Api/Store/GetPageList
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
            var where = PredicateBuilder.True<Store>();
            //获取排序字段
            var orderField = Request.Form["orderField"].FirstOrDefault();
            Expression<Func<Store, object>> orderEx;
            switch (orderField)
            {
                case "id":
                    orderEx = p => p.id;
                    break;
                case "storeName":
                    orderEx = p => p.storeName;
                    break;
                case "mobile":
                    orderEx = p => p.mobile;
                    break;
                case "linkMan":
                    orderEx = p => p.linkMan;
                    break;
                case "logoImage":
                    orderEx = p => p.logoImage;
                    break;
                case "areaId":
                    orderEx = p => p.areaId;
                    break;
                case "address":
                    orderEx = p => p.address;
                    break;
                case "coordinate":
                    orderEx = p => p.coordinate;
                    break;
                case "latitude":
                    orderEx = p => p.latitude;
                    break;
                case "longitude":
                    orderEx = p => p.longitude;
                    break;
                case "createTime":
                    orderEx = p => p.createTime;
                    break;
                case "updateTime":
                    orderEx = p => p.updateTime;
                    break;
                default:
                    orderEx = p => p.isDefault;
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
            //门店名称 nvarchar
            var storeName = Request.Form["storeName"].FirstOrDefault();
            if (!string.IsNullOrEmpty(storeName)) @where = @where.And(p => p.storeName.Contains(storeName));
            //门店电话/手机号 nvarchar
            var mobile = Request.Form["mobile"].FirstOrDefault();
            if (!string.IsNullOrEmpty(mobile)) @where = @where.And(p => p.mobile.Contains(mobile));
            //门店联系人 nvarchar
            var linkMan = Request.Form["linkMan"].FirstOrDefault();
            if (!string.IsNullOrEmpty(linkMan)) @where = @where.And(p => p.linkMan.Contains(linkMan));
            //门店logo nvarchar
            var logoImage = Request.Form["logoImage"].FirstOrDefault();
            if (!string.IsNullOrEmpty(logoImage)) @where = @where.And(p => p.logoImage.Contains(logoImage));
            //门店地区id int
            var areaId = Request.Form["areaId"].FirstOrDefault().ObjectToInt(0);
            if (areaId > 0) @where = @where.And(p => p.areaId == areaId);
            //门店详细地址 nvarchar
            var address = Request.Form["address"].FirstOrDefault();
            if (!string.IsNullOrEmpty(address)) @where = @where.And(p => p.address.Contains(address));
            //坐标位置 nvarchar
            var coordinate = Request.Form["coordinate"].FirstOrDefault();
            if (!string.IsNullOrEmpty(coordinate)) @where = @where.And(p => p.coordinate.Contains(coordinate));
            //纬度 nvarchar
            var latitude = Request.Form["latitude"].FirstOrDefault();
            if (!string.IsNullOrEmpty(latitude)) @where = @where.And(p => p.latitude.Contains(latitude));
            //经度 nvarchar
            var longitude = Request.Form["longitude"].FirstOrDefault();
            if (!string.IsNullOrEmpty(longitude)) @where = @where.And(p => p.longitude.Contains(longitude));
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

            //更新时间 datetime
            var updateTime = Request.Form["updateTime"].FirstOrDefault();
            if (!string.IsNullOrEmpty(updateTime))
            {
                if (updateTime.Contains("到"))
                {
                    var dts = updateTime.Split("到");
                    var dtStart = dts[0].Trim().ObjectToDate();
                    where = where.And(p => p.updateTime > dtStart);
                    var dtEnd = dts[1].Trim().ObjectToDate();
                    where = where.And(p => p.updateTime < dtEnd);
                }
                else
                {
                    var dt = updateTime.ObjectToDate();
                    where = where.And(p => p.updateTime > dt);
                }
            }

            //获取数据
            var list = await _StoreServices.QueryPageAsync(where, orderEx, orderBy, pageCurrent, pageSize);
            //返回数据
            jm.data = list;
            jm.code = 0;
            jm.count = list.TotalCount;
            jm.msg = "数据调用成功!";
            return jm;
        }

        #endregion

        #region 首页数据============================================================

        // POST: Api/Store/GetIndex
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

        // POST: Api/Store/GetCreate
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

        // POST: Api/Store/DoCreate
        /// <summary>
        ///     创建提交
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost]
        [Description("创建提交")]
        public async Task<AdminUiCallBack> DoCreate([FromBody] Store entity)
        {
            var jm = new AdminUiCallBack();

            entity.createTime = DateTime.Now;
            jm = await _StoreServices.InsertAsync(entity);

            return jm;
        }

        #endregion

        #region 编辑数据============================================================

        // POST: Api/Store/GetEdit
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

            var model = await _StoreServices.QueryByIdAsync(entity.id);
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

        // POST: Admins/Store/Edit
        /// <summary>
        ///     编辑提交
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost]
        [Description("编辑提交")]
        public async Task<AdminUiCallBack> DoEdit([FromBody] Store entity)
        {
            var jm = new AdminUiCallBack();

            entity.updateTime = DateTime.Now;
            jm = await _StoreServices.UpdateAsync(entity);

            return jm;
        }

        #endregion

        #region 删除数据============================================================

        // POST: Api/Store/DoDelete/10
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

            var model = await _StoreServices.QueryByIdAsync(entity.id);
            if (model == null)
            {
                jm.msg = GlobalConstVars.DataisNo;
                return jm;
            }

            var bl = await _StoreServices.DeleteByIdAsync(entity.id);
            if (bl)
            {
                await _ClerkServices.DeleteAsync(p => p.storeId == model.id);
            }

            jm.code = bl ? 0 : 1;
            jm.msg = bl ? GlobalConstVars.DeleteSuccess : GlobalConstVars.DeleteFailure;
            return jm;

        }

        #endregion

        #region 设置是否默认============================================================

        // POST: Api/Store/DoSetisDefault/10
        /// <summary>
        ///     设置是否默认
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost]
        [Description("设置是否默认")]
        public async Task<AdminUiCallBack> DoSetisDefault([FromBody] FMUpdateBoolDataByIntId entity)
        {
            var jm = new AdminUiCallBack();

            var oldModel = await _StoreServices.QueryByIdAsync(entity.id);
            if (oldModel == null)
            {
                jm.msg = "不存在此信息";
                return jm;
            }

            oldModel.isDefault = entity.data;
            oldModel.updateTime = DateTime.Now;

            jm = await _StoreServices.UpdateAsync(oldModel);

            return jm;
        }

        #endregion

        //店员设置

        #region 获取列表============================================================

        // POST: Api/Store/GetClerkPageList
        /// <summary>
        ///     获取列表
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Description("获取列表")]
        public async Task<AdminUiCallBack> GetClerkPageList()
        {
            var jm = new AdminUiCallBack();
            var pageCurrent = Request.Form["page"].FirstOrDefault().ObjectToInt(1);
            var pageSize = Request.Form["limit"].FirstOrDefault().ObjectToInt(30);
            var where = PredicateBuilder.True<StoreClerkDto>();
            //获取排序字段
            var orderField = Request.Form["orderField"].FirstOrDefault();
            Expression<Func<StoreClerkDto, object>> orderEx;
            switch (orderField)
            {
                case "id":
                    orderEx = p => p.id;
                    break;
                case "storeId":
                    orderEx = p => p.storeId;
                    break;
                case "userId":
                    orderEx = p => p.userId;
                    break;
                case "isDel":
                    orderEx = p => p.isDel;
                    break;
                case "createTime":
                    orderEx = p => p.createTime;
                    break;
                case "updateTime":
                    orderEx = p => p.updateTime;
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
            //店铺ID int
            var storeId = Request.Form["storeId"].FirstOrDefault().ObjectToInt(0);
            if (storeId > 0) @where = @where.And(p => p.storeId == storeId);
            //用户ID int
            var userId = Request.Form["userId"].FirstOrDefault().ObjectToInt(0);
            if (userId > 0) @where = @where.And(p => p.userId == userId);
            //是否删除 bit
            var isDel = Request.Form["isDel"].FirstOrDefault();
            if (!string.IsNullOrEmpty(isDel) && isDel.ToLowerInvariant() == "true")
                @where = @where.And(p => p.isDel);
            else if (!string.IsNullOrEmpty(isDel) && isDel.ToLowerInvariant() == "false")
                @where = @where.And(p => p.isDel == false);
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

            //删除时间 datetime
            var updateTime = Request.Form["updateTime"].FirstOrDefault();
            if (!string.IsNullOrEmpty(updateTime))
            {
                if (updateTime.Contains("到"))
                {
                    var dts = updateTime.Split("到");
                    var dtStart = dts[0].Trim().ObjectToDate();
                    where = where.And(p => p.updateTime > dtStart);
                    var dtEnd = dts[1].Trim().ObjectToDate();
                    where = where.And(p => p.updateTime < dtEnd);
                }
                else
                {
                    var dt = updateTime.ObjectToDate();
                    where = where.And(p => p.updateTime > dt);
                }
            }

            //获取数据
            var list = await _ClerkServices.QueryStoreClerkDtoPageAsync(where, orderEx, orderBy, pageCurrent, pageSize, true);
            //返回数据
            jm.data = list;
            jm.code = 0;
            jm.count = list.TotalCount;
            jm.msg = "数据调用成功!";
            return jm;
        }

        #endregion

        #region 店员首页数据============================================================

        // POST: Api/Store/GetClerkIndex
        /// <summary>
        ///     店员首页数据
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Description("店员首页数据")]
        public AdminUiCallBack GetClerkIndex()
        {
            //返回数据
            var jm = new AdminUiCallBack { code = 0 };
            return jm;
        }

        #endregion

        #region 创建店员数据============================================================

        // POST: Api/Store/GetClerkCreate
        /// <summary>
        ///     创建店员数据
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Description("创建店员数据")]
        public AdminUiCallBack GetClerkCreate()
        {
            //返回数据
            var jm = new AdminUiCallBack { code = 0 };
            return jm;
        }

        #endregion



    }
}