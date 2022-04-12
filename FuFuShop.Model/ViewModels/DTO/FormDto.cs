
namespace FuFuShop.Model.ViewModels.DTO
{
    public class FormStatisticsDto
    {
        public string day { get; set; }
        public int nums { get; set; }
        public int formId { get; set; }
    }

    public class FormStatisticsViewDto
    {
        public string[] day { get; set; }
        public int[] data { get; set; }
        public int formId { get; set; }
    }
}