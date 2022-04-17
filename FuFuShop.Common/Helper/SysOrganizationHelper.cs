using FuFuShop.Model.Entities;
namespace FuFuShop.Common.Helper
{
    /// <summary>
    /// 组织机构帮助类
    /// </summary>
    public static class SysOrganizationHelper
    {

        /// <summary>
        /// 获取组织结构树
        /// </summary>
        /// <param name="list"></param>
        /// <param name="id"></param>
        /// <param name="treeNodes"></param>
        /// <returns></returns>
        public static void GetOrganizeChildIds(List<SysOrganization> list, int id, ref List<int> treeNodes)
        {
            treeNodes.Add(id);
            if (list == null) return;
            List<SysOrganization> sublist;
            sublist = list.Where(t => t.parentId == id).ToList();
            if (!sublist.Any()) return;
            foreach (var item in sublist)
            {
                GetOrganizeChildIds(list, item.id, ref treeNodes);
            }
        }

    }
}
