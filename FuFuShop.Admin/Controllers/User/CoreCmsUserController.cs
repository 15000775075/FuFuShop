

using FuFuShop.Admin.Filter;
using FuFuShop.Common.AppSettings;
using FuFuShop.Common.Extensions;
using FuFuShop.Common.Helper;
using FuFuShop.Model.Entities;
using FuFuShop.Model.FromBody;
using FuFuShop.Model.ViewModels.UI;
using FuFuShop.Services.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NPOI.HSSF.UserModel;
using SqlSugar;
using System.ComponentModel;
using System.Linq.Expressions;

namespace FuFuShop.Admin.Controllers.User
{
    /// <summary>
    ///     用户表
    /// </summary>
    [Description("用户表")]
    [Route("api/[controller]/[action]")]
    [ApiController]
    [RequiredErrorForAdmin]
    [Authorize(Permissions.Name)]
    public class CoreCmsUserController : ControllerBase
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
        public CoreCmsUserController(
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


        #region 首页数据============================================================

        // POST: Api/FuFuShopUser/GetIndex
        /// <summary>
        ///     首页数据
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Description("首页数据")]
        public async Task<AdminUiCallBack> GetIndex()
        {
            //返回数据
            var jm = new AdminUiCallBack { code = 0 };

            var sexTypes = EnumHelper.EnumToList<GlobalEnumVars.UserSexTypes>();
            var userStatus = EnumHelper.EnumToList<GlobalEnumVars.UserStatus>();
            var userAccountTypes = EnumHelper.EnumToList<GlobalEnumVars.UserAccountTypes>();
            jm.data = new
            {
                sexTypes,
                userStatus,
                userAccountTypes
            };
            return jm;
        }

        #endregion

        #region 创建提交============================================================

        // POST: Api/FuFuShopUser/DoCreate
        /// <summary>
        ///     创建提交
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost]
        [Description("创建提交")]
        public async Task<AdminUiCallBack> DoCreate([FromBody] FuFuShopUser entity)
        {
            var jm = new AdminUiCallBack();

            if (string.IsNullOrEmpty(entity.mobile))
            {
                jm.msg = "请输入用户手机号";
                return jm;
            }

            var isHava = await _UserServices.ExistsAsync(p => p.mobile == entity.mobile);
            if (isHava)
            {
                jm.msg = "已存在此手机号码";
                return jm;
            }

            entity.createTime = DateTime.Now;
            entity.passWord = CommonHelper.Md5For32(entity.passWord);
            entity.parentId = 0;

            var bl = await _UserServices.InsertAsync(entity) > 0;
            jm.code = bl ? 0 : 1;
            jm.msg = bl ? GlobalConstVars.CreateSuccess : GlobalConstVars.CreateFailure;


            return jm;
        }

        #endregion

        #region 编辑数据============================================================

        // POST: Api/FuFuShopUser/GetEdit
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

            var model = await _UserServices.QueryByIdAsync(entity.id);
            if (model == null)
            {
                jm.msg = "不存在此信息";
                return jm;
            }

            jm.code = 0;
            var sexTypes = EnumHelper.EnumToList<GlobalEnumVars.UserSexTypes>();
            var userStatus = EnumHelper.EnumToList<GlobalEnumVars.UserStatus>();
            var userGrade = await _UserServices.QueryAsync();

            jm.data = new
            {
                model,
                userGrade,
                sexTypes,
                userStatus
            };

            return jm;
        }

        #endregion

        #region 编辑提交============================================================

        // POST: Admins/FuFuShopUser/Edit
        /// <summary>
        ///     编辑提交
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost]
        [Description("编辑提交")]
        public async Task<AdminUiCallBack> DoEdit([FromBody] FuFuShopUser entity)
        {
            var jm = new AdminUiCallBack();

            var oldModel = await _UserServices.QueryByIdAsync(entity.id);
            if (oldModel == null)
            {
                jm.msg = "不存在此信息";
                return jm;
            }

            if (entity.mobile != oldModel.mobile)
            {
                var isHava = await _UserServices.ExistsAsync(p => p.mobile == entity.mobile);
                if (isHava)
                {
                    jm.msg = "已存在此手机号码";
                    return jm;
                }
            }

            //事物处理过程开始

            if (!string.IsNullOrEmpty(entity.passWord)) oldModel.passWord = CommonHelper.Md5For32(entity.passWord);
            oldModel.mobile = entity.mobile;
            oldModel.sex = entity.sex;
            oldModel.birthday = entity.birthday;
            oldModel.avatarImage = entity.avatarImage;
            oldModel.nickName = entity.nickName;
            oldModel.grade = entity.grade;
            oldModel.updataTime = DateTime.Now;
            oldModel.status = entity.status;
            oldModel.userName = entity.userName;
            //事物处理过程结束
            var bl = await _UserServices.UpdateAsync(oldModel);
            jm.code = bl ? 0 : 1;
            jm.msg = bl ? GlobalConstVars.EditSuccess : GlobalConstVars.EditFailure;

            return jm;
        }

        #endregion

        #region 选择导出============================================================
        // POST: Api/FuFuShopUser/SelectExportExcel/10
        /// <summary>
        /// 选择导出
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost]
        [Description("选择导出")]
        public async Task<AdminUiCallBack> SelectExportExcel([FromBody] FMArrayIntIds entity)
        {
            var jm = new AdminUiCallBack();

            //创建Excel文件的对象
            var book = new HSSFWorkbook();
            //添加一个sheet
            var mySheet = book.CreateSheet("Sheet1");
            //获取list数据
            var listModel = await _UserServices.QueryListByClauseAsync(p => entity.id.Contains(p.id), p => p.id, OrderByType.Asc);
            //给sheet1添加第一行的头部标题
            var headerRow = mySheet.CreateRow(0);
            var headerStyle = ExcelHelper.GetHeaderStyle(book);

            var cell0 = headerRow.CreateCell(0);
            cell0.SetCellValue("用户ID");
            cell0.CellStyle = headerStyle;
            mySheet.SetColumnWidth(0, 10 * 256);

            var cell1 = headerRow.CreateCell(1);
            cell1.SetCellValue("用户名");
            cell1.CellStyle = headerStyle;
            mySheet.SetColumnWidth(1, 10 * 256);

            var cell2 = headerRow.CreateCell(2);
            cell2.SetCellValue("密码");
            cell2.CellStyle = headerStyle;
            mySheet.SetColumnWidth(2, 10 * 256);

            var cell3 = headerRow.CreateCell(3);
            cell3.SetCellValue("手机号");
            cell3.CellStyle = headerStyle;
            mySheet.SetColumnWidth(3, 10 * 256);

            var cell4 = headerRow.CreateCell(4);
            cell4.SetCellValue("性别[1男2女3未知]");
            cell4.CellStyle = headerStyle;
            mySheet.SetColumnWidth(4, 10 * 256);

            var cell5 = headerRow.CreateCell(5);
            cell5.SetCellValue("生日");
            cell5.CellStyle = headerStyle;
            mySheet.SetColumnWidth(5, 10 * 256);

            var cell6 = headerRow.CreateCell(6);
            cell6.SetCellValue("头像");
            cell6.CellStyle = headerStyle;
            mySheet.SetColumnWidth(6, 10 * 256);

            var cell7 = headerRow.CreateCell(7);
            cell7.SetCellValue("昵称");
            cell7.CellStyle = headerStyle;
            mySheet.SetColumnWidth(7, 10 * 256);

            var cell8 = headerRow.CreateCell(8);
            cell8.SetCellValue("余额");
            cell8.CellStyle = headerStyle;
            mySheet.SetColumnWidth(8, 10 * 256);

            var cell9 = headerRow.CreateCell(9);
            cell9.SetCellValue("积分");
            cell9.CellStyle = headerStyle;
            mySheet.SetColumnWidth(9, 10 * 256);

            var cell10 = headerRow.CreateCell(10);
            cell10.SetCellValue("用户等级");
            cell10.CellStyle = headerStyle;
            mySheet.SetColumnWidth(10, 10 * 256);

            var cell11 = headerRow.CreateCell(11);
            cell11.SetCellValue("创建时间");
            cell11.CellStyle = headerStyle;
            mySheet.SetColumnWidth(11, 10 * 256);

            var cell12 = headerRow.CreateCell(12);
            cell12.SetCellValue("更新时间");
            cell12.CellStyle = headerStyle;
            mySheet.SetColumnWidth(12, 10 * 256);

            var cell13 = headerRow.CreateCell(13);
            cell13.SetCellValue("状态[1正常2停用]");
            cell13.CellStyle = headerStyle;
            mySheet.SetColumnWidth(13, 10 * 256);

            var cell14 = headerRow.CreateCell(14);
            cell14.SetCellValue("推荐人");
            cell14.CellStyle = headerStyle;
            mySheet.SetColumnWidth(14, 10 * 256);

            var cell15 = headerRow.CreateCell(15);
            cell15.SetCellValue("关联三方账户");
            cell15.CellStyle = headerStyle;
            mySheet.SetColumnWidth(15, 10 * 256);

            var cell16 = headerRow.CreateCell(16);
            cell16.SetCellValue("删除标志 有数据就是删除");
            cell16.CellStyle = headerStyle;
            mySheet.SetColumnWidth(16, 10 * 256);

            headerRow.Height = 30 * 20;
            var commonCellStyle = ExcelHelper.GetCommonStyle(book);

            //将数据逐步写入sheet1各个行
            for (var i = 0; i < listModel.Count; i++)
            {
                var rowTemp = mySheet.CreateRow(i + 1);

                var rowTemp0 = rowTemp.CreateCell(0);
                rowTemp0.SetCellValue(listModel[i].id.ToString());
                rowTemp0.CellStyle = commonCellStyle;

                var rowTemp1 = rowTemp.CreateCell(1);
                rowTemp1.SetCellValue(listModel[i].userName);
                rowTemp1.CellStyle = commonCellStyle;

                var rowTemp2 = rowTemp.CreateCell(2);
                rowTemp2.SetCellValue(listModel[i].passWord);
                rowTemp2.CellStyle = commonCellStyle;

                var rowTemp3 = rowTemp.CreateCell(3);
                rowTemp3.SetCellValue(listModel[i].mobile);
                rowTemp3.CellStyle = commonCellStyle;

                var rowTemp4 = rowTemp.CreateCell(4);
                rowTemp4.SetCellValue(listModel[i].sex.ToString());
                rowTemp4.CellStyle = commonCellStyle;

                var rowTemp5 = rowTemp.CreateCell(5);
                rowTemp5.SetCellValue(listModel[i].birthday.ToString());
                rowTemp5.CellStyle = commonCellStyle;

                var rowTemp6 = rowTemp.CreateCell(6);
                rowTemp6.SetCellValue(listModel[i].avatarImage);
                rowTemp6.CellStyle = commonCellStyle;

                var rowTemp7 = rowTemp.CreateCell(7);
                rowTemp7.SetCellValue(listModel[i].nickName);
                rowTemp7.CellStyle = commonCellStyle;

                var rowTemp8 = rowTemp.CreateCell(8);
                rowTemp8.SetCellValue(listModel[i].balance.ToString());
                rowTemp8.CellStyle = commonCellStyle;

                var rowTemp9 = rowTemp.CreateCell(9);
                rowTemp9.SetCellValue(listModel[i].point.ToString());
                rowTemp9.CellStyle = commonCellStyle;

                var rowTemp10 = rowTemp.CreateCell(10);
                rowTemp10.SetCellValue(listModel[i].grade.ToString());
                rowTemp10.CellStyle = commonCellStyle;

                var rowTemp11 = rowTemp.CreateCell(11);
                rowTemp11.SetCellValue(listModel[i].createTime.ToString());
                rowTemp11.CellStyle = commonCellStyle;

                var rowTemp12 = rowTemp.CreateCell(12);
                rowTemp12.SetCellValue(listModel[i].updataTime.ToString());
                rowTemp12.CellStyle = commonCellStyle;

                var rowTemp13 = rowTemp.CreateCell(13);
                rowTemp13.SetCellValue(listModel[i].status.ToString());
                rowTemp13.CellStyle = commonCellStyle;

                var rowTemp14 = rowTemp.CreateCell(14);
                rowTemp14.SetCellValue(listModel[i].parentId.ToString());
                rowTemp14.CellStyle = commonCellStyle;

                var rowTemp15 = rowTemp.CreateCell(15);
                rowTemp15.SetCellValue(listModel[i].userWx.ToString());
                rowTemp15.CellStyle = commonCellStyle;

                var rowTemp16 = rowTemp.CreateCell(16);
                rowTemp16.SetCellValue(listModel[i].isDelete.ToString());
                rowTemp16.CellStyle = commonCellStyle;

            }
            // 导出excel
            string webRootPath = _webHostEnvironment.WebRootPath;
            string tpath = "/files/" + DateTime.Now.ToString("yyyy-MM-dd") + "/";
            string fileName = DateTime.Now.ToString("yyyyMMddHHmmssfff") + "-FuFuShopUser导出(选择结果).xls";
            string filePath = webRootPath + tpath;
            DirectoryInfo di = new DirectoryInfo(filePath);
            if (!di.Exists)
            {
                di.Create();
            }
            FileStream fileHssf = new FileStream(filePath + fileName, FileMode.Create);
            book.Write(fileHssf);
            fileHssf.Close();

            jm.code = 0;
            jm.msg = GlobalConstVars.ExcelExportSuccess;
            jm.data = tpath + fileName;

            return jm;
        }
        #endregion

        #region 查询导出============================================================
        // POST: Api/FuFuShopUser/QueryExportExcel/10
        /// <summary>
        /// 查询导出
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Description("查询导出")]
        public async Task<AdminUiCallBack> QueryExportExcel()
        {
            var jm = new AdminUiCallBack();

            var where = PredicateBuilder.True<FuFuShopUser>();
            //查询筛选

            //用户ID int
            var id = Request.Form["id"].FirstOrDefault().ObjectToInt(0);
            if (id > 0)
            {
                where = where.And(p => p.id == id);
            }
            //用户名 nvarchar
            var userName = Request.Form["userName"].FirstOrDefault();
            if (!string.IsNullOrEmpty(userName))
            {
                where = where.And(p => p.userName.Contains(userName));
            }
            //密码 nvarchar
            var passWord = Request.Form["passWord"].FirstOrDefault();
            if (!string.IsNullOrEmpty(passWord))
            {
                where = where.And(p => p.passWord.Contains(passWord));
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
            //头像 nvarchar
            var avatarImage = Request.Form["avatarImage"].FirstOrDefault();
            if (!string.IsNullOrEmpty(avatarImage))
            {
                where = where.And(p => p.avatarImage.Contains(avatarImage));
            }
            //昵称 nvarchar
            var nickName = Request.Form["nickName"].FirstOrDefault();
            if (!string.IsNullOrEmpty(nickName))
            {
                where = where.And(p => p.nickName.Contains(nickName));
            }
            //积分 int
            var point = Request.Form["point"].FirstOrDefault().ObjectToInt(0);
            if (point > 0)
            {
                where = where.And(p => p.point == point);
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
                var dt = createTime.ObjectToDate();
                where = where.And(p => p.createTime > dt);
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
            //关联三方账户 int
            var userWx = Request.Form["userWx"].FirstOrDefault().ObjectToInt(0);
            if (userWx > 0)
            {
                where = where.And(p => p.userWx == userWx);
            }
            //删除标志 有数据就是删除 bit
            var isDelete = Request.Form["isDelete"].FirstOrDefault();
            if (!string.IsNullOrEmpty(isDelete) && isDelete.ToLowerInvariant() == "true")
            {
                where = where.And(p => p.isDelete == true);
            }
            else if (!string.IsNullOrEmpty(isDelete) && isDelete.ToLowerInvariant() == "false")
            {
                where = where.And(p => p.isDelete == false);
            }
            //获取数据
            //创建Excel文件的对象
            var book = new HSSFWorkbook();
            //添加一个sheet
            var mySheet = book.CreateSheet("Sheet1");
            //获取list数据
            var listModel = await _UserServices.QueryListByClauseAsync(where, p => p.id, OrderByType.Asc);
            //给sheet1添加第一行的头部标题
            var headerRow = mySheet.CreateRow(0);
            var headerStyle = ExcelHelper.GetHeaderStyle(book);

            var cell0 = headerRow.CreateCell(0);
            cell0.SetCellValue("用户ID");
            cell0.CellStyle = headerStyle;
            mySheet.SetColumnWidth(0, 10 * 256);

            var cell1 = headerRow.CreateCell(1);
            cell1.SetCellValue("用户名");
            cell1.CellStyle = headerStyle;
            mySheet.SetColumnWidth(1, 10 * 256);

            var cell2 = headerRow.CreateCell(2);
            cell2.SetCellValue("密码");
            cell2.CellStyle = headerStyle;
            mySheet.SetColumnWidth(2, 10 * 256);

            var cell3 = headerRow.CreateCell(3);
            cell3.SetCellValue("手机号");
            cell3.CellStyle = headerStyle;
            mySheet.SetColumnWidth(3, 10 * 256);

            var cell4 = headerRow.CreateCell(4);
            cell4.SetCellValue("性别[1男2女3未知]");
            cell4.CellStyle = headerStyle;
            mySheet.SetColumnWidth(4, 10 * 256);

            var cell5 = headerRow.CreateCell(5);
            cell5.SetCellValue("生日");
            cell5.CellStyle = headerStyle;
            mySheet.SetColumnWidth(5, 10 * 256);

            var cell6 = headerRow.CreateCell(6);
            cell6.SetCellValue("头像");
            cell6.CellStyle = headerStyle;
            mySheet.SetColumnWidth(6, 10 * 256);

            var cell7 = headerRow.CreateCell(7);
            cell7.SetCellValue("昵称");
            cell7.CellStyle = headerStyle;
            mySheet.SetColumnWidth(7, 10 * 256);

            var cell8 = headerRow.CreateCell(8);
            cell8.SetCellValue("余额");
            cell8.CellStyle = headerStyle;
            mySheet.SetColumnWidth(8, 10 * 256);

            var cell9 = headerRow.CreateCell(9);
            cell9.SetCellValue("积分");
            cell9.CellStyle = headerStyle;
            mySheet.SetColumnWidth(9, 10 * 256);

            var cell10 = headerRow.CreateCell(10);
            cell10.SetCellValue("用户等级");
            cell10.CellStyle = headerStyle;
            mySheet.SetColumnWidth(10, 10 * 256);

            var cell11 = headerRow.CreateCell(11);
            cell11.SetCellValue("创建时间");
            cell11.CellStyle = headerStyle;
            mySheet.SetColumnWidth(11, 10 * 256);

            var cell12 = headerRow.CreateCell(12);
            cell12.SetCellValue("更新时间");
            cell12.CellStyle = headerStyle;
            mySheet.SetColumnWidth(12, 10 * 256);

            var cell13 = headerRow.CreateCell(13);
            cell13.SetCellValue("状态[1正常2停用]");
            cell13.CellStyle = headerStyle;
            mySheet.SetColumnWidth(13, 10 * 256);

            var cell14 = headerRow.CreateCell(14);
            cell14.SetCellValue("推荐人");
            cell14.CellStyle = headerStyle;
            mySheet.SetColumnWidth(14, 10 * 256);

            var cell15 = headerRow.CreateCell(15);
            cell15.SetCellValue("关联三方账户");
            cell15.CellStyle = headerStyle;
            mySheet.SetColumnWidth(15, 10 * 256);

            var cell16 = headerRow.CreateCell(16);
            cell16.SetCellValue("删除标志 有数据就是删除");
            cell16.CellStyle = headerStyle;
            mySheet.SetColumnWidth(16, 10 * 256);


            headerRow.Height = 30 * 20;
            var commonCellStyle = ExcelHelper.GetCommonStyle(book);

            //将数据逐步写入sheet1各个行
            for (var i = 0; i < listModel.Count; i++)
            {
                var rowTemp = mySheet.CreateRow(i + 1);


                var rowTemp0 = rowTemp.CreateCell(0);
                rowTemp0.SetCellValue(listModel[i].id.ToString());
                rowTemp0.CellStyle = commonCellStyle;



                var rowTemp1 = rowTemp.CreateCell(1);
                rowTemp1.SetCellValue(listModel[i].userName);
                rowTemp1.CellStyle = commonCellStyle;



                var rowTemp2 = rowTemp.CreateCell(2);
                rowTemp2.SetCellValue(listModel[i].passWord);
                rowTemp2.CellStyle = commonCellStyle;



                var rowTemp3 = rowTemp.CreateCell(3);
                rowTemp3.SetCellValue(listModel[i].mobile);
                rowTemp3.CellStyle = commonCellStyle;



                var rowTemp4 = rowTemp.CreateCell(4);
                rowTemp4.SetCellValue(listModel[i].sex.ToString());
                rowTemp4.CellStyle = commonCellStyle;



                var rowTemp5 = rowTemp.CreateCell(5);
                rowTemp5.SetCellValue(listModel[i].birthday.ToString());
                rowTemp5.CellStyle = commonCellStyle;



                var rowTemp6 = rowTemp.CreateCell(6);
                rowTemp6.SetCellValue(listModel[i].avatarImage);
                rowTemp6.CellStyle = commonCellStyle;



                var rowTemp7 = rowTemp.CreateCell(7);
                rowTemp7.SetCellValue(listModel[i].nickName);
                rowTemp7.CellStyle = commonCellStyle;



                var rowTemp8 = rowTemp.CreateCell(8);
                rowTemp8.SetCellValue(listModel[i].balance.ToString());
                rowTemp8.CellStyle = commonCellStyle;



                var rowTemp9 = rowTemp.CreateCell(9);
                rowTemp9.SetCellValue(listModel[i].point.ToString());
                rowTemp9.CellStyle = commonCellStyle;



                var rowTemp10 = rowTemp.CreateCell(10);
                rowTemp10.SetCellValue(listModel[i].grade.ToString());
                rowTemp10.CellStyle = commonCellStyle;



                var rowTemp11 = rowTemp.CreateCell(11);
                rowTemp11.SetCellValue(listModel[i].createTime.ToString());
                rowTemp11.CellStyle = commonCellStyle;



                var rowTemp12 = rowTemp.CreateCell(12);
                rowTemp12.SetCellValue(listModel[i].updataTime.ToString());
                rowTemp12.CellStyle = commonCellStyle;



                var rowTemp13 = rowTemp.CreateCell(13);
                rowTemp13.SetCellValue(listModel[i].status.ToString());
                rowTemp13.CellStyle = commonCellStyle;



                var rowTemp14 = rowTemp.CreateCell(14);
                rowTemp14.SetCellValue(listModel[i].parentId.ToString());
                rowTemp14.CellStyle = commonCellStyle;



                var rowTemp15 = rowTemp.CreateCell(15);
                rowTemp15.SetCellValue(listModel[i].userWx.ToString());
                rowTemp15.CellStyle = commonCellStyle;



                var rowTemp16 = rowTemp.CreateCell(16);
                rowTemp16.SetCellValue(listModel[i].isDelete.ToString());
                rowTemp16.CellStyle = commonCellStyle;


            }
            // 写入到excel
            string webRootPath = _webHostEnvironment.WebRootPath;
            string tpath = "/files/" + DateTime.Now.ToString("yyyy-MM-dd") + "/";
            string fileName = DateTime.Now.ToString("yyyyMMddHHmmssfff") + "-FuFuShopUser导出(查询结果).xls";
            string filePath = webRootPath + tpath;
            DirectoryInfo di = new DirectoryInfo(filePath);
            if (!di.Exists)
            {
                di.Create();
            }
            FileStream fileHssf = new FileStream(filePath + fileName, FileMode.Create);
            book.Write(fileHssf);
            fileHssf.Close();

            jm.code = 0;
            jm.msg = GlobalConstVars.ExcelExportSuccess;
            jm.data = tpath + fileName;

            return jm;
        }
        #endregion

    }
}