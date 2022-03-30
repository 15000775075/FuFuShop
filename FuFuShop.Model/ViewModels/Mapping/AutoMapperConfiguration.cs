using AutoMapper;
using FuFuShop.Model.ViewModels.Basics;

namespace FuFuShop.Model.ViewModels.Mapping
{
    /// <summary>
    /// AutoMapper的全局实体映射配置静态类
    /// </summary>
    public class AutoMapperConfiguration : Profile, AutoMapperIProfile
    {
        public AutoMapperConfiguration()
        {
            //CreateMap<Manager, ManagerDTO>().ReverseMap();

            CreateMap<SqlSugar.DbTableInfo, DbTableInfoTree>()
                .AfterMap((from, to, context) =>
                {
                    to.Label = from.Name + "[" + from.Description + "]";
                });


        }
    }
}
