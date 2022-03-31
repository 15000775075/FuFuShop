using FuFuShop.Model.Entitys;
using FuFuShop.Model.ViewModels.UI;
using FuFuShop.Repository.BaseRepository;
using FuFuShop.Repository.UnitOfWork;
using SqlSugar;

namespace FuFuShop.Repository
{
    /// <summary>
    /// 用户表 接口实现
    /// </summary>
    public class UserRepository : BaseRepository<FuFuShopUser>, IUserRepository
    {
        public UserRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }


        ///// <summary>
        /////     获取下级推广用户数量
        ///// </summary>
        ///// <param name="parentId">父类序列</param>
        ///// <param name="type">1获取1级，其他为2级</param>
        ///// <param name="thisMonth">显示当月</param>
        ///// <returns></returns>
        //public async Task<int> QueryChildCountAsync(int parentId, int type = 1, bool thisMonth = false)
        //{
        //    var totalSum = 0;

        //    DateTime dt = DateTime.Now;
        //    //本月第一天时间      
        //    DateTime dtFirst = dt.AddDays(1 - (dt.Day));
        //    dtFirst = new DateTime(dtFirst.Year, dtFirst.Month, dtFirst.Day, 0, 0, 0);
        //    //获得某年某月的天数    
        //    int year = dt.Date.Year;
        //    int month = dt.Date.Month;
        //    int dayCount = DateTime.DaysInMonth(year, month);
        //    //本月最后一天时间    
        //    DateTime dtLast = dtFirst.AddDays(dayCount - 1);


        //    if (type == 1)
        //    {
        //        totalSum = await DbClient.Queryable<User>().Where(p => p.parentId == parentId)
        //            .WhereIF(thisMonth, p => p.createTime > dtFirst && p.createTime < dtLast).With(SqlWith.NoLock)
        //            .CountAsync();
        //    }
        //    else
        //    {
        //        totalSum = await DbClient.Queryable<User, User>(
        //                (p, sParentUser) => new object[]
        //                {
        //                    JoinType.Left,p.parentId==sParentUser.id
        //                }
        //            )
        //            .Select((p, sParentUser) => new User()
        //            {
        //                id = p.id,
        //                parentId = p.parentId,
        //                childNum = SqlFunc.Subqueryable<User>().Where(o => o.parentId == p.id).WhereIF(thisMonth, o => o.createTime > dtFirst && o.createTime < dtLast).Count()
        //            })
        //            .MergeTable().With(SqlWith.Null)
        //            .Where(p => p.parentId == parentId)
        //            .SumAsync(p => p.childNum);
        //    }

        //    return totalSum;
        //}




        ///// <summary>
        /////     根据条件查询分页数据
        ///// </summary>
        ///// <param name="predicate">判断集合</param>
        ///// <param name="orderByType">排序方式</param>
        ///// <param name="pageIndex">当前页面索引</param>
        ///// <param name="pageSize">分布大小</param>
        ///// <param name="orderByExpression"></param>
        ///// <returns></returns>
        //public async Task<IPageList<User>> QueryPageAsync(Expression<Func<User, bool>> predicate,
        //    Expression<Func<User, object>> orderByExpression, OrderByType orderByType, int pageIndex = 1,
        //    int pageSize = 20)
        //{
        //    RefAsync<int> totalCount = 0;
        //    var page = await DbClient.Queryable<User, UserWeChatInfo, User>(
        //            (p, sWeChatInfo, sParentUser) => new object[]
        //            {
        //                JoinType.Left,p.id==sWeChatInfo.userId,
        //                JoinType.Left,p.parentId==sParentUser.id
        //            }
        //        )
        //        .Select((p, sWeChatInfo, sParentUser) => new User()
        //        {
        //            id = p.id,
        //            userName = p.userName,
        //            passWord = p.passWord,
        //            mobile = p.mobile,
        //            sex = p.sex,
        //            birthday = p.birthday,
        //            avatarImage = p.avatarImage,
        //            nickName = p.nickName,
        //            balance = p.balance,
        //            point = p.point,
        //            grade = p.grade,
        //            createTime = p.createTime,
        //            updataTime = p.updataTime,
        //            status = p.status,
        //            parentId = p.parentId,
        //            userWx = p.userWx,
        //            isDelete = p.isDelete,
        //            type = (int)sWeChatInfo.type,
        //            parentNickName = sParentUser.nickName,
        //            childNum = SqlFunc.Subqueryable<User>().Where(o => o.parentId == p.id).Count()
        //        })
        //        .MergeTable().With(SqlWith.Null)
        //        .OrderBy(orderByExpression, orderByType)
        //        .Where(predicate)
        //        .ToPageListAsync(pageIndex, pageSize, totalCount);
        //    var list = new PageList<User>(page, pageIndex, pageSize, totalCount);
        //    return list;
        //}


        /// <summary>
        /// 按天统计新会员
        /// </summary>
        /// <returns></returns>
        public async Task<List<StatisticsOut>> Statistics(int day)
        {
            var dt = DateTime.Now.AddDays(-day);
            var list = await DbClient.Queryable<FuFuShopUser>()
                .Where(p => p.createTime >= dt)
                .Select(it => new
                {
                    it.id,
                    createTime = it.createTime.Date
                })
                .MergeTable()
                .GroupBy(it => it.createTime)
                .Select(it => new StatisticsOut { day = it.createTime.ToString("yyyy-MM-dd"), nums = SqlFunc.AggregateCount(it.id) })
                .ToListAsync();

            return list;
        }

        ///// <summary>
        ///// 按天统计当天下单活跃会员
        ///// </summary>
        ///// <returns></returns>
        //public async Task<List<StatisticsOut>> StatisticsOrder(int day)
        //{

        //    var outs = new List<StatisticsOut>();

        //    for (int i = 0; i < day; i++)
        //    {
        //        var dt = DateTime.Now;
        //        var where = PredicateBuilder.True<CoreCmsOrder>();
        //        var currDay = DateTime.Now.ToString("yyyy-MM-dd");
        //        if (i == 0)
        //        {
        //            where = where.And(p => p.createTime < DateTime.Now);
        //            currDay = DateTime.Now.ToString("yyyy-MM-dd");
        //        }
        //        else
        //        {
        //            var iDt = DateTime.Now.AddDays(-i);
        //            var dtEnd = new DateTime(iDt.Year, iDt.Month, iDt.Day, 23, 59, 59);
        //            where = where.And(p => p.createTime < dtEnd);
        //            currDay = DateTime.Now.AddDays(-i).ToString("yyyy-MM-dd");
        //        }
        //        var iDt2 = DateTime.Now.AddDays(-i);
        //        var dtStart = new DateTime(iDt2.Year, iDt2.Month, iDt2.Day, 00, 00, 00);
        //        where = where.And(p => p.createTime >= dtStart);

        //        var item = new StatisticsOut();
        //        item.nums = await DbClient.Queryable<CoreCmsOrder>().Where(where).GroupBy(p => p.userId).Select(p => p.userId).CountAsync();
        //        item.day = currDay;
        //        outs.Add(item);
        //    }

        //    return outs;
        //}

    }
}
