using SqlSugar;
using System.ComponentModel.DataAnnotations;

namespace FuFuShop.Model.Entities.Bill
{
    /// <summary>
    /// 发货单详情表
    /// </summary>
    public partial class BillDeliveryItem
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public BillDeliveryItem()
        {
        }

        /// <summary>
        /// 序列
        /// </summary>
        [Display(Name = "序列")]

        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]

        [Required(ErrorMessage = "请输入{0}")]



        public int id { get; set; }


        /// <summary>
        /// 订单编号
        /// </summary>
        [Display(Name = "订单编号")]


        [StringLength(maximumLength: 20, ErrorMessage = "{0}不能超过{1}字")]


        public string orderId { get; set; }


        /// <summary>
        /// 发货单号 关联bill_delivery.id
        /// </summary>
        [Display(Name = "发货单号 关联bill_delivery.id")]

        [Required(ErrorMessage = "请输入{0}")]
        [StringLength(maximumLength: 20, ErrorMessage = "{0}不能超过{1}字")]


        public string deliveryId { get; set; }


        /// <summary>
        /// 商品ID 关联goods.id
        /// </summary>
        [Display(Name = "商品ID 关联goods.id")]

        [Required(ErrorMessage = "请输入{0}")]



        public int goodsId { get; set; }


        /// <summary>
        /// 货品ID 关联products.id
        /// </summary>
        [Display(Name = "货品ID 关联products.id")]

        [Required(ErrorMessage = "请输入{0}")]



        public int productId { get; set; }


        /// <summary>
        /// 货品编码
        /// </summary>
        [Display(Name = "货品编码")]

        [Required(ErrorMessage = "请输入{0}")]
        [StringLength(maximumLength: 30, ErrorMessage = "{0}不能超过{1}字")]


        public string sn { get; set; }


        /// <summary>
        /// 商品编码
        /// </summary>
        [Display(Name = "商品编码")]

        [Required(ErrorMessage = "请输入{0}")]
        [StringLength(maximumLength: 30, ErrorMessage = "{0}不能超过{1}字")]


        public string bn { get; set; }


        /// <summary>
        /// 商品名称
        /// </summary>
        [Display(Name = "商品名称")]

        [Required(ErrorMessage = "请输入{0}")]
        [StringLength(maximumLength: 200, ErrorMessage = "{0}不能超过{1}字")]


        public string name { get; set; }


        /// <summary>
        /// 发货数量
        /// </summary>
        [Display(Name = "发货数量")]

        [Required(ErrorMessage = "请输入{0}")]



        public int nums { get; set; }


        /// <summary>
        /// 重量
        /// </summary>
        [Display(Name = "重量")]

        [Required(ErrorMessage = "请输入{0}")]



        public decimal weight { get; set; }


        /// <summary>
        /// 货品明细序列号存储
        /// </summary>
        [Display(Name = "货品明细序列号存储")]





        public string addon { get; set; }


    }
}
