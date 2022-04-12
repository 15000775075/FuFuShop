
using FuFuShop.Repository;
using FuFuShop.Common.AppSettings;
using FuFuShop.Common.Helper;
using FuFuShop.Model.Entities;
using FuFuShop.Model.ViewModels.UI;
using FuFuShop.Repository.UnitOfWork;
using FuFuShop.Services.BaseServices;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace FuFuShop.Services
{
    /// <summary>
    /// 消息发送表 接口实现
    /// </summary>
    public class MessageServices : BaseServices<Message>, IMessageServices
    {
        private readonly IMessageRepository _dal;
        private readonly IUnitOfWork _unitOfWork;
        public MessageServices(IUnitOfWork unitOfWork, IMessageRepository dal)
        {
            _dal = dal;
            base.BaseDal = dal;
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// 站内消息
        /// </summary>
        /// <param name="userId">接受者id</param>
        /// <param name="code">模板编码</param>
        /// <param name="parameters">参数</param>
        /// <returns></returns>
        public async Task<WebApiCallBack> Send(int userId, string code, JObject parameters)
        {
            var jm = new WebApiCallBack();

            var content = MessageHelper.GetTemp(code, parameters);
            if (string.IsNullOrEmpty(content))
            {
                jm.msg = GlobalErrorCodeVars.Code10009;
                return jm;
            }

            var msg = new Message
            {
                userId = userId,
                code = code,
                parameters = JsonConvert.SerializeObject(parameters),
                contentBody = content,
                status = false,
                createTime = DateTime.Now
            };

            var bl = await _dal.InsertAsync(msg) > 0;
            jm.status = bl;
            jm.msg = bl ? "站内消息发布成功" : "站内消息发布失败";

            return jm;
        }


        /// <summary>
        /// 消息查看，更新已读状态
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<WebApiCallBack> info(int userId, int id)
        {
            var jm = new WebApiCallBack { status = true };

            var info = await _dal.QueryByClauseAsync(p => p.userId == userId && p.id == id);
            if (info != null)
            {
                await _dal.UpdateAsync(p => new Message() { status = true }, p => p.id == info.id);
            }

            return jm;
        }

        /// <summary>
        /// 判断是否有新消息
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<bool> HasNew(int userId)
        {
            var bl = await _dal.ExistsAsync(p => p.userId == userId && p.status == false);
            return bl;
        }


    }
}
