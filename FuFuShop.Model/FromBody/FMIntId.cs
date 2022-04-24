using System.ComponentModel.DataAnnotations;

namespace FuFuShop.Model.FromBody
{
    public class FMIntId
    {
        /// <summary>
        ///     序列
        /// </summary>
        [Display(Name = "序列")]
        [Required(ErrorMessage = "请输入要提交的序列参数")]
        public int id { get; set; }

        public object data { get; set; } = "";
    }

    public class FMIntIdByListIntData
    {
        public int id { get; set; }
        public List<int> data { get; set; } = new List<int>();
    }


    public class FMArrayIntIds
    {
        public int[] id { get; set; }
        public object data { get; set; } = "";
    }

    public class FMStringId
    {
        public string id { get; set; }
        public object data { get; set; } = "";
    }

    public class FMArrayStringIds
    {
        public string[] id { get; set; }
        public object data { get; set; } = "";
    }


    public class FMGuidId
    {
        public Guid id { get; set; }
        public object data { get; set; } = "";
    }


    public class FMArrayGuidIds
    {
        public Guid[] id { get; set; }
        public object data { get; set; } = "";
    }
}