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
using FuFuShop.Model.Entities;
using FuFuShop.Model.ViewModels.Basics;
using FuFuShop.Model.ViewModels.UI;
using FuFuShop.Repository.BaseRepository;
using FuFuShop.Repository.UnitOfWork;
using SqlSugar;
using System.Linq.Expressions;

namespace FuFuShop.Repository
{
    /// <summary>
    /// 商品评价表 接口实现
    /// </summary>
    public class GoodsCommentRepository : BaseRepository<GoodsComment>, IGoodsCommentRepository
    {
        public GoodsCommentRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        #region 实现重写增删改查操作==========================================================

        /// <summary>
        /// 重写异步插入方法
        /// </summary>
        /// <param name="entity">实体数据</param>
        /// <returns></returns>
        public new async Task<AdminUiCallBack> InsertAsync(GoodsComment entity)
        {
            var jm = new AdminUiCallBack();

            var bl = await DbClient.Insertable(entity).ExecuteReturnIdentityAsync() > 0;
            jm.code = bl ? 0 : 1;
            jm.msg = bl ? GlobalConstVars.CreateSuccess : GlobalConstVars.CreateFailure;

            return jm;
        }

        /// <summary>
        /// 重写异步更新方法
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public new async Task<AdminUiCallBack> UpdateAsync(GoodsComment entity)
        {
            var jm = new AdminUiCallBack();

            var oldModel = await DbClient.Queryable<GoodsComment>().In(entity.id).SingleAsync();
            if (oldModel == null)
            {
                jm.msg = "不存在此信息";
                return jm;
            }
            //事物处理过程开始
            oldModel.id = entity.id;
            oldModel.commentId = entity.commentId;
            oldModel.score = entity.score;
            oldModel.userId = entity.userId;
            oldModel.goodsId = entity.goodsId;
            oldModel.orderId = entity.orderId;
            oldModel.addon = entity.addon;
            oldModel.images = entity.images;
            oldModel.contentBody = entity.contentBody;
            oldModel.sellerContent = entity.sellerContent;
            oldModel.isDisplay = entity.isDisplay;
            oldModel.createTime = entity.createTime;

            //事物处理过程结束
            var bl = await DbClient.Updateable(oldModel).ExecuteCommandHasChangeAsync();
            jm.code = bl ? 0 : 1;
            jm.msg = bl ? GlobalConstVars.EditSuccess : GlobalConstVars.EditFailure;

            return jm;
        }

        /// <summary>
        /// 重写异步更新方法
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public new async Task<AdminUiCallBack> UpdateAsync(List<GoodsComment> entity)
        {
            var jm = new AdminUiCallBack();

            var bl = await DbClient.Updateable(entity).ExecuteCommandHasChangeAsync();
            jm.code = bl ? 0 : 1;
            jm.msg = bl ? GlobalConstVars.EditSuccess : GlobalConstVars.EditFailure;

            return jm;
        }

        /// <summary>
        /// 重写删除指定ID的数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public new async Task<AdminUiCallBack> DeleteByIdAsync(object id)
        {
            var jm = new AdminUiCallBack();

            var bl = await DbClient.Deleteable<GoodsComment>(id).ExecuteCommandHasChangeAsync();
            jm.code = bl ? 0 : 1;
            jm.msg = bl ? GlobalConstVars.DeleteSuccess : GlobalConstVars.DeleteFailure;

            return jm;
        }

        /// <summary>
        /// 重写删除指定ID集合的数据(批量删除)
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public new async Task<AdminUiCallBack> DeleteByIdsAsync(int[] ids)
        {
            var jm = new AdminUiCallBack();

            var bl = await DbClient.Deleteable<GoodsComment>().In(ids).ExecuteCommandHasChangeAsync();
            jm.code = bl ? 0 : 1;
            jm.msg = bl ? GlobalConstVars.DeleteSuccess : GlobalConstVars.DeleteFailure;

            return jm;
        }

        #endregion


        /// <summary>
        /// 商家回复评价
        /// </summary>
        /// <param name="id">序列</param>
        /// <param name="sellerContent">回复内容</param>
        /// <returns></returns>
        public async Task<AdminUiCallBack> Reply(int id, string sellerContent)
        {
            var jm = new AdminUiCallBack();

            var oldModel = await DbClient.Queryable<GoodsComment>().In(id).SingleAsync();
            if (oldModel == null)
            {
                jm.msg = "不存在此信息";
                return jm;
            }
            //事物处理过程开始
            oldModel.sellerContent = sellerContent;

            //事物处理过程结束
            var bl = await DbClient.Updateable(oldModel).ExecuteCommandHasChangeAsync();
            jm.code = bl ? 0 : 1;
            jm.msg = bl ? GlobalConstVars.EditSuccess : GlobalConstVars.EditFailure;

            return jm;
        }



        /// <summary>
        /// 获取单个详情数据
        /// </summary>
        /// <param name="predicate">判断集合</param>
        /// <param name="orderByType">排序方式</param>
        /// <param name="orderByExpression"></param>
        /// <returns></returns>
        public async Task<GoodsComment> DetailsByIdAsync(Expression<Func<GoodsComment, bool>> predicate,
            Expression<Func<GoodsComment, object>> orderByExpression, OrderByType orderByType)
        {
            var model = await DbClient.Queryable<GoodsComment, FuFuShopUser, Goods>((p, cUser, cGood) => new JoinQueryInfos(
                    JoinType.Left, p.userId == cUser.id,
                    JoinType.Left, p.goodsId == cGood.id
                ))
                .Select((p, cUser, cGood) => new GoodsComment
                {
                    id = p.id,
                    commentId = p.commentId,
                    score = p.score,
                    userId = p.userId,
                    goodsId = p.goodsId,
                    orderId = p.orderId,
                    addon = p.addon,
                    images = p.images,
                    contentBody = p.contentBody,
                    sellerContent = p.sellerContent,
                    isDisplay = p.isDisplay,
                    createTime = p.createTime,
                    avatarImage = cUser.avatarImage,
                    nickName = cUser.nickName,
                    mobile = cUser.mobile,
                    goodName = cGood.name,
                })
                .MergeTable()
                .OrderByIF(orderByExpression != null, orderByExpression, orderByType)
                .Where(predicate)
                .FirstAsync();

            if (model != null)
            {
                model.imagesArr = !string.IsNullOrEmpty(model.images) ? model.images.Split(",") : new String[0];
            }

            return model;
        }


        #region 重写根据条件查询分页数据
        /// <summary>
        ///     重写根据条件查询分页数据
        /// </summary>
        /// <param name="predicate">判断集合</param>
        /// <param name="orderByType">排序方式</param>
        /// <param name="pageIndex">当前页面索引</param>
        /// <param name="pageSize">分布大小</param>
        /// <param name="orderByExpression"></param>
        /// <param name="blUseNoLock">是否使用WITH(NOLOCK)</param>
        /// <returns></returns>
        public new async Task<IPageList<GoodsComment>> QueryPageAsync(Expression<Func<GoodsComment, bool>> predicate,
            Expression<Func<GoodsComment, object>> orderByExpression, OrderByType orderByType, int pageIndex = 1,
            int pageSize = 20, bool blUseNoLock = false)
        {
            RefAsync<int> totalCount = 0;

            List<GoodsComment> page = await DbClient.Queryable<GoodsComment, FuFuShopUser, Goods>((p, cUser, cGood) => new JoinQueryInfos(
                    JoinType.Left, p.userId == cUser.id,
                    JoinType.Left, p.goodsId == cGood.id
                ))
                .Select((p, cUser, cGood) => new GoodsComment
                {
                    id = p.id,
                    commentId = p.commentId,
                    score = p.score,
                    userId = p.userId,
                    goodsId = p.goodsId,
                    orderId = p.orderId,
                    addon = p.addon,
                    images = p.images,
                    contentBody = p.contentBody,
                    sellerContent = p.sellerContent,
                    isDisplay = p.isDisplay,
                    createTime = p.createTime,
                    avatarImage = cUser.avatarImage,
                    nickName = cUser.nickName,
                    mobile = cUser.mobile,
                    goodName = cGood.name,
                })
                .MergeTable()//将上面的操作变成一个表 mergetable
                .OrderByIF(orderByExpression != null, orderByExpression, orderByType)
                .Where(predicate)
                .ToPageListAsync(pageIndex, pageSize, totalCount);

            if (page.Any())
            {
                foreach (var item in page)
                {
                    item.imagesArr = !string.IsNullOrEmpty(item.images) ? item.images.Split(",") : new String[0];
                }
            }


            var list = new PageList<GoodsComment>(page, pageIndex, pageSize, totalCount);
            return list;
        }

        #endregion

    }
}
