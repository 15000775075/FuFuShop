using SqlSugar;
using System.ComponentModel.DataAnnotations;

namespace FuFuShop.Model.Entities
{
    /// <summary>
    ///     商品类型扩展
    /// </summary>
    public partial class Goods
    {
        /// <summary>
        ///     货品编码
        /// </summary>
        [Display(Name = "货品编码")]
        [SugarColumn(IsIgnore = true)]
        public string sn { get; set; }

        /// <summary>
        ///     销售价
        /// </summary>
        [Display(Name = "销售价")]
        [SugarColumn(IsIgnore = true)]
        public decimal price { get; set; } = 0;

        /// <summary>
        ///     成本价
        /// </summary>
        [Display(Name = "成本价")]
        [SugarColumn(IsIgnore = true)]
        public decimal costprice { get; set; } = 0;

        /// <summary>
        ///     市场价
        /// </summary>
        [Display(Name = "市场价")]
        [SugarColumn(IsIgnore = true)]
        public decimal mktprice { get; set; } = 0;


        /// <summary>
        ///     库存
        /// </summary>
        [Display(Name = "库存")]
        [SugarColumn(IsIgnore = true)]
        public int stock { get; set; } = 0;

        /// <summary>
        ///     冻结库存
        /// </summary>
        [Display(Name = "冻结库存")]
        [SugarColumn(IsIgnore = true)]
        public int freezeStock { get; set; } = 0;

        /// <summary>
        ///     重量
        /// </summary>
        [Display(Name = "重量")]
        [SugarColumn(IsIgnore = true)]
        public decimal weight { get; set; } = 0;


        /// <summary>
        ///     图集
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public string[] album { get; set; }

        /// <summary>
        ///     品牌数据
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public Brand brand { get; set; }

        /// <summary>
        ///     关联参数
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public Products product { get; set; } = new();


        /// <summary>
        ///     是否收藏
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public bool isFav { get; set; } = false;



        /// <summary>
        ///     拼团价格
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public decimal pinTuanPrice { get; set; } = 0;


        /// <summary>
        ///     拼团记录数量
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public int pinTuanRecordNums { get; set; } = 0;

        /// <summary>
        ///     拼团总单数
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public int buyPinTuanCount { get; set; } = 0;

        /// <summary>
        ///     团购秒杀促销总单数
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public int buyPromotionCount { get; set; } = 0;

        /// <summary>
        ///     标签列表
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public List<Label> labels { get; set; } = new();

        /// <summary>
        ///     所属团购秒杀
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public int groupId { get; set; }

        [SugarColumn(IsIgnore = true)] public int groupType { get; set; }

        [SugarColumn(IsIgnore = true)] public bool groupStatus { get; set; }

        [SugarColumn(IsIgnore = true)] public DateTime groupTime { get; set; }

        [SugarColumn(IsIgnore = true)] public DateTime groupStartTime { get; set; }

        [SugarColumn(IsIgnore = true)] public DateTime groupEndTime { get; set; }

        [SugarColumn(IsIgnore = true)] public int groupTimestamp { get; set; }
    }
}