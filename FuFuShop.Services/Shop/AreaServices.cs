using FuFuShop.Common.AppSettings;
using FuFuShop.Model.Entities.Shop;
using FuFuShop.Model.ViewModels.Basics;
using FuFuShop.Model.ViewModels.DTO;
using FuFuShop.Model.ViewModels.UI;
using FuFuShop.Repository.Shop;
using FuFuShop.Repository.UnitOfWork;
using FuFuShop.Services.BaseServices;
using SqlSugar;
using System.Linq.Expressions;

namespace FuFuShop.Services.Shop
{
    public class AreaServices : BaseServices<Area>, IAreaServices
    {
        private readonly IAreaRepository _dal;
        private readonly IUnitOfWork _unitOfWork;
        public AreaServices(IUnitOfWork unitOfWork, IAreaRepository dal)
        {
            _dal = dal;
            BaseDal = dal;
            _unitOfWork = unitOfWork;
        }

        #region 实现重写增删改查操作==========================================================

        /// <summary>
        /// 重写异步插入方法
        /// </summary>
        /// <param name="entity">实体数据</param>
        /// <returns></returns>
        public new async Task<AdminUiCallBack> InsertAsync(Area entity)
        {
            return await _dal.InsertAsync(entity);
        }

        /// <summary>
        /// 重写异步更新方法方法
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public new async Task<AdminUiCallBack> UpdateAsync(Area entity)
        {
            return await _dal.UpdateAsync(entity);
        }

        /// <summary>
        /// 重写异步更新方法方法
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public new async Task<AdminUiCallBack> UpdateAsync(List<Area> entity)
        {
            return await _dal.UpdateAsync(entity);
        }

        /// <summary>
        /// 重写删除指定ID的数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public new async Task<AdminUiCallBack> DeleteByIdAsync(object id)
        {
            return await _dal.DeleteByIdAsync(id);
        }

        /// <summary>
        /// 重写删除指定ID集合的数据(批量删除)
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public new async Task<AdminUiCallBack> DeleteByIdsAsync(int[] ids)
        {
            return await _dal.DeleteByIdsAsync(ids);
        }

        #endregion

        #region 获取缓存的所有数据==========================================================

        /// <summary>
        /// 获取缓存的所有数据
        /// </summary>
        /// <returns></returns>
        public async Task<List<Area>> GetCaChe()
        {
            return await _dal.GetCaChe();
        }

        /// <summary>
        ///     更新cache
        /// </summary>
        public async Task<List<Area>> UpdateCaChe()
        {
            return await _dal.UpdateCaChe();
        }

        #endregion

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
        public new async Task<IPageList<Area>> QueryPageAsync(Expression<Func<Area, bool>> predicate,
            Expression<Func<Area, object>> orderByExpression, OrderByType orderByType, int pageIndex = 1,
            int pageSize = 20, bool blUseNoLock = false)
        {
            return await _dal.QueryPageAsync(predicate, orderByExpression, orderByType, pageIndex, pageSize, blUseNoLock);
        }
        #endregion


        #region 获取所有省市区信息
        /// <summary>
        /// 获取所有省市区信息
        /// </summary>
        /// <returns></returns>
        public async Task<List<AreaTreeDto>> GetTreeArea(List<PostAreasTreeNode> checkedAreases, int parentId = 0,
            int currentChecked = 0)
        {
            var list = await UpdateCaChe();
            var areaTrees = GetTrees(list, parentId, checkedAreases, currentChecked);
            return areaTrees;
        }

        /// <summary>
        /// 迭代方法
        /// </summary>
        /// <param name="oldNavs"></param>
        /// <param name="parentId"></param>
        /// <returns></returns>
        private static List<AreaTreeDto> GetTrees(List<Area> allDatas, int parentId, List<PostAreasTreeNode> checkedAreases, int currentChecked = 0)
        {
            List<AreaTreeDto> childTree = new List<AreaTreeDto>();
            var model = allDatas.Where(p => p.parentId == parentId).ToList();
            foreach (var item in model)
            {
                var areaTree = new AreaTreeDto();
                areaTree.id = item.id;
                areaTree.title = item.name;
                areaTree.isLast = allDatas.Exists(p => p.parentId == item.id) == false;
                areaTree.level = (int)item.depth;
                areaTree.parentId = (int)item.parentId;

                var isChecked = "0";
                var idStr = item.id.ToString();
                var parentIdStr = item.parentId.ToString();
                //判断是否选中的数据
                if (checkedAreases != null)
                {
                    var areaModel = checkedAreases.Find(p => p.id == idStr);
                    if (areaModel != null)
                    {
                        isChecked = areaModel.ischecked.ToString();
                    }
                    var parentModel = checkedAreases.Find(p => p.id == parentIdStr);
                    if (parentModel != null && parentModel.ischecked == 1)
                    {
                        isChecked = "1";
                    }
                }
                //当前父节点是1，下面肯定都是1
                if (currentChecked == 1)
                {
                    isChecked = "1";
                }

                var checkArr = new AreaTreeCheckArr()
                {
                    @checked = isChecked,
                    type = "0"
                };
                areaTree.checkArr = new List<AreaTreeCheckArr>();
                areaTree.checkArr.Add(checkArr);

                childTree.Add(areaTree);
                areaTree.children = GetTrees(allDatas, item.id, checkedAreases, currentChecked);
            }
            return childTree;
        }


        #endregion

        #region 组装地区数据
        /// <summary>
        /// 组装地区数据
        /// </summary>
        public List<AreaTreeDto> resolve2(List<Area> allDatas, int parentId, List<PostAreasTreeNode> checkedAreases, int currentChecked = 0)
        {
            var areaTreeList = new List<AreaTreeDto>();
            var nowList = allDatas.Where(p => p.parentId == parentId).ToList();
            foreach (var item in nowList)
            {
                var isChecked = "0";
                var idStr = item.id.ToString();
                var parentIdStr = item.parentId.ToString();

                //判断是否选中的数据
                if (checkedAreases != null)
                {
                    var model = checkedAreases.Find(p => p.id == idStr);
                    if (model != null)
                    {
                        isChecked = model.ischecked.ToString();
                    }
                    var parentModel = checkedAreases.Find(p => p.id == parentIdStr);
                    if (parentModel != null && parentModel.ischecked == 1)
                    {
                        isChecked = "1";
                    }
                }
                //当前父节点是1，下面肯定都是1
                if (currentChecked == 1)
                {
                    isChecked = "1";
                }

                var isLast = false;
                var isChild = allDatas.Exists(p => p.parentId == item.id);
                if (!isChild)
                {
                    isLast = true;
                }

                var areaTree = new AreaTreeDto();
                areaTree.id = item.id;
                areaTree.title = item.name;
                areaTree.isLast = isLast;
                areaTree.level = (int)item.depth;
                areaTree.parentId = (int)item.parentId;
                var checkArr = new AreaTreeCheckArr()
                {
                    @checked = isChecked,
                    type = "0"
                };
                areaTree.checkArr = new List<AreaTreeCheckArr>();
                areaTree.checkArr.Add(checkArr);
                areaTreeList.Add(areaTree);
            }
            return areaTreeList;
        }

        #endregion

        #region 根据areaId获取三级区域名称

        /// <summary>
        /// 根据areaId获取三级区域名称
        /// </summary>
        /// <param name="areaId"></param>
        /// <param name="cacheAreas"></param>
        /// <returns></returns>
        public async Task<WebApiCallBack> GetAreaFullName(int areaId, List<Area> cacheAreas = null)
        {
            var jm = new WebApiCallBack { status = true };

            cacheAreas ??= await GetCaChe();
            var arr = GetArea(cacheAreas, areaId);
            var str = string.Empty;
            if (arr.Any())
            {
                arr.Reverse();//倒序
                arr.ForEach(p => { str += p.name + " "; });
            }
            jm.data = str;

            return jm;
        }
        #endregion

        #region 根据id来返回省市区信息，如果没有查到，就返回省的列表
        /// <summary>
        /// 根据id来返回省市区信息，如果没有查到，就返回省的列表
        /// </summary>
        public List<Area> GetArea(List<Area> cacheAreas, int id = 0)
        {
            var outAreas = new List<Area>();

            if (id > 0)
            {
                GetParentArea(cacheAreas, id, outAreas);
            }
            return outAreas;
        }

        #endregion

        #region 递归取得父节点信息

        /// <summary>
        /// 递归取得父节点信息
        /// </summary>
        /// <param name=""></param>
        /// <param name="id"></param>
        /// <param name="outAreas"></param>
        /// <param name="cacheAreas"></param>
        /// <returns></returns>
        private void GetParentArea(List<Area> cacheAreas, int id, List<Area> outAreas)
        {
            //获取当前级别
            var model = cacheAreas.FirstOrDefault(p => p.id == id);
            if (model != null)
            {
                if (outAreas.All(p => p.id != model.id)) outAreas.Add(model);
                //获取父级
                var parentModel = cacheAreas.Find(p => p.id == model.parentId);
                if (parentModel != null)
                {
                    if (outAreas.All(p => p.id != parentModel.id)) outAreas.Add(parentModel);
                    if (parentModel.parentId != 0)
                    {
                        //上面还有节点
                        var parentParentModel = cacheAreas.Find(p => p.id == parentModel.parentId);
                        if (parentParentModel != null && outAreas.All(p => p.id != parentParentModel.id)) outAreas.Add(parentParentModel);
                    }
                }
            }
        }
        #endregion

        #region 获取最终地区ID
        /// <summary>
        /// 获取最终地区ID
        /// </summary>
        /// <param name="provinceName">省</param>
        /// <param name="cityName">市</param>
        /// <param name="countyName">县</param>
        /// <param name="postalCode">邮编</param>
        /// <returns></returns>
        public async Task<WebApiCallBack> GetAreaId(string provinceName, string cityName, string countyName, string postalCode)
        {
            var jm = new WebApiCallBack();

            if (string.IsNullOrEmpty(provinceName) || string.IsNullOrEmpty(cityName) || string.IsNullOrEmpty(countyName))
            {
                jm.msg = "请提交合法参数信息";
                return jm;
            }

            jm = new WebApiCallBack
            {
                status = true,
                data = await GetThreeAreaId(provinceName, cityName, countyName, postalCode)
            };
            return jm;
        }

        /// <summary>
        /// 获取最终地区ID
        /// </summary>
        /// <param name="provinceName">省</param>
        /// <param name="cityName">市</param>
        /// <param name="countyName">县</param>
        /// <param name="postalCode">邮编</param>
        /// <returns></returns>
        public async Task<int> GetThreeAreaId(string provinceName, string cityName, string countyName, string postalCode)
        {
            var areaData = await GetCaChe();
            var id = 0;
            var countyList = areaData.Where(p => p.depth == (int)GlobalEnumVars.AreaDepth.County && p.name == countyName).ToList();
            if (countyList.Any())
            {
                if (countyList.Count > 1)
                {
                    var cityModel = areaData.Find(p => p.depth == (int)GlobalEnumVars.AreaDepth.City && p.name == cityName);
                    if (cityModel != null)
                    {
                        //foreach (var item in countyList)
                        //{
                        //    if (item.parentId == cityModel.id)
                        //    {
                        //        id = item.id;
                        //    }
                        //}
                        var result = countyList.Find(p => p.parentId == cityModel.id);
                        return result?.id ?? 0;
                    }
                }
                else
                {
                    id = countyList[0].id;
                }
            }
            else
            {
                //var cityModel = areaData.Find(p => p.depth == (int)GlobalEnumVars.AreaDepth.City && p.name == cityName);
                //if (cityModel != null)
                //{
                //    //创建区域
                //    var area = new Area();
                //    area.depth = (int)GlobalEnumVars.AreaDepth.County;
                //    area.name = countyName;
                //    area.postalCode = postalCode;
                //    area.parentId = cityModel.id;
                //    area.sort = 100;
                //    id = await base.InsertAsync(area);

                //    await UpdateCaChe();

                //}
                //else
                //{
                //    var province = areaData.Find(p => p.depth == (int)GlobalEnumVars.AreaDepth.Province && p.name == provinceName);
                //    if (province != null)
                //    {
                //        //创建城市
                //        var areaCity = new Area();
                //        areaCity.depth = (int)GlobalEnumVars.AreaDepth.City;
                //        areaCity.name = cityName;
                //        //areaCity.postalCode = postalCode;
                //        areaCity.parentId = province.id;
                //        areaCity.sort = 100;
                //        var cityId = await base.InsertAsync(areaCity);

                //        //创建区域
                //        var areaCounty = new Area();
                //        areaCounty.depth = (int)GlobalEnumVars.AreaDepth.County;
                //        areaCounty.name = countyName;
                //        areaCounty.postalCode = postalCode;
                //        areaCounty.parentId = cityId;
                //        areaCounty.sort = 100;
                //        id = await base.InsertAsync(areaCounty);
                //    }
                //    else
                //    {
                //        //创建省
                //        var areaProvince = new Area();
                //        areaProvince.depth = (int)GlobalEnumVars.AreaDepth.Province;
                //        areaProvince.name = cityName;
                //        //areaCity.postalCode = postalCode;
                //        areaProvince.parentId = (int)GlobalEnumVars.AreaDepth.ProvinceParentId;
                //        areaProvince.sort = 100;
                //        var provinceId = await base.InsertAsync(areaProvince);

                //        //创建城市
                //        var areaCity = new Area();
                //        areaCity.depth = (int)GlobalEnumVars.AreaDepth.City;
                //        areaCity.name = cityName;
                //        //areaCity.postalCode = postalCode;
                //        areaCity.parentId = provinceId;
                //        areaCity.sort = 100;
                //        var cityId = await base.InsertAsync(areaCity);

                //        //创建区域
                //        var areaCounty = new Area();
                //        areaCounty.depth = (int)GlobalEnumVars.AreaDepth.County;
                //        areaCounty.name = countyName;
                //        areaCounty.postalCode = postalCode;
                //        areaCounty.parentId = cityId;
                //        areaCounty.sort = 100;
                //        id = await base.InsertAsync(areaCounty);
                //    }
                //    await UpdateCaChe();
                //}
            }

            return id;
        }
        #endregion

    }
}
