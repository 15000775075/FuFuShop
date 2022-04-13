using FuFuShop.Model.Entities.Shop;
using FuFuShop.Model.ViewModels.DTO;

namespace FuFuShop.Common.Helper
{
    public class AreaHelper
    {
        /// <summary>
        /// 迭代方法
        /// </summary>
        /// <param name="oldNavs"></param>
        /// <param name="parentId"></param>
        /// <returns></returns>
        public static List<AreasDto> GetList(List<Area> oldNavs)
        {
            List<AreasDto> childList = new List<AreasDto>();
            var model1 = oldNavs.Where(p => p.depth == 1).ToList();
            var model2 = oldNavs.Where(p => p.depth == 2).ToList();
            var model3 = oldNavs.Where(p => p.depth == 3).ToList();

            foreach (var item in model1)
            {
                var child = new AreasDto();
                child.value = item.id;
                child.label = item.name;

                var fsChild = new List<AreasDto>();
                var sc = model2.Where(p => p.parentId == item.id).ToList();
                foreach (var sss in sc)
                {
                    var scItem = new AreasDto();
                    scItem.value = sss.id;
                    scItem.label = sss.name;

                    var scChild = new List<AreasDtoTh>();
                    var th = model3.Where(p => p.parentId == sss.id).ToList();
                    foreach (var itsmth in th)
                    {
                        scChild.Add(new AreasDtoTh()
                        {
                            label = itsmth.name,
                            value = itsmth.id
                        });
                    }
                    scItem.children = scChild;
                    fsChild.Add(scItem);
                }

                child.children = fsChild;
                childList.Add(child);
            }
            return childList;
        }


    }
}
