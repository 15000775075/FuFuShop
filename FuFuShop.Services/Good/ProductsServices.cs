using FuFuShop.Common.Helper;
using FuFuShop.Model.Entities;
using FuFuShop.Model.FromBody;
using FuFuShop.Model.ViewModels.DTO;
using FuFuShop.Repository.Good;
using FuFuShop.Repository.UnitOfWork;
using FuFuShop.Services.BaseServices;
using FuFuShop.Services.User;
using Microsoft.Extensions.DependencyInjection;

namespace FuFuShop.Services.Good
{
    /// <summary>
    /// 货品表 接口实现
    /// </summary>
    public class ProductsServices : BaseServices<Products>, IProductsServices
    {
        private readonly IProductsRepository _dal;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IServiceProvider _serviceProvider;

        public ProductsServices(IUnitOfWork unitOfWork, IProductsRepository dal,
            IServiceProvider serviceProvider
            )
        {
            _dal = dal;
            BaseDal = dal;
            _unitOfWork = unitOfWork;
            _serviceProvider = serviceProvider;
        }

        /// <summary>
        /// 根据货品ID获取货品信息
        /// </summary>
        /// <param name="id">货品序列</param>
        /// <param name="isPromotion">是否计算促销</param>
        /// <param name="userId">用户序列</param>
        /// <param name="type">类型</param>
        /// <param name="groupId"></param>
        /// <returns></returns>
        public async Task<Products> GetProductInfo(int id, bool isPromotion, int userId, string type = "goods", int groupId = 0)
        {
            using var container = _serviceProvider.CreateScope();

            var orderServices = container.ServiceProvider.GetService<IOrderServices>();
            var goodsServices = container.ServiceProvider.GetService<IGoodsServices>();
            var userServices = container.ServiceProvider.GetService<IUserServices>();

            //获取货品
            var productModel = await _dal.QueryByClauseAsync(p => p.id == id);
            if (productModel == null) return null;
            //获取商品信息
            var goods = await goodsServices.QueryByIdAsync(productModel.goodsId);
            if (goods == null) return null;
            //DTO映射
            productModel.bn = goods.bn;
            productModel.images = !string.IsNullOrEmpty(productModel.images) ? GoodsHelper.GetOneImage(productModel.images) : GoodsHelper.GetOneImage(goods.images);
            productModel.totalStock = Convert.ToInt32(productModel.stock);
            productModel.stock = GoodsHelper.GetStock(productModel.stock, productModel.freezeStock);
            productModel.name = goods.name;

            var price = productModel.price;
            productModel.price = price;

            //如果是多规格商品，算多规格
            if (goods.openSpec == 1 && !string.IsNullOrEmpty(goods.spesDesc))
            {
                var defaultSpec = new Dictionary<string, Dictionary<string, DefaultSpesDesc>>();
                //一级拆分
                var spesDescArr = goods.spesDesc.Split("|");
                var productSpecDescArr = productModel.spesDesc.Split(",");
                foreach (var item in spesDescArr)
                {
                    //小类拆分
                    var itemArr = item.Split(".");
                    //键值对处理
                    var keyValue = itemArr[1].Split(":");
                    var defaultSpesDesc = new DefaultSpesDesc();
                    defaultSpesDesc.name = keyValue[1];
                    foreach (var childItem in productSpecDescArr)
                    {
                        if (childItem == itemArr[1])
                        {
                            defaultSpesDesc.isDefault = true;
                        }
                    }
                    if (defaultSpec.ContainsKey(keyValue[0]))
                    {
                        defaultSpec[keyValue[0]].Add(keyValue[1], defaultSpesDesc);
                    }
                    else
                    {
                        var a = new Dictionary<string, DefaultSpesDesc> { { keyValue[1], defaultSpesDesc } };

                        defaultSpec.Add(keyValue[0], a);
                    }
                }
                //取其他货品信息
                var otherProduts = await _dal.QueryListByClauseAsync(t => t.goodsId == goods.id && t.isDel == false && t.id != productModel.id);
                if (otherProduts.Any())
                {
                    foreach (var item in defaultSpec)
                    {
                        foreach (var childItem in item.Value)
                        {
                            //如果是默认选中的，跳出本次
                            if (childItem.Value.isDefault) continue;
                            //当前主货品sku
                            var tempProductSpesDesc = productSpecDescArr;
                            //替换当前sku的当前值为当前遍历的值
                            for (var i = 0; i < tempProductSpesDesc.Length; i++)
                            {
                                if (tempProductSpesDesc[i].Contains(item.Key)) tempProductSpesDesc[i] = item.Key + ":" + childItem.Key;
                            }
                            //循环所有货品，找到对应的多规格
                            foreach (var o in otherProduts)
                            {
                                var otherProductSpesDesc = o.spesDesc.Split(",");
                                if (!tempProductSpesDesc.Except(otherProductSpesDesc).Any())
                                {
                                    childItem.Value.productId = o.id;
                                    break;
                                }
                            }
                        }
                    }
                }
                productModel.defaultSpecificationDescription = defaultSpec;
            }

            productModel.amount = productModel.price;
            productModel.promotionList = new Dictionary<int, WxNameTypeDto>();
            productModel.promotionAmount = 0;

            //开启计算促销
            if (isPromotion)
            {
                //模拟购物车数据库结构，去取促销信息
                var miniCart = new CartDto();
                miniCart.userId = userId;
                miniCart.goodsAmount = productModel.amount;
                miniCart.amount = productModel.amount;
                var listOne = new CartProducts()
                {
                    id = 0,
                    isSelect = true,
                    userId = userId,
                    productId = productModel.id,
                    nums = 1,
                    products = productModel
                };
                miniCart.list.Add(listOne);

                var cartModel = new CartDto();



                //把促销信息和新的价格等，覆盖到这里
                var promotionList = cartModel.promotionList;

                if (cartModel.list[0].products.promotionList.Count > 0)
                {
                    //把订单促销和商品促销合并,都让他显示
                    foreach (KeyValuePair<int, WxNameTypeDto> kvp in cartModel.list[0].products.promotionList)
                    {
                        if (promotionList.ContainsKey(kvp.Key))
                        {
                            promotionList[kvp.Key] = kvp.Value;
                        }
                        else
                        {
                            promotionList.Add(kvp.Key, kvp.Value);
                        }
                    }
                }
                productModel.price = cartModel.list[0].products.price; //新的商品单价
                productModel.amount = cartModel.list[0].products.amount; //商品总价格
                productModel.promotionList = promotionList; //促销列表
                productModel.promotionAmount = cartModel.list[0].products.promotionAmount; //如果商品促销应用了，那么促销的金额
            }
            return productModel;
        }
        /// <summary>
        /// 判断货品上下架状态
        /// </summary>
        /// <param name="productsId">货品序列</param>
        /// <returns></returns>
        public async Task<bool> GetShelfStatus(int productsId)
        {
            return await _dal.GetShelfStatus(productsId);
        }
    }
}