/***********************************************************************
 *            Project: 
 *        ProjectName: 核心内容管理系统                                
 *                Web: https://www..net                      
 *             Author: 大灰灰                                          
 *              Email: jianweie@163.com                                
 *         CreateTime: 2021/1/31 21:45:10
 *        Description: 暂无
 ***********************************************************************/

using FuFuShop.Common.AppSettings;
using FuFuShop.Common.Auth.HttpContextUser;
using FuFuShop.Common.Extensions;
using FuFuShop.Common.Helper;
using FuFuShop.Model.Entities;
using FuFuShop.Model.Entities.Shop;
using FuFuShop.Model.FromBody;
using FuFuShop.Model.ViewModels.DTO;
using FuFuShop.Model.ViewModels.Echarts;
using FuFuShop.Model.ViewModels.UI;
using FuFuShop.Services;
using FuFuShop.Services.Bill;
using FuFuShop.Services.Good;
using FuFuShop.Services.Shop;
using FuFuShop.Services.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SqlSugar;
using System.ComponentModel;

namespace FuFuShop.Admin.Controllers
{
    /// <summary>
    ///     后端常用方法
    /// </summary>
    [Description("后端常用方法")]
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class ToolsController : ControllerBase
    {
        private readonly IAreaServices _areaServices;
        private readonly IGoodsServices _GoodsServices;
        //  private readonly INoticeServices _NoticeServices;
        private readonly ISettingServices _SettingServices;
        private readonly ILogisticsServices _logisticsServices;
        private readonly ISysMenuServices _sysMenuServices;
        private readonly ISysOrganizationServices _sysOrganizationServices;
        private readonly ISysRoleServices _sysRoleServices;
        private readonly ISysUserRoleServices _sysUserRoleServices;
        private readonly ISysRoleMenuServices _sysRoleMenuServices;
        private readonly ISysUserServices _sysUserServices;
        private readonly IHttpContextUser _user;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ISysLoginRecordServices _sysLoginRecordServices;
        private readonly ISysNLogRecordsServices _sysNLogRecordsServices;
        private readonly IBillPaymentsServices _paymentsServices;
        private readonly IBillDeliveryServices _billDeliveryServices;
        private readonly IBillAftersalesServices _aftersalesServices;
        private readonly IUserServices _userServices;
        private readonly IOrderServices _orderServices;
        private readonly ISettingServices _settingServices;
        private readonly IProductsServices _productsServices;
        private readonly IToolsServices _toolsServices;



        //   private readonly WeChat.Service.HttpClients.IWeChatApiHttpClientFactory _weChatApiHttpClientFactory;



        /// <summary>
        ///     构造函数
        /// </summary>
        public ToolsController(
            IHttpContextUser user
            , IWebHostEnvironment webHostEnvironment
            , IGoodsServices GoodsServices
            , ISettingServices SettingServices

            , IAreaServices areaServices
            , ISysUserServices sysUserServices
            , ISysRoleServices sysRoleServices
            , ISysMenuServices sysMenuServices
            , ISysUserRoleServices sysUserRoleServices
            , ISysOrganizationServices sysOrganizationServices,
            ILogisticsServices logisticsServices,
            ISysLoginRecordServices sysLoginRecordServices,
            ISysNLogRecordsServices sysNLogRecordsServices,
            IBillPaymentsServices paymentsServices,
            IBillDeliveryServices billDeliveryServices,
            IUserServices userServices,
            IOrderServices orderServices,
            IBillAftersalesServices aftersalesServices,
            ISettingServices settingServices,
            IProductsServices productsServices,
            ISysRoleMenuServices sysRoleMenuServices,
            IToolsServices toolsServices)
        {
            _user = user;
            _webHostEnvironment = webHostEnvironment;

            _GoodsServices = GoodsServices;
            _SettingServices = SettingServices;
            _areaServices = areaServices;
            _sysUserServices = sysUserServices;
            _sysRoleServices = sysRoleServices;
            _sysMenuServices = sysMenuServices;
            _sysUserRoleServices = sysUserRoleServices;
            _sysOrganizationServices = sysOrganizationServices;
            _logisticsServices = logisticsServices;
            _sysLoginRecordServices = sysLoginRecordServices;
            _sysNLogRecordsServices = sysNLogRecordsServices;
            _paymentsServices = paymentsServices;
            _billDeliveryServices = billDeliveryServices;
            _userServices = userServices;
            _orderServices = orderServices;
            _aftersalesServices = aftersalesServices;
            _settingServices = settingServices;
            _productsServices = productsServices;
            _sysRoleMenuServices = sysRoleMenuServices;
            _toolsServices = toolsServices;
        }

        #region 获取登录用户用户信息(用于面板展示)====================================================

        /// <summary>
        ///     获取登录用户用户信息(用于面板展示)
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<AdminUiCallBack> GetUserInfo()
        {
            var jm = new AdminUiCallBack();
            var userModel = await _sysUserServices.QueryByIdAsync(_user.ID);
            jm.code = userModel == null ? 401 : 0;
            jm.msg = userModel == null ? "注销登录" : "数据获取正常";
            jm.data = userModel == null ? null : new
            {
                userModel.userName,
                userModel.nickName,
                userModel.createTime
            };
            return jm;
        }

        #endregion

        #region 获取登录用户用户全部信息(用于编辑个人信息)====================================================

        /// <summary>
        ///     获取登录用户用户全部信息(用于编辑个人信息)
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<AdminUiCallBack> GetEditUserInfo()
        {
            var jm = new AdminUiCallBack();
            var userModel = await _sysUserServices.QueryByIdAsync(_user.ID);

            if (userModel != null)
            {
                var roles = await _sysUserRoleServices.QueryListByClauseAsync(p => p.userId == userModel.id);
                if (roles.Any())
                {
                    var roleIds = roles.Select(p => p.roleId).ToList();
                    userModel.roles = await _sysRoleServices.QueryListByClauseAsync(p => roleIds.Contains(p.id));
                }

                if (userModel.organizationId != null && userModel.organizationId > 0)
                {
                    var organization = await _sysOrganizationServices.QueryByIdAsync(userModel.organizationId);
                    if (organization != null) userModel.organizationName = organization.organizationName;
                }
            }

            jm.code = 0;
            jm.msg = "数据获取正常";
            jm.data = userModel;
            return jm;
        }

        #endregion

        #region 获取角色列表信息====================================================

        /// <summary>
        ///     获取角色列表信息
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<AdminUiCallBack> GetManagerRoles()
        {
            var jm = new AdminUiCallBack();
            var roles = await _sysRoleServices.QueryAsync();
            jm.code = 0;
            jm.msg = "数据获取正常";
            jm.data = roles.Select(p => new { title = p.roleName, value = p.id });
            return jm;
        }

        #endregion

        #region 用户编辑个人登录账户密码====================================================

        /// <summary>
        ///     获取登录用户用户信息
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<AdminUiCallBack> EditLoginUserPassWord([FromBody] FMEditLoginUserPassWord entity)
        {
            var jm = new AdminUiCallBack();

            if (string.IsNullOrEmpty(entity.oldPassword))
            {
                jm.msg = "请键入旧密码";
                return jm;
            }

            if (string.IsNullOrEmpty(entity.password))
            {
                jm.msg = "请键入新密码";
                return jm;
            }

            if (string.IsNullOrEmpty(entity.repassword))
            {
                jm.msg = "请键入新密码确认密码";
                return jm;
            }

            if (entity.password != entity.repassword)
            {
                jm.msg = "新密码与确认密码不相符";
                return jm;
            }

            if (entity.password == entity.oldPassword)
            {
                jm.msg = "请设置与旧密码不同的新密码";
                return jm;
            }

            var oldPassWord = CommonHelper.Md5For32(entity.oldPassword);
            var newPassWord = CommonHelper.Md5For32(entity.password);

            var userModel = await _sysUserServices.QueryByIdAsync(_user.ID);
            if (userModel.passWord.ToUpperInvariant() != oldPassWord)
            {
                jm.msg = "旧密码输入错误";
                return jm;
            }
            else if (userModel.passWord.ToUpperInvariant() == newPassWord)
            {
                jm.msg = "新旧密码一致，无需修改，请设置与旧密码不同的新密码";
                return jm;
            }

            userModel.passWord = newPassWord;

            var bl = await _sysUserServices.UpdateAsync(userModel);

            jm.code = bl ? 0 : 1;
            jm.msg = bl ? "修改成功" : "修改失败";
            return jm;
        }

        #endregion

        #region 用户编辑个人非安全隐私数据====================================================

        /// <summary>
        ///     用户编辑个人非安全隐私数据
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<AdminUiCallBack> EditLoginUserInfo([FromBody] EditLoginUserInfo entity)
        {
            var jm = new AdminUiCallBack();

            if (entity.trueName.Length > 4)
            {
                jm.msg = "用户真实姓名不能大于4个字符。";
                return jm;
            }



            var userModel = await _sysUserServices.QueryByIdAsync(_user.ID);

            if (!string.IsNullOrEmpty(entity.nickName)) userModel.nickName = entity.nickName;
            if (!string.IsNullOrEmpty(entity.avatar)) userModel.avatar = entity.avatar;
            if (entity.sex > 0) userModel.sex = entity.sex;
            if (!string.IsNullOrEmpty(entity.phone)) userModel.phone = entity.phone;
            if (!string.IsNullOrEmpty(entity.email)) userModel.email = entity.email;
            if (!string.IsNullOrEmpty(entity.introduction)) userModel.introduction = entity.introduction;

            if (!string.IsNullOrEmpty(entity.trueName)) userModel.trueName = entity.trueName;
            if (!string.IsNullOrEmpty(entity.idCard)) userModel.idCard = entity.idCard;
            if (entity.birthday != null) userModel.birthday = entity.birthday;

            userModel.updateTime = DateTime.Now;
            var bl = await _sysUserServices.UpdateAsync(userModel);

            jm.code = bl ? 0 : 1;
            jm.msg = bl ? "修改成功" : "修改失败";
            return jm;
        }

        #endregion

        //通用操作=========================================================================

        #region 通用上传接口====================================================

        /// <summary>
        ///     通用上传接口
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<AdminUiCallBack> UploadFiles()
        {
            var jm = new AdminUiCallBack();

            var filesStorageOptions = await _SettingServices.GetFilesStorageOptions();

            //初始化上传参数
            var maxSize = 1024 * 1024 * filesStorageOptions.MaxSize; //上传大小5M

            var file = Request.Form.Files["file"];
            if (file == null)
            {
                jm.msg = "请选择文件";
                return jm;
            }

            var fileName = file.FileName;
            var fileExt = Path.GetExtension(fileName).ToLowerInvariant();

            //检查大小
            if (file.Length > maxSize)
            {
                jm.msg = "上传文件大小超过限制，最大允许上传" + filesStorageOptions.MaxSize + "M";
                return jm;
            }

            //检查文件扩展名
            if (string.IsNullOrEmpty(fileExt) || Array.IndexOf(filesStorageOptions.FileTypes.Split(','), fileExt.Substring(1).ToLower()) == -1)
            {
                jm.msg = "上传文件扩展名是不允许的扩展名,请上传后缀名为：" + filesStorageOptions.FileTypes;
                return jm;
            }

            string url = await _toolsServices.UpLoadFileForQCloudOSS(filesStorageOptions, fileExt, file);

            var bl = !string.IsNullOrEmpty(url);
            jm.code = bl ? 0 : 1;
            jm.msg = bl ? "上传成功!" : "上传失败";
            jm.data = new
            {
                fileUrl = url,
                src = url
            };

            return jm;
        }

        #endregion

        #region 裁剪Base64上传====================================================

        /// <summary>
        ///     裁剪Base64上传
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<AdminUiCallBack> UploadFilesFByBase64([FromBody] FMBase64Post entity)
        {
            var jm = new AdminUiCallBack();

            var filesStorageOptions = await _SettingServices.GetFilesStorageOptions();

            if (string.IsNullOrEmpty(entity.base64))
            {
                jm.msg = "请上传合法内容";
                return jm;
            }

            entity.base64 = entity.base64.Replace("data:image/png;base64,", "").Replace("data:image/jgp;base64,", "").Replace("data:image/jpg;base64,", "").Replace("data:image/jpeg;base64,", "");//将base64头部信息替换
            byte[] bytes = Convert.FromBase64String(entity.base64);
            MemoryStream memStream = new MemoryStream(bytes);

            string url = _toolsServices.UpLoadBase64ForQCloudOSS(filesStorageOptions, bytes);

            var bl = !string.IsNullOrEmpty(url);
            jm.code = bl ? 0 : 1;
            jm.msg = bl ? "上传成功!" : "上传失败";
            jm.data = new
            {
                fileUrl = url,
                src = url
            };

            return jm;
        }

        #endregion



        #region 根据id获取商品信息====================================================

        // POST: Api/Tools/GetGoodsByIds
        /// <summary>
        ///     根据id获取商品信息
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Description("根据id获取商品信息")]
        public async Task<AdminUiCallBack> GetGoodsByIds([FromBody] FMArrayIntIds entity)
        {
            var jm = new AdminUiCallBack();

            var list = await _GoodsServices.QueryByIDsAsync(entity.id);
            jm.code = 0;
            jm.data = list;

            return jm;
        }

        #endregion



        //通用页面获取=========================================================================

        #region 获取商品列表====================================================

        // POST: Api/Tools/GetGoods
        /// <summary>
        ///     获取商品列表
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Description("获取商品列表")]
        public async Task<AdminUiCallBack> GetGoods()
        {
            var jm = new AdminUiCallBack();
            var pageCurrent = Request.Form["page"].FirstOrDefault().ObjectToInt(1);
            var pageSize = Request.Form["limit"].FirstOrDefault().ObjectToInt(30);
            var where = PredicateBuilder.True<Goods>();
            where = where.And(p => p.isMarketable);
            //商品编码 nvarchar
            var bn = Request.Form["bn"].FirstOrDefault();
            if (!string.IsNullOrEmpty(bn)) where = where.And(p => p.bn.Contains(bn));
            //商品名称 nvarchar
            var name = Request.Form["name"].FirstOrDefault();
            if (!string.IsNullOrEmpty(name)) where = where.And(p => p.name.Contains(name));
            where = where.And(p => p.isDel == false);
            //获取数据
            var list = await _GoodsServices.QueryPageAsync(where, p => p.createTime, OrderByType.Desc,
                pageCurrent, pageSize);
            //返回数据

            var newObj = list.Select(p => new
            {
                p.id,
                image = !string.IsNullOrEmpty(p.images) ? p.images.Split(",")[0] : "/static/images/common/empty.png",
                p.images,
                p.price,
                p.name,
                p.stock
            }).ToList();


            jm.data = newObj;
            jm.code = 0;
            jm.count = list.TotalCount;
            jm.msg = "数据调用成功!";
            return jm;
        }

        #endregion

        #region 根据商品序列获取货品数据====================================================

        // POST: Api/Tools/GetProducts
        /// <summary>
        ///     根据商品序列获取货品数据
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Description("获取商品列表")]
        public async Task<AdminUiCallBack> GetProducts(FMIntId entity)
        {
            var jm = new AdminUiCallBack();

            var list = await _productsServices.GetProducts(entity.id);

            jm.code = 0;
            jm.data = list;
            jm.msg = "数据调用成功!";
            return jm;
        }

        #endregion




        #region 获取区域信息=======================================================

        /// <summary>
        ///     获取区域信息
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<WebApiCallBack> GetArea()
        {
            var jm = new WebApiCallBack();

            var ischecked = Request.Form["ischecked"].FirstOrDefault().ObjectToInt(0);
            var nodeId = Request.Form["nodeId"].FirstOrDefault().ObjectToInt(0);
            var idsStr = Request.Form["ids"].FirstOrDefault();

            var ids = new List<PostAreasTreeNode>();
            if (!string.IsNullOrEmpty(idsStr)) ids = JsonConvert.DeserializeObject<List<PostAreasTreeNode>>(idsStr);
            var areaTrees = await _areaServices.GetTreeArea(ids, nodeId, ischecked);

            jm.status = true;
            jm.data = areaTrees;
            jm.msg = ids.Count.ToString();

            return jm;
        }

        #endregion

        #region 物流查询接口=======================================================

        /// <summary>
        ///     物流查询接口
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<WebApiCallBack> LogisticsByApi([FromBody] FMApiLogisticsByApiPost entity)
        {
            var jm = new WebApiCallBack();

            if (string.IsNullOrEmpty(entity.code) || string.IsNullOrEmpty(entity.no))
            {
                jm.code = 1;
                jm.msg = GlobalErrorCodeVars.Code13225;
                return jm;
            }

            var systemLogistics = SystemSettingDictionary.GetSystemLogistics();
            foreach (var p in systemLogistics)
            {
                if (entity.code == p.sKey)
                {
                    jm.msg = p.sDescription + "不支持轨迹查询";
                    return jm;
                }
            }

            jm = await _logisticsServices.ExpressPoll(entity.code, entity.no, entity.mobile);
            return jm;

        }

        #endregion

        //用户相关=========================================================================

        #region 根据用户权限获取对应左侧菜单列表====================================================

        /// <summary>
        ///     根据用户权限获取对应左侧菜单列表
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<AdminUiCallBack> GetNavs()
        {
            var jm = new AdminUiCallBack();

            //先获取用户关联角色
            var roles = await _sysUserRoleServices.QueryListByClauseAsync(p => p.userId == _user.ID);
            if (roles.Any())
            {
                var roleIds = roles.Select(p => p.roleId).ToList();
                var sysRoleMenu = await _sysRoleMenuServices.QueryListByClauseAsync(p => roleIds.Contains(p.roleId));


                var menuIds = sysRoleMenu.Select(p => p.menuId).ToList();


                var where = PredicateBuilder.True<SysMenu>();
                where = where.And(p => p.deleted == false && p.hide == false && p.menuType == 0);
                where = where.And(p => menuIds.Contains(p.id));

                var navs = await _sysMenuServices.QueryListByClauseAsync(where, p => p.sortNumber, OrderByType.Asc);
                var menus = GetMenus(navs, 0);

                jm.data = menus;
            }

            jm.msg = "数据获取正常";
            jm.code = 0;

            return jm;
        }

        /// <summary>
        ///     迭代方法
        /// </summary>
        /// <param name="oldNavs"></param>
        /// <param name="parentId"></param>
        /// <returns></returns>
        private static List<AdminUiMenu> GetMenus(List<SysMenu> oldNavs, int parentId)
        {
            var childTree = new List<AdminUiMenu>();
            if (parentId == 0)
            {
                var topMenu = new AdminUiMenu { title = "主页", icon = "layui-icon-home", name = "HomePanel" };
                var list = new List<AdminUiMenu>
                {
                    new AdminUiMenu
                        {title = "控制台", jump = "/", name = "controllerPanel", list = new List<AdminUiMenu>()}
                };
                topMenu.list = list;
                childTree.Add(topMenu);
            }

            var model = oldNavs.Where(p => p.parentId == parentId).ToList();
            foreach (var item in model)
            {
                var menu = new AdminUiMenu
                {
                    name = item.identificationCode,
                    title = item.menuName,
                    icon = item.menuIcon,
                    jump = !string.IsNullOrEmpty(item.path) ? item.path : null
                };
                childTree.Add(menu);
                menu.list = GetMenus(oldNavs, item.id);
            }

            return childTree;
        }

        #endregion

        #region 后台Select三级下拉联动配合

        /// <summary>
        ///     获取大类列表
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<List<AreasDtoForAdminEdit>> GetAreaCheckedList([FromBody] FMIntId entity)
        {
            var res = new List<AreasDtoForAdminEdit>();

            if (entity.id != 0)
            {
                var model3 = new AreasDtoForAdminEdit();
                model3.info = await _areaServices.QueryByIdAsync(entity.id);
                if (model3.info != null && model3.info.parentId != 0)
                {
                    model3.list = await _areaServices.QueryListByClauseAsync(p => p.parentId == model3.info.parentId);

                    var model2 = new AreasDtoForAdminEdit();
                    model2.info = await _areaServices.QueryByIdAsync(model3.info.parentId);
                    if (model2.info != null && model2.info.parentId != 0)
                    {
                        model2.list =
                            await _areaServices.QueryListByClauseAsync(p => p.parentId == model2.info.parentId);

                        var model = new AreasDtoForAdminEdit();
                        model.info = await _areaServices.QueryByIdAsync(model2.info.parentId);
                        if (model.info != null)
                            model.list =
                                await _areaServices.QueryListByClauseAsync(p => p.parentId == model.info.parentId);
                        res.Add(model);
                    }

                    res.Add(model2);
                }

                res.Add(model3);
            }
            else
            {
                var model4 = new AreasDtoForAdminEdit();
                model4.list = await _areaServices.QueryListByClauseAsync(p => p.parentId == 0);
                model4.info = model4.list.FirstOrDefault();
                res.Add(model4);
            }

            return res;
        }

        /// <summary>
        ///     取地区的下级列表
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<List<Area>> GetAreaChildren([FromBody] FMIntId entity)
        {
            var list = await _areaServices.QueryListByClauseAsync(p => p.parentId == entity.id);
            return list;
        }

        #endregion

        //后端首页使用数据

        #region 获取最近登录日志============================================================

        // POST: Api/Tools/GetSysLoginRecord
        /// <summary>
        ///     获取最近登录日志
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Description("获取最近登录日志")]
        public async Task<AdminUiCallBack> GetSysLoginRecord()
        {
            var jm = new AdminUiCallBack();

            //获取数据
            var list = await _sysLoginRecordServices.QueryPageAsync(p => p.id > 0, p => p.createTime, OrderByType.Desc, 1, 10);
            //返回数据
            jm.data = list;
            jm.code = 0;
            jm.count = list.TotalCount;
            jm.msg = "数据调用成功!";
            return jm;
        }

        #endregion

        #region 获取全局Nlog日志============================================================

        // POST: Api/Tools/GetSysNLogRecords
        /// <summary>
        ///     获取全局Nlog日志
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Description("获取全局Nlog日志")]
        public async Task<AdminUiCallBack> GetSysNLogRecords()
        {
            var jm = new AdminUiCallBack();
            //获取数据
            var list = await _sysNLogRecordsServices.QueryPageAsync(p => p.id > 0, p => p.id, OrderByType.Desc, 1, 10);
            //返回数据
            jm.data = list;
            jm.code = 0;
            jm.count = list.TotalCount;
            jm.msg = "数据调用成功!";
            return jm;
        }

        #endregion

        #region 获取7天订单情况数据统计============================================================

        // POST: Api/Tools/GetOrdersStatistics
        /// <summary>
        ///     获取7天订单情况数据统计
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Description("获取7天订单情况数据统计")]
        public async Task<AdminUiCallBack> GetOrdersStatistics()
        {
            var jm = new AdminUiCallBack();


            var data = new List<string>() { "已支付", "已发货" };
            var legend = new Legend();
            legend.data = data;

            var paymentsStatistics = await _paymentsServices.Statistics();
            var billDeliveryStatistics = await _billDeliveryServices.Statistics();


            var xAxis = new List<XAxis>();
            var xItem = new XAxis();
            xItem.type = "category";
            xItem.data = paymentsStatistics.Select(p => p.day).ToList();

            for (int i = 0; i < xItem.data.Count; i++)
            {
                xItem.data[i] = Convert.ToDateTime(xItem.data[i]).ToString("d日");
            }
            xAxis.Add(xItem);

            var series = new List<SeriesDataIntItem>
            {
                new SeriesDataIntItem()
                {
                    name = "已支付", type = "line", data = paymentsStatistics.Select(p => p.nums).ToList()
                },
                new SeriesDataIntItem()
                {
                    name = "已发货", type = "line", data = billDeliveryStatistics.Select(p => p.nums).ToList()
                }
            };

            jm.code = 0;
            jm.data = new
            {
                legend,
                xAxis,
                series
            };

            //返回数据
            jm.msg = "数据调用成功!";
            return jm;
        }

        #endregion

        #region 获取用户最新统计数据============================================================

        // POST: Api/Tools/GetUsersStatistics
        /// <summary>
        ///     获取用户最新统计数据
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Description("获取用户最新统计数据")]
        public async Task<AdminUiCallBack> GetUsersStatistics()
        {
            var jm = new AdminUiCallBack();


            var data = new List<string>() { "新增记录", "活跃记录" };
            var legend = new Legend();
            legend.data = data;

            var regs = await _userServices.Statistics(7);
            var orders = await _userServices.StatisticsOrder(7);


            var xAxis = new List<XAxis>();
            var xItem = new XAxis();
            xItem.type = "category";
            xItem.data = orders.Select(p => p.day).ToList();

            for (int i = 0; i < xItem.data.Count; i++)
            {
                xItem.data[i] = Convert.ToDateTime(xItem.data[i]).ToString("d日");
            }
            xAxis.Add(xItem);



            var series = new List<SeriesDataIntItem>
            {
                new SeriesDataIntItem() {name = "新增记录", type = "line", data = regs.Select(p => p.nums).ToList()},
                new SeriesDataIntItem() {name = "活跃记录", type = "line", data = orders.Select(p => p.nums).ToList()}
            };



            jm.code = 0;
            jm.data = new
            {
                legend,
                xAxis,
                series
            };

            //返回数据
            jm.msg = "数据调用成功!";
            return jm;
        }

        #endregion

        #region 获取代办事宜数据============================================================

        // POST: Api/Tools/GetBackLog
        /// <summary>
        ///     获取代办事宜数据
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Description("获取用户最新统计数据")]
        public async Task<AdminUiCallBack> GetBackLog()
        {
            var jm = new AdminUiCallBack();
            //待支付
            var paymentWhere = _orderServices.GetReverseStatus((int)GlobalEnumVars.OrderAllStatusType.ALL_PENDING_PAYMENT);
            var unpaidCount = await _orderServices.GetCountAsync(paymentWhere);


            //待发货
            var deliveredWhere = _orderServices.GetReverseStatus((int)GlobalEnumVars.OrderAllStatusType.ALL_PENDING_DELIVERY);
            var unshipCount = await _orderServices.GetCountAsync(deliveredWhere);

            //待售后
            var aftersalesCount = await _aftersalesServices.GetCountAsync(p => p.status == (int)GlobalEnumVars.BillAftersalesStatus.WaitAudit);

            var allConfigs = await _settingServices.GetConfigDictionaries();
            var goodsStocksWarn = CommonHelper.GetConfigDictionary(allConfigs, SystemSettingConstVars.GoodsStocksWarn).ObjectToInt(10);

            //库存报警
            var goodsStaticsTotalWarn = await _productsServices.GoodsStaticsTotalWarn(goodsStocksWarn);

            //返回数据
            jm.code = 0;
            jm.msg = "数据调用成功!";
            jm.data = new
            {
                unpaidCount,
                unshipCount,
                aftersalesCount,
                goodsStaticsTotalWarn
            };

            return jm;
        }

        #endregion

    }
}