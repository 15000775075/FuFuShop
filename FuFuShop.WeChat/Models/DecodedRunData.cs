namespace FuFuShop.WeChat.Models
{
    [Serializable]
    public class DecodedRunData : DecodeEntityBase
    {
        public List<DecodedRunData_StepModel> stepInfoList { get; set; }
    }

    [Serializable]
    public class DecodedRunData_StepModel
    {
        public long timestamp { get; set; }
        public long step { get; set; }
    }
}
