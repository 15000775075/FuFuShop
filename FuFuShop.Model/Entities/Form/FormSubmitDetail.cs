
using SqlSugar;
using System.ComponentModel.DataAnnotations;

namespace FuFuShop.Model.Entities
{
    /// <summary>
    /// 提交表单保存大文本值表
    /// </summary>
    [SugarTable("FuFuShop_FormSubmitDetail", TableDescription = "提交表单保存大文本值表")]
    public partial class FormSubmitDetail
    {
        /// <summary>
        /// 提交表单保存大文本值表
        /// </summary>
        public FormSubmitDetail()
        {
        }

        /// <summary>
        /// 序列
        /// </summary>
        [Display(Name = "序列")]
        [SugarColumn(ColumnDescription = "序列", IsPrimaryKey = true, IsIdentity = true)]
        [Required(ErrorMessage = "请输入{0}")]
        public int id { get; set; }
        /// <summary>
        /// 提交表单id
        /// </summary>
        [Display(Name = "提交表单id")]
        [SugarColumn(ColumnDescription = "提交表单id")]
        [Required(ErrorMessage = "请输入{0}")]
        public int submitId { get; set; }
        /// <summary>
        /// 表单id
        /// </summary>
        [Display(Name = "表单id")]
        [SugarColumn(ColumnDescription = "表单id")]
        [Required(ErrorMessage = "请输入{0}")]
        public int formId { get; set; }
        /// <summary>
        /// 表单项id
        /// </summary>
        [Display(Name = "表单项id")]
        [SugarColumn(ColumnDescription = "表单项id")]
        [Required(ErrorMessage = "请输入{0}")]
        public int formItemId { get; set; }
        /// <summary>
        /// 表单项名称
        /// </summary>
        [Display(Name = "表单项名称")]
        [SugarColumn(ColumnDescription = "表单项名称", IsNullable = true)]
        [StringLength(200, ErrorMessage = "【{0}】不能超过{1}字符长度")]
        public string formItemName { get; set; }
        /// <summary>
        /// 表单项值
        /// </summary>
        [Display(Name = "表单项值")]
        [SugarColumn(ColumnDescription = "表单项值", IsNullable = true)]
        public string formItemValue { get; set; }
    }
}