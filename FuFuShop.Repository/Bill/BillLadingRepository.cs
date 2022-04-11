using FuFuShop.Common.AppSettings;
using FuFuShop.Common.Helper;
using FuFuShop.Model.Entities;
using FuFuShop.Model.ViewModels.UI;
using FuFuShop.Repository.BaseRepository;
using FuFuShop.Repository.UnitOfWork;

namespace FuFuShop.Repository
{
    /// <summary>
    ///     提货单表 接口实现
    /// </summary>
    public class BillLadingRepository : BaseRepository<BillLading>, IBillLadingRepository
    {
        public BillLadingRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }


        /// <summary>
        ///     添加提货单
        /// </summary>
        /// <returns></returns>
        public async Task<WebApiCallBack> AddData(string orderId, int storeId, string name, string mobile)
        {
            var jm = new WebApiCallBack();

            var model = new BillLading();
            model.id = GenerateId();
            model.orderId = orderId;
            model.storeId = storeId;
            model.name = name;
            model.mobile = mobile;
            model.clerkId = 0;
            model.status = false;
            model.createTime = DateTime.Now;
            model.isDel = false;

            //事物处理过程结束
            await DbClient.Insertable(model).ExecuteCommandAsync();
            jm.code = 0;
            jm.msg = "添加成功";

            return jm;
        }


        /// <summary>
        ///     生成唯一提货单号
        /// </summary>
        /// <returns></returns>
        private string GenerateId()
        {
            bool bl;
            string id;
            do
            {
                id = CommonHelper.GetSerialNumberType((int)GlobalEnumVars.SerialNumberType.提货单号);
                var id1 = id;
                bl = DbClient.Queryable<BillLading>().Any(p => p.id == id1);
            } while (bl);

            return id;
        }
    }
}