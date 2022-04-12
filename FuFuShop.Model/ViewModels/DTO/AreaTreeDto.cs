

using System.ComponentModel;

namespace FuFuShop.Model.ViewModels.DTO
{
    public class AreaTreeDto
    {
        public int id { get; set; }
        public string title { get; set; }

        public bool isLast { get; set; }

        public int level { get; set; }

        public int parentId { get; set; }

        public List<AreaTreeCheckArr> checkArr { get; set; }

        public List<AreaTreeDto> children { get; set; }
    }

    public class AreaTreeCheckArr
    {
        public string type { get; set; } = "0";
        public string @checked { get; set; } = "0";
    }

    public class PostGetAreaParameters
    {
        [Description("选中节点")] public string ids { get; set; }

        [Description("节点序列")] public int nodeId { get; set; } = 0;

        [Description("是否选中")] public int ischecked { get; set; } = 0;
    }

    /// <summary>
    ///     编辑时默认选中的反馈数据中ids实体
    /// </summary>
    public class PostAreasTreeNode
    {
        public string id { get; set; }
        public string pid { get; set; }
        public string name { get; set; }
        public int ischecked { get; set; }
    }
}