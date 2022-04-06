using FuFuShop.Common.AppSettings;
using FuFuShop.Common.Caching;
using FuFuShop.Model.Entities;
using FuFuShop.Services;
using FuFuShop.WeChat.Options;
using FuFuShop.WeChat.Services.HttpClients;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SKIT.FlurlHttpClient.Wechat.Api;
using SKIT.FlurlHttpClient.Wechat.Api.Models;

namespace FuFuShop.Tasks.Tasks
{
    /// <summary>
    /// 定时刷新获取微信AccessToken
    /// </summary>
    public class RefreshWeChatAccessTokenJob
    {
        private readonly ISysTaskLogServices _taskLogServices;

        private readonly IRedisOperationRepository _redisOperationRepository;

        private readonly WeChatOptions _weChatOptions;
        private readonly IWeChatApiHttpClientFactory _weChatApiHttpClientFactory;
        private readonly IWeChatAccessTokenServices _weChatAccessTokenServices;


        public RefreshWeChatAccessTokenJob(IRedisOperationRepository redisOperationRepository, ISysTaskLogServices taskLogServices, IOptions<WeChatOptions> weChatOptions, IWeChatApiHttpClientFactory weChatApiHttpClientFactory, IWeChatAccessTokenServices weChatAccessTokenServices)
        {
            _redisOperationRepository = redisOperationRepository;
            _taskLogServices = taskLogServices;
            _weChatApiHttpClientFactory = weChatApiHttpClientFactory;
            _weChatAccessTokenServices = weChatAccessTokenServices;
            _weChatOptions = weChatOptions.Value;
        }

        public async Task Execute()
        {
            try
            {
                //微信公众号刷新
                if (!string.IsNullOrEmpty(_weChatOptions.WeiXinAppId) && !string.IsNullOrEmpty(_weChatOptions.WeiXinAppSecret))
                {
                    var entity = await _weChatAccessTokenServices.QueryByClauseAsync(p => p.appId == _weChatOptions.WeiXinAppId && p.appType == (int)GlobalEnumVars.AccessTokenEnum.WeiXinAccessToken);
                    if (entity == null || entity.expireTimestamp <= DateTimeOffset.Now.ToUnixTimeSeconds())
                    {
                        var client = _weChatApiHttpClientFactory.CreateWeXinClient();
                        var request = new CgibinTokenRequest();
                        var response = await client.ExecuteCgibinTokenAsync(request);
                        if (!response.IsSuccessful())
                        {
                            //插入日志
                            var log = new SysTaskLog
                            {
                                createTime = DateTime.Now,
                                isSuccess = false,
                                name = "定时刷新获取微信AccessToken",
                                parameters = $"刷新 AppId 为 {_weChatOptions.WeiXinAppId} 微信 AccessToken 失败（状态码：{response.RawStatus}，错误代码：{response.ErrorCode}，错误描述：{response.ErrorMessage}）。"
                            };
                            await _taskLogServices.InsertAsync(log);
                        }
                        else
                        {
                            // 提前十分钟过期，以便于系统能及时刷新，防止因在过期临界点时出现问题
                            long nextExpireTimestamp = DateTimeOffset.Now.AddSeconds(response.ExpiresIn).AddMinutes(-10).ToUnixTimeSeconds();

                            if (entity == null)
                            {
                                entity = new WeChatAccessToken();

                                entity.appId = _weChatOptions.WeiXinAppId;
                                entity.accessToken = response.AccessToken;
                                entity.appType = (int)GlobalEnumVars.AccessTokenEnum.WeiXinAccessToken;
                                entity.expireTimestamp = nextExpireTimestamp;
                                entity.createTimestamp = DateTimeOffset.Now.ToUnixTimeSeconds();
                                entity.updateTimestamp = entity.createTimestamp;

                                entity.id = await _weChatAccessTokenServices.InsertAsync(entity);

                            }
                            else
                            {
                                entity.accessToken = response.AccessToken;
                                entity.expireTimestamp = nextExpireTimestamp;
                                entity.updateTimestamp = DateTimeOffset.Now.ToUnixTimeSeconds();
                                await _weChatAccessTokenServices.UpdateAsync(entity);
                            }
                            await _redisOperationRepository.Set(GlobalEnumVars.AccessTokenEnum.WeiXinAccessToken.ToString(), entity, TimeSpan.FromMinutes(120));

                            //插入日志
                            var model = new SysTaskLog
                            {
                                createTime = DateTime.Now,
                                isSuccess = true,
                                name = "定时刷新获取微信AccessToken",
                                parameters = JsonConvert.SerializeObject(entity)
                            };
                            await _taskLogServices.InsertAsync(model);
                        }
                    }
                    else
                    {
                        //插入日志
                        var model = new SysTaskLog
                        {
                            createTime = DateTime.Now,
                            isSuccess = true,
                            name = "定时刷新获取微信AccessToken",
                            parameters = "无需刷新AccessToken,AccessToken 未过期"
                        };
                        await _taskLogServices.InsertAsync(model);
                    }
                }
                //微信小程序也刷新
                if (!string.IsNullOrEmpty(_weChatOptions.WxOpenAppId) && !string.IsNullOrEmpty(_weChatOptions.WxOpenAppSecret))
                {
                    var entity = await _weChatAccessTokenServices.QueryByClauseAsync(p => p.appId == _weChatOptions.WxOpenAppId && p.appType == (int)GlobalEnumVars.AccessTokenEnum.WxOpenAccessToken);
                    if (entity == null || entity.expireTimestamp <= DateTimeOffset.Now.ToUnixTimeSeconds())
                    {
                        var client = _weChatApiHttpClientFactory.CreateWxOpenClient();

                        var request = new CgibinTokenRequest();
                        var response = await client.ExecuteCgibinTokenAsync(request);
                        if (response.IsSuccessful())
                        {
                            // 提前十分钟过期，以便于系统能及时刷新，防止因在过期临界点时出现问题
                            long nextExpireTimestamp = DateTimeOffset.Now.AddSeconds(response.ExpiresIn).AddMinutes(-10).ToUnixTimeSeconds();

                            if (entity == null)
                            {
                                entity = new WeChatAccessToken();

                                entity.appId = _weChatOptions.WxOpenAppId;
                                entity.accessToken = response.AccessToken;
                                entity.appType = (int)GlobalEnumVars.AccessTokenEnum.WxOpenAccessToken;
                                entity.expireTimestamp = nextExpireTimestamp;
                                entity.createTimestamp = DateTimeOffset.Now.ToUnixTimeSeconds();
                                entity.updateTimestamp = entity.createTimestamp;

                                await _weChatAccessTokenServices.InsertAsync(entity);
                            }
                            else
                            {
                                entity.accessToken = response.AccessToken;
                                entity.expireTimestamp = nextExpireTimestamp;
                                entity.updateTimestamp = DateTimeOffset.Now.ToUnixTimeSeconds();
                                await _weChatAccessTokenServices.UpdateAsync(entity);
                            }

                            await _redisOperationRepository.Set(
                                GlobalEnumVars.AccessTokenEnum.WxOpenAccessToken.ToString(), entity,
                                TimeSpan.FromMinutes(120));

                            //插入日志
                            var model = new SysTaskLog
                            {
                                createTime = DateTime.Now,
                                isSuccess = true,
                                name = "定时刷新获取微信AccessToken",
                                parameters = JsonConvert.SerializeObject(entity)
                            };
                            await _taskLogServices.InsertAsync(model);
                        }
                        else
                        {
                            //插入日志
                            var log = new SysTaskLog
                            {
                                createTime = DateTime.Now,
                                isSuccess = false,
                                name = "定时刷新获取微信AccessToken",
                                parameters = $"刷新 AppId 为 {_weChatOptions.WeiXinAppId} 微信 AccessToken 失败（状态码：{response.RawStatus}，错误代码：{response.ErrorCode}，错误描述：{response.ErrorMessage}）。"
                            };
                            await _taskLogServices.InsertAsync(log);
                        }
                    }
                    else
                    {
                        //插入日志
                        var model = new SysTaskLog
                        {
                            createTime = DateTime.Now,
                            isSuccess = true,
                            name = "定时刷新获取微信AccessToken",
                            parameters = "无需刷新AccessToken,AccessToken 未过期"
                        };
                        await _taskLogServices.InsertAsync(model);
                    }
                }
            }
            catch (Exception ex)
            {
                //插入日志
                var model = new SysTaskLog
                {
                    createTime = DateTime.Now,
                    isSuccess = false,
                    name = "定时刷新获取微信AccessToken",
                    parameters = JsonConvert.SerializeObject(ex)
                };
                await _taskLogServices.InsertAsync(model);
            }
        }
    }
}
