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
using FuFuShop.Model.ViewModels.UI;
using FuFuShop.Services.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using System.ComponentModel;
using System.Linq.Expressions;

namespace FuFuShop.Admin.Controllers
{
    /// <summary>
    ///     用户表
    /// </summary>
    [Description("用户表")]
    [Route("api/[controller]/[action]")]
    [ApiController]
    [RequiredErrorForAdmin]
    [Authorize(Permissions.Name)]
    public class UserController : ControllerBase
    {

        private readonly IUserServices _UserServices;
        private readonly IWebHostEnvironment _webHostEnvironment;


        /// <summary>
        ///     构造函数
        /// </summary>
        /// <param name="webHostEnvironment"></param>
        /// <param name="UserServices"></param>
        /// <param name="UserGradeServices"></param>
        /// <param name="UserBalanceServices"></param>
        /// <param name="UserPointLogServices"></param>
        public UserController(
            IWebHostEnvironment webHostEnvironment
            , IUserServices UserServices

        )
        {
            _webHostEnvironment = webHostEnvironment;
            _UserServices = UserServices;

        }

        #region 获取列表============================================================

        // POST: Api/FuFuShopUser/GetPageList
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
            var where = PredicateBuilder.True<FuFuShopUser>();
            //获取排序字段
            var orderField = Request.Form["orderField"].FirstOrDefault();
            Expression<Func<FuFuShopUser, object>> orderEx;
            switch (orderField)
            {
                case "id":
                    orderEx = p => p.id;
                    break;
                case "userName":
                    orderEx = p => p.userName;
                    break;
                case "passWord":
                    orderEx = p => p.passWord;
                    break;
                case "mobile":
                    orderEx = p => p.mobile;
                    break;
                case "sex":
                    orderEx = p => p.sex;
                    break;
                case "birthday":
                    orderEx = p => p.birthday;
                    break;
                case "avatarImage":
                    orderEx = p => p.avatarImage;
                    break;
                case "nickName":
                    orderEx = p => p.nickName;
                    break;
                case "balance":
                    orderEx = p => p.balance;
                    break;
                case "point":
                    orderEx = p => p.point;
                    break;
                case "grade":
                    orderEx = p => p.grade;
                    break;
                case "createTime":
                    orderEx = p => p.createTime;
                    break;
                case "updataTime":
                    orderEx = p => p.updataTime;
                    break;
                case "status":
                    orderEx = p => p.status;
                    break;
                case "parentId":
                    orderEx = p => p.parentId;
                    break;
                case "isDelete":
                    orderEx = p => p.isDelete;
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

            //用户名 nvarchar
            var userName = Request.Form["userName"].FirstOrDefault();
            if (!string.IsNullOrEmpty(userName))
            {
                where = where.And(p => p.userName.Contains(userName));
            }
            //手机号 nvarchar
            var mobile = Request.Form["mobile"].FirstOrDefault();
            if (!string.IsNullOrEmpty(mobile))
            {
                where = where.And(p => p.mobile.Contains(mobile));
            }
            //性别[1男2女3未知] int
            var sex = Request.Form["sex"].FirstOrDefault().ObjectToInt(0);
            if (sex > 0)
            {
                where = where.And(p => p.sex == sex);
            }
            //昵称 nvarchar
            var nickName = Request.Form["nickName"].FirstOrDefault();
            if (!string.IsNullOrEmpty(nickName))
            {
                where = where.And(p => p.nickName.Contains(nickName));
            }
            //用户等级 int
            var grade = Request.Form["grade"].FirstOrDefault().ObjectToInt(0);
            if (grade > 0)
            {
                where = where.And(p => p.grade == grade);
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
            //更新时间 datetime
            var updataTime = Request.Form["updataTime"].FirstOrDefault();
            if (!string.IsNullOrEmpty(updataTime))
            {
                var dt = updataTime.ObjectToDate();
                where = where.And(p => p.updataTime > dt);
            }
            //状态[1正常2停用] int
            var status = Request.Form["status"].FirstOrDefault().ObjectToInt(0);
            if (status > 0)
            {
                where = where.And(p => p.status == status);
            }
            //推荐人 int
            var parentId = Request.Form["parentId"].FirstOrDefault().ObjectToInt(0);
            if (parentId > 0)
            {
                where = where.And(p => p.parentId == parentId);
            }
            //删除标志 有数据就是删除 bit
            var isDelete = Request.Form["isDelete"].FirstOrDefault();
            if (!string.IsNullOrEmpty(isDelete) && isDelete.ToLowerInvariant() == "true")
            {
                where = where.And(p => p.isDelete);
            }
            else if (!string.IsNullOrEmpty(isDelete) && isDelete.ToLowerInvariant() == "false")
            {
                where = where.And(p => p.isDelete == false);
            }
            //获取数据
            var list = await _UserServices.QueryPageAsync(where, orderEx, orderBy, pageCurrent, pageSize);
            //返回数据
            jm.data = list;
            jm.code = 0;
            jm.count = list.TotalCount;
            jm.msg = "数据调用成功!";
            return jm;
        }

        #endregion



    }
}