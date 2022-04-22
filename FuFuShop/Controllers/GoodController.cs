using AutoMapper;
using FuFuShop.Common.AppSettings;
using FuFuShop.Common.Auth.HttpContextUser;
using FuFuShop.Common.Extensions;
using FuFuShop.Model.Entities;
using FuFuShop.Model.FromBody;
using FuFuShop.Model.ViewModels.DTO;
using FuFuShop.Model.ViewModels.UI;
using FuFuShop.Services;
using FuFuShop.Services.Good;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SqlSugar;

namespace FuFuShop.Controllers
{
    /// <summary>
    /// 商品相关接口处理
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class GoodController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IHttpContextUser _user;
        private readonly IGoodsCategoryServices _goodsCategoryServices;
        private readonly IGoodsServices _goodsServices;
        private readonly IBrandServices _brandServices;
        private readonly IProductsServices _productsServices;
        private readonly IGoodsCommentServices _goodsCommentServices;
        private readonly IGoodsParamsServices _goodsParamsServices;

        /// <summary>
        /// 构造函数
        /// </summary>
        public GoodController(IMapper mapper
            , IHttpContextUser user
            , IGoodsCategoryServices goodsCategoryServices,
            IGoodsServices goodsServices,
            IBrandServices brandServices,
            IProductsServices productsServices,
            IGoodsCommentServices goodsCommentServices ,
             IGoodsParamsServices goodsParamsServices
        )
        {
            _mapper = mapper;
            _user = user;
            _goodsCategoryServices = goodsCategoryServices;
            _goodsServices = goodsServices;
            _brandServices = brandServices;
            _productsServices = productsServices;
            _goodsCommentServices = goodsCommentServices;
            _goodsParamsServices = goodsParamsServices;

        }

        /// <summary>
        /// 获取所有商品分类栏目数据
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<WebApiCallBack> GetAllCategories()
        {
            var jm = new WebApiCallBack() { status = true };

            var data = await _goodsCategoryServices.QueryListByClauseAsync(p => p.isShow == true, p => p.sort,
                OrderByType.Asc);
            var wxGoodCategoryDto = new List<WxGoodCategoryDto>();

            var parents = data.Where(p => p.parentId == 0).ToList();
            if (parents.Any())
            {
                parents.ForEach(p =>
                {
                    var model = new WxGoodCategoryDto();
                    model.id = p.id;
                    model.name = p.name;
                    model.imageUrl = !string.IsNullOrEmpty(p.imageUrl) ? p.imageUrl : "/static/images/common/empty.png";
                    model.sort = p.sort;

                    var childs = data.Where(p => p.parentId == model.id).ToList();
                    if (childs.Any())
                    {
                        var childsList = new List<WxGoodCategoryChild>();
                        childs.ForEach(o =>
                        {
                            childsList.Add(new WxGoodCategoryChild()
                            {
                                id = o.id,
                                imageUrl = !string.IsNullOrEmpty(o.imageUrl) ? o.imageUrl : "/static/images/common/empty.png",
                                name = o.name,
                                sort = o.sort
                            });
                        });
                        model.child = childsList;
                    }
                    wxGoodCategoryDto.Add(model);
                });
            }
            jm.status = true;
            jm.data = wxGoodCategoryDto;

            return jm;
        }

        /// <summary>
        /// 根据查询条件获取分页数据
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<WebApiCallBack> GetGoodsPageList([FromBody] FMPageByWhereOrder entity)
        {
            var jm = new WebApiCallBack();

            var where = PredicateBuilder.True<Goods>();
            where = where.And(p => p.isDel == false);
            where = where.And(p => p.isMarketable == true);

            var className = string.Empty;
            if (!string.IsNullOrWhiteSpace(entity.where))
            {
                var obj = JsonConvert.DeserializeAnonymousType(entity.where, new
                {
                    priceFrom = "",
                    priceTo = "",
                    catId = "",
                    brandId = "",
                    labelId = "",
                    searchName = "",
                });

                if (!string.IsNullOrWhiteSpace(obj.priceFrom))
                {
                    var priceF = obj.priceFrom.ObjectToDouble(0);
                    if (priceF >= 0)
                    {
                        var f = Convert.ToDecimal(priceF);
                        where = where.And(p => p.price >= f);
                    }
                }
                if (!string.IsNullOrWhiteSpace(obj.priceTo))
                {
                    var priceT = obj.priceTo.ObjectToDouble(0);
                    if (priceT > 0)
                    {
                        var f = Convert.ToDecimal(priceT);
                        where = where.And(p => p.price <= f);
                    }
                }
                if (!string.IsNullOrWhiteSpace(obj.catId))
                {
                    var catId = obj.catId.ObjectToInt(0);
                    if (catId > 0)
                    {
                        var category = await _goodsCategoryServices.QueryByIdAsync(catId);
                        if (category != null)
                        {
                            className = category.name;
                        }

                        var childs = await _goodsCategoryServices.QueryListByClauseAsync(p => p.parentId == catId);
                        if (childs.Any())
                        {
                            var ids = childs.Select(p => p.id).ToList();
                            where = where.And(p => ids.Contains(p.goodsCategoryId) || p.goodsCategoryId == catId);
                        }
                        else
                        {
                            where = where.And(p => p.goodsCategoryId == catId);
                        }
                    }
                }
                if (!string.IsNullOrWhiteSpace(obj.brandId))
                {
                    var brandId = obj.brandId.ObjectToInt(0);
                    if (brandId > 0)
                    {
                        where = where.And(p => p.brandId == brandId);
                    }
                }
                if (!string.IsNullOrWhiteSpace(obj.labelId))
                {
                    where = where.And(p => (',' + p.labelIds.Trim(',') + ',').Contains(',' + obj.labelId.Trim(',') + ','));
                }
                if (!string.IsNullOrWhiteSpace(obj.searchName))
                {
                    where = where.And(p => p.name.Contains(obj.searchName));
                }
            }

            var orderBy = " isRecommend desc,isHot desc";
            if (!string.IsNullOrWhiteSpace(entity.order))
            {
                orderBy += "," + entity.order;
            }


            //获取数据
            var list = await _goodsServices.QueryPageAsync(where, orderBy, entity.page, entity.limit, false);
            if (list.Any())
            {
                foreach (var goods in list)
                {
                    goods.images = !string.IsNullOrEmpty(goods.images) ? goods.images.Split(",")[0] : "/static/images/common/empty.png";
                }
            }

            //获取品牌
            var brands = await _brandServices.QueryListByClauseAsync(p => p.isShow == true, p => p.sort, OrderByType.Desc);


            //返回数据
            jm.status = true;
            jm.data = new
            {
                list,
                className,
                entity.page,
                list.TotalCount,
                list.TotalPages,
                entity.limit,
                entity.where,
                entity.order,
                brands
            };
            jm.msg = "数据调用成功!";

            return jm;
        }

        /// <summary>
        /// 获取商品详情
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<WebApiCallBack> GetDetial([FromBody] FMIntId entity)
        {
            var jm = new WebApiCallBack();

            var userId = 0;
            if (_user != null)
            {
                userId = _user.ID;
            }

            var model = await _goodsServices.GetGoodsDetial(entity.id, userId, false);
            if (model == null)
            {
                jm.msg = "商品获取失败";
                return jm;
            }

            jm.status = true;
            jm.msg = "获取商品详情成功";
            jm.data = model;
            jm.methodDescription = JsonConvert.SerializeObject(_user);

            return jm;
        }
        /// <summary>
        /// 获取随机推荐商品
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<WebApiCallBack> GetGoodsRecommendList([FromBody] FMIntId entity)
        {
            if (entity.id <= 0)
            {
                entity.id = 10;
            }

            var bl = entity.data.ObjectToBool();

            var jm = new WebApiCallBack()
            {
                status = true,
                code = 0,
                msg = "获取成功",
                data = await _goodsServices.GetGoodsRecommendList(entity.id, bl)
            };
            return jm;
        }


        #region 获取单个货品信息======================================================================
        /// <summary>
        /// 获取单个货品信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<WebApiCallBack> GetProductInfo([FromBody] FMGetProductInfo entity)
        {
            var jm = new WebApiCallBack();

            var userId = 0;
            if (_user != null)
            {
                userId = _user.ID;
            }

            bool bl = entity.type == "pinTuan" || entity.type == "group";

            var getProductInfo = await _productsServices.GetProductInfo(entity.id, bl, userId, entity.type, entity.groupId);
            if (getProductInfo == null)
            {
                jm.msg = "获取单个货品失败";
                return jm;
            }

            jm.status = true;
            jm.msg = "获取单个货品成功";
            jm.data = getProductInfo;

            return jm;
        }

        #endregion

        #region 获取商品评价列表分页数据======================================================================
        /// <summary>
        /// 获取商品评价列表分页数据
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<WebApiCallBack> GetGoodsComment([FromBody] FMPageByIntId entity)
        {
            var jm = new WebApiCallBack();

            //获取数据
            var list = await _goodsCommentServices.QueryPageAsync(p => p.goodsId == entity.id && p.isDisplay == true, p => p.createTime, OrderByType.Desc, entity.page, entity.limit);

            if (list.Any())
            {
                foreach (var item in list)
                {
                    item.imagesArr = !string.IsNullOrEmpty(item.images) ? item.images.Split(",") : null;
                }
            }

            jm.status = true;
            jm.msg = "获取评论成功";
            jm.data = new
            {
                list,
                commentsCount = list.TotalCount,
                totalPages = list.TotalPages
            };

            return jm;
        }
        #endregion
        #region 获取商品参数======================================================================
        /// <summary>
        /// 获取单个商品参数
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<WebApiCallBack> GetGoodsParams([FromBody] FMIntId entity)
        {
            var jm = new WebApiCallBack();

            //获取数据
            var goods = await _goodsServices.QueryByIdAsync(entity.id);
            if (goods == null)
            {
                jm.msg = GlobalConstVars.DataisNo;
                return jm;
            }
            var list = new List<WxNameValueDto>();
            var goodsParams = await _goodsParamsServices.QueryAsync();

            if (!string.IsNullOrEmpty(goods.parameters))
            {
                var arrItem = goods.parameters.Split("|");
                foreach (var item in arrItem)
                {
                    if (!item.Contains(":")) continue;

                    var childArr = item.Split(":");
                    if (childArr.Length == 2)
                    {
                        var paramsId = Convert.ToInt32(childArr[0]);
                        var paramsModel = goodsParams.First(p => p.id == paramsId);
                        if (paramsModel != null)
                        {
                            list.Add(new WxNameValueDto()
                            {
                                name = paramsModel.name,
                                value = childArr[1]
                            });
                        }
                    }
                }
            }
            jm.status = true;
            jm.msg = "获取商品参数成功";
            jm.data = list;

            return jm;
        }
        #endregion


    }
}