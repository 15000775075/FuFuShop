

using SqlSugar;

namespace FuFuShop.Model.Entities
{
    /// <summary>
    ///     退货单表
    /// </summary>
    public partial class BillReship
    {
        /// <summary>
        ///     物流名称
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public string logiName { get; set; }


        /// <summary>
        ///     状态中文描述
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public string statusName { get; set; }


        /// <summary>
        ///     退货明细
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public List<BillReshipItem> items { get; set; }


        /// <summary>
        ///     用户昵称
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public string userNickName { get; set; }
    }
}