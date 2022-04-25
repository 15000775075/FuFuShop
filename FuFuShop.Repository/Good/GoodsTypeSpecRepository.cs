

using FuFuShop.Common.AppSettings;
using FuFuShop.Common.Helper;
using FuFuShop.Model.Entities;
using FuFuShop.Model.FromBody;
using FuFuShop.Model.ViewModels.UI;
using FuFuShop.Repository.BaseRepository;
using FuFuShop.Repository.UnitOfWork;

namespace FuFuShop.Repository
{
    /// <summary>
    /// 商品类型属性表 接口实现
    /// </summary>
    public class GoodsTypeSpecRepository : BaseRepository<GoodsTypeSpec>, IGoodsTypeSpecRepository
    {

        public GoodsTypeSpecRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {

        }


        /// <summary>
        ///     使用事务重写异步插入方法
        /// </summary>
        /// <param name="entity">实体数据</param>
        /// <returns></returns>
        public async Task<AdminUiCallBack> InsertAsync(FmGoodsTypeSpecInsert entity)
        {
            var jm = new AdminUiCallBack();

            if (entity.value == null)
            {
                jm.msg = "请添加属性值"; return jm;
            }

            for (int i = 0; i < entity.value.Count; i++)
            {
                entity.value[i] = entity.value[i].Trim();
                if (GoodsHelper.FilterChar(entity.value[i]) == false) continue;
                jm.msg = "属性值不符合支持规则"; return jm;
            }

            if (entity.value.GroupBy(n => n).Any(c => c.Count() > 1))
            {
                jm.msg = "属性值不允许有相同"; return jm;
            }

            var goodsTypeSpec = new GoodsTypeSpec();
            goodsTypeSpec.name = entity.name;
            goodsTypeSpec.sort = entity.sort;
            var specId = await DbClient.Insertable(goodsTypeSpec).ExecuteReturnIdentityAsync();
            if (specId > 0 && entity.value != null && entity.value.Count > 0)
            {
                var list = new List<GoodsTypeSpecValue>();
                for (var index = 0; index < entity.value.Count; index++)
                {
                    var item = entity.value[index];
                    list.Add(new GoodsTypeSpecValue()
                    {
                        specId = specId,
                        value = item,
                        sort = index + 1
                    });
                }
                var bl = await DbClient.Insertable(list).ExecuteCommandAsync() > 0;
                jm.code = bl ? 0 : 1;
                jm.msg = bl ? GlobalConstVars.CreateSuccess : GlobalConstVars.CreateFailure;
            }


            return jm;
        }

        /// <summary>
        /// 重写异步更新方法方法
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<AdminUiCallBack> UpdateAsync(FmGoodsTypeSpecUpdate entity)
        {
            var jm = new AdminUiCallBack();

            if (entity.value == null)
            {
                jm.msg = "请添加属性值"; return jm;
            }
            for (int i = 0; i < entity.value.Count; i++)
            {
                entity.value[i] = entity.value[i].Trim();
                if (GoodsHelper.FilterChar(entity.value[i]) == false) continue;
                jm.msg = "属性值不符合支持规则"; return jm;
            }
            if (entity.value.GroupBy(n => n).Any(c => c.Count() > 1))
            {
                jm.msg = "属性值不允许有相同"; return jm;
            }

            var oldModel = await DbClient.Queryable<GoodsTypeSpec>().In(entity.id).SingleAsync();
            if (oldModel == null)
            {
                jm.msg = "不存在此信息";
                return jm;
            }
            oldModel.name = entity.name;
            oldModel.sort = entity.sort;
            var bl = await DbClient.Updateable(oldModel).ExecuteCommandHasChangeAsync();
            if (bl)
            {
                var oldValues = await DbClient.Queryable<GoodsTypeSpecValue>().OrderBy(p => p.sort)
                    .Where(p => p.specId == oldModel.id).ToListAsync();

                //获取需要删除的数据库数据
                var deleteValues = oldValues.Where(p => !entity.value.Contains(p.value)).ToList();
                //删除旧数据
                if (deleteValues.Any()) bl = await DbClient.Deleteable<GoodsTypeSpecValue>(deleteValues).ExecuteCommandHasChangeAsync();

                //新数据
                var values = oldValues.Select(p => p.value).ToList();
                var newValues = entity.value.Except(values).ToList();

                //插入新数据
                if (newValues.Any())
                {
                    var newList = newValues.Select((t, index) => new GoodsTypeSpecValue() { specId = oldModel.id, value = t, sort = oldValues.Count + index }).ToList();
                    bl = await DbClient.Insertable<GoodsTypeSpecValue>(newList).ExecuteCommandAsync() > 0;
                }
            }
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

            var bl = await DbClient.Deleteable<GoodsTypeSpec>(id).ExecuteCommandHasChangeAsync();
            jm.code = bl ? 0 : 1;
            jm.msg = bl ? GlobalConstVars.DeleteSuccess : GlobalConstVars.DeleteFailure;
            if (bl)
            {
                await DbClient.Deleteable<GoodsTypeSpecValue>(p => p.specId == (int)id).ExecuteCommandHasChangeAsync();
            }

            return jm;
        }

    }
}
