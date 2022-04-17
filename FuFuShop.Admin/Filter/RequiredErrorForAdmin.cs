using FuFuShop.Model.ViewModels.Basics;
using FuFuShop.Model.ViewModels.UI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;

namespace FuFuShop.Admin.Filter
{
    /// <summary>
    /// 请求验证错误处理
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true)]
    public class RequiredErrorForAdmin : ResultFilterAttribute
    {
        public override void OnResultExecuting(ResultExecutingContext actionContext)
        {
            //base.OnResultExecuting(actionContext);
            var modelState = actionContext.ModelState;
            List<ErrorView> errors = new List<ErrorView>();
            if (!modelState.IsValid)
            {
                var baseResult = new AdminUiCallBack()
                {
                    code = 1,
                    msg = "请提交必要的参数",
                };
                foreach (var key in modelState.Keys)
                {
                    var state = modelState[key];
                    if (state.Errors.Any())
                    {
                        ErrorView errorView = new ErrorView();
                        errorView.ErrorName = key;
                        errorView.Error = state.Errors.First().ErrorMessage;
                        errors.Add(errorView);
                        baseResult.msg += errorView.ErrorName + "-" + errorView.Error;
                    }
                }
                baseResult.data = errors;
                actionContext.Result = new ContentResult
                {
                    Content = JsonConvert.SerializeObject(baseResult),
                    ContentType = "application/json"
                };
            }
        }
    }
}
