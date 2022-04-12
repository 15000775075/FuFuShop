


using SqlSugar;

namespace FuFuShop.Model.ViewModels.Basics
{
    /// <summary>
    ///     代码生成器下拉数据列表实体
    /// </summary>
    public class DbTableInfoTree
    {
        public string Name { get; set; }
        public string Label { get; set; }
        public string Description { get; set; }
        public DbObjectType DbObjectType { get; set; }
    }

    /// <summary>
    ///     表名带字段
    /// </summary>
    public class DbTableInfoAndColumns
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public List<DbColumnInfo> columns { get; set; } = null;
        public DbObjectType DbObjectType { get; set; }
    }
}