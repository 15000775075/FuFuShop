using FuFuShop.Common.AppSettings;
using FuFuShop.Model.Entities;
using FuFuShop.Repository;
using FuFuShop.Repository.Good;
using FuFuShop.Repository.UnitOfWork;
using FuFuShop.Services.BaseServices;
using Microsoft.Extensions.DependencyInjection;

namespace FuFuShop.Services
{
    /// <summary>
    /// 订单明细表 接口实现
    /// </summary>
    public class OrderItemServices : BaseServices<OrderItem>, IOrderItemServices
    {
        private readonly IOrderItemRepository _dal;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IServiceProvider _serviceProvider;

        public OrderItemServices(IUnitOfWork unitOfWork,
            IServiceProvider serviceProvider,
            IOrderItemRepository dal)
        {
            _serviceProvider = serviceProvider;
            _dal = dal;
            BaseDal = dal;
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// 发货数量
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="item">发货明细</param>
        /// <returns></returns>
        public async Task<bool> ship(string orderId, Dictionary<int, int> item)
        {
            using (var container = _serviceProvider.CreateScope())
            {
                var goodsRepository = container.ServiceProvider.GetService<IGoodsRepository>();

                var isOver = true;     //是否发完了，true发完了，false未发完
                var list = await base.QueryListByClauseAsync(p => p.orderId == orderId);
                foreach (var child in list)
                {
                    if (item.ContainsKey(child.productId))
                    {
                        var maxNum = child.nums - child.sendNums; //还需要减掉已发数量

                        //还需要减掉已退的数量
                        var reshipNums = _dal.GetaftersalesNums(orderId, child.sn);
                        maxNum = maxNum - reshipNums;

                        if (item[child.productId] > maxNum)  //如果发超了怎么办
                        {
                            throw new System.Exception(orderId + "的" + child.sn + "发超了");
                        }

                        if (isOver && item[child.productId] < maxNum)  //判断是否订单发完了，有一个没发完，就是未发完
                        {
                            isOver = false;
                        }

                        var updateSendNums = item[child.productId] + child.sendNums;
                        await _dal.UpdateAsync(p => new OrderItem() { sendNums = updateSendNums },
                            p => p.id == child.id);

                        //发货后，减库存
                        goodsRepository.ChangeStock(child.productId, GlobalEnumVars.OrderChangeStockType.send.ToString(), item[child.productId]);

                        item.Remove(child.productId);
                    }
                }
                //如果没发完，也报错
                if (item.Count > 0)
                {
                    throw new System.Exception("发货明细里包含订单之外的商品");
                }
                return isOver;
            }


        }

    }
}