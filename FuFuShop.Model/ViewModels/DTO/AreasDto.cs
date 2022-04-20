using FuFuShop.Model.Entities.Shop;

namespace FuFuShop.Model.ViewModels.DTO
{
    public class AreasDto
    {
        public string label { get; set; }
        public int value { get; set; }
        public object children { get; set; }
    }

    public class AreasDtoTh
    {
        public string label { get; set; }
        public int value { get; set; }
    }


    /// <summary>
    ///     后端编辑三级下拉实体
    /// </summary>
    public class AreasDtoForAdminEdit
    {
        public Area info { get; set; } = new();
        public List<Area> list { get; set; } = new();
    }
}