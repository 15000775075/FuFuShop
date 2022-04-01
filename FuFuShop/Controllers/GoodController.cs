using AutoMapper;
using CoreCms.Net.IServices;
using FuFuShop.Common.Auth.HttpContextUser;
using FuFuShop.Model.ViewModels.DTO;
using FuFuShop.Model.ViewModels.UI;
using Microsoft.AspNetCore.Mvc;
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


        /// <summary>
        /// 构造函数
        /// </summary>
        public GoodController(IMapper mapper
            , IHttpContextUser user
            , IGoodsCategoryServices goodsCategoryServices
        )
        {
            _mapper = mapper;
            _user = user;
            _goodsCategoryServices = goodsCategoryServices;

        }

        //公共接口====================================================================================================

       
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




    }
}