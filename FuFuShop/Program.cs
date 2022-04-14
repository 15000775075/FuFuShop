
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Essensoft.Paylink.Alipay;
using Essensoft.Paylink.WeChatPay;
using FuFuShop.Common.AppSettings;
using FuFuShop.Common.Auth;
using FuFuShop.Common.Auth.HttpContextUser;
using FuFuShop.Common.AutoFac;
using FuFuShop.Common.Caching;
using FuFuShop.Common.Extensions;
using FuFuShop.Common.Helper;
using FuFuShop.Common.Loging;
using FuFuShop.Filter;
using FuFuShop.Model.ViewModels.Mapping;
using FuFuShop.Tasks.Tasks;
using FuFuShop.WeChat.Options;
using FuFuShop.WeChat.Service.HttpClients;
using FuFuShop.WeChat.Services.HttpClients;
using Hangfire;
using Hangfire.Dashboard.BasicAuthorization;
using Hangfire.Redis;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using SqlSugar;
using SqlSugar.IOC;
using StackExchange.Redis;
using Swashbuckle.AspNetCore.Filters;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

//添加本地路径获取支持
builder.Services.AddSingleton(new AppSettingsHelper(builder.Environment.ContentRootPath));
builder.Services.AddSingleton(new LogLockHelper(builder.Environment.ContentRootPath));
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();


#region Swagger
builder.Services.AddSwaggerGen((s) =>
{
    //遍历出全部的版本，做文档信息展示
    typeof(CustomApiVersion.ApiVersions).GetEnumNames().ToList().ForEach(version =>
    {
        s.SwaggerDoc(version, new OpenApiInfo
        {
            Version = version,
            Title = $"FuFuShop接口文档",
            Description = $"FuFuShopHTTP API " + version,
            Contact = new OpenApiContact { Name = "FuFuShopApi", Email = "", Url = new Uri("http://www.baidu.com") },
        });
        s.OrderActionsBy(o => o.RelativePath);
    });

    try
    {
        //生成API XML文档
        var basePath = AppContext.BaseDirectory;
        var xmlPath = Path.Combine(basePath, "FuFuShop.xml");
        s.IncludeXmlComments(xmlPath);
    }
    catch (Exception ex)
    {
        NLogUtil.WriteFileLog(NLog.LogLevel.Error, LogType.Swagger, "Swagger", "Swagger生成失败，Doc.xml丢失，请检查并拷贝。", ex);
    }

    // 开启加权小锁
    s.OperationFilter<AddResponseHeadersFilter>();
    s.OperationFilter<AppendAuthorizeToSummaryOperationFilter>();

    // 在header中添加token，传递到后台
    s.OperationFilter<SecurityRequirementsOperationFilter>();

    // 必须是 oauth2
    s.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        Description = "JWT授权(数据将在请求头中进行传输) 直接在下框中输入Bearer {token}（注意两者之间是一个空格）\"",
        Name = "Authorization",//jwt默认的参数名称
        In = ParameterLocation.Header,//jwt默认存放Authorization信息的位置(请求头中)
        Type = SecuritySchemeType.ApiKey
    });

});

#endregion

#region   SqlSugar
//注入 ORM
SugarIocServices.AddSqlSugar(new IocConfig()
{
    //数据库连接
    ConnectionString = AppSettingsConstVars.DbSqlConnection,
    //判断数据库类型
    DbType = AppSettingsConstVars.DbDbType == IocDbType.MySql.ToString() ? IocDbType.MySql : IocDbType.SqlServer,
    //是否开启自动关闭数据库连接-//不设成true要手动close
    IsAutoCloseConnection = true,
});

//设置参数
builder.Services.ConfigurationSugar(db =>
{
    db.CurrentConnectionConfig.InitKeyType = InitKeyType.Attribute;

    //执行SQL 错误事件，可监控sql（暂时屏蔽，需要可开启）
    db.Aop.OnLogExecuting = (sql, p) =>
    {
        NLogUtil.WriteFileLog(NLog.LogLevel.Error, LogType.Other, "SqlSugar执行SQL错误事件打印Sql", sql);
    };

    //执行SQL 错误事件
    db.Aop.OnError = (exp) =>
    {
        NLogUtil.WriteFileLog(NLog.LogLevel.Error, LogType.Other, "SqlSugar", "执行SQL错误事件", exp);
    };

    //设置更多连接参数
    //db.CurrentConnectionConfig.XXXX=XXXX
    //db.CurrentConnectionConfig.MoreSetting=new MoreSetting(){}
    //读写分离等都在这儿设置
});
#endregion

#region   Core
builder.Services.AddCors(c =>
{
    if (!AppSettingsConstVars.CorsEnableAllIPs)
    {
        c.AddPolicy(AppSettingsConstVars.CorsPolicyName, policy =>
        {
            policy.WithOrigins(AppSettingsConstVars.CorsIPs.Split(','));
            policy.AllowAnyHeader();//Ensures that the policy allows any header.
            policy.AllowAnyMethod();
            policy.AllowCredentials();
        });
    }
    else
    {
        //允许任意跨域请求
        c.AddPolicy(AppSettingsConstVars.CorsPolicyName, policy =>
        {
            policy.SetIsOriginAllowed((host) => true)
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials();
        });
    }
});
#endregion

// AutoMapper支持
builder.Services.AddAutoMapper(typeof(AutoMapperConfiguration));

// 引入Payment 依赖注入(支付宝支付/微信支付)
builder.Services.AddAlipay();
builder.Services.AddWeChatPay();

// 在 appsettings.json 中 配置选项
builder.Services.Configure<WeChatPayOptions>(builder.Configuration.GetSection("WeChatPay"));
builder.Services.Configure<AlipayOptions>(builder.Configuration.GetSection("Alipay"));

//注册自定义微信接口配置文件
builder.Services.Configure<WeChatOptions>(builder.Configuration.GetSection(nameof(WeChatOptions)));


// 注入工厂 HTTP 客户端
builder.Services.AddHttpClient();
builder.Services.AddSingleton<IWeChatApiHttpClientFactory, WeChatApiHttpClientFactory>();


#region Redis
builder.Services.AddTransient<IRedisOperationRepository, RedisOperationRepository>();

// 配置启动Redis服务，虽然可能影响项目启动速度，但是不能在运行的时候报错，所以是合理的
builder.Services.AddSingleton<ConnectionMultiplexer>(sp =>
{
    //获取连接字符串
    string redisConfiguration = AppSettingsConstVars.RedisConfigConnectionString;

    var configuration = ConfigurationOptions.Parse(redisConfiguration, true);

    configuration.ResolveDns = true;

    return ConnectionMultiplexer.Connect(configuration);
});
#endregion

#region   Authorization
//读取配置文件
var symmetricKeyAsBase64 = AppSettingsConstVars.JwtConfigSecretKey;
var keyByteArray = Encoding.ASCII.GetBytes(symmetricKeyAsBase64);
var signingKey = new SymmetricSecurityKey(keyByteArray);
var issuer = AppSettingsConstVars.JwtConfigIssuer;
var audience = AppSettingsConstVars.JwtConfigAudience;

var signingCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

// 如果要数据库动态绑定，这里先留个空，后边处理器里动态赋值
var permission = new List<PermissionItem>();

// 角色与接口的权限要求参数
var permissionRequirement = new PermissionRequirement(
    "/api/denied",// 拒绝授权的跳转地址（目前无用）
    permission,
    ClaimTypes.Role,//基于角色的授权
    issuer,//发行人
    audience,//听众
    signingCredentials,//签名凭据
    expiration: TimeSpan.FromSeconds(60 * 60 * 24)//接口的过期时间
    );


// 复杂的策略授权
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(Permissions.Name, policy => policy.Requirements.Add(permissionRequirement));
});


// 令牌验证参数
var tokenValidationParameters = new TokenValidationParameters
{
    ValidateIssuerSigningKey = true, //是否验证SecurityKey
    IssuerSigningKey = signingKey, //拿到SecurityKey
    ValidateIssuer = true, //是否验证Issuer
    ValidIssuer = issuer,//发行人
    ValidateAudience = true, //是否验证Audience
    ValidAudience = audience,//订阅人
    ValidateLifetime = true, //是否验证失效时间
    ClockSkew = TimeSpan.FromSeconds(60),
    RequireExpirationTime = true,
};

// core自带官方JWT认证，开启Bearer认证
builder.Services.AddAuthentication(o =>
{
    o.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    o.DefaultChallengeScheme = nameof(ApiResponseHandler);
    o.DefaultForbidScheme = nameof(ApiResponseHandler);
})
 // 添加JwtBearer服务
 .AddJwtBearer(o =>
 {
     o.TokenValidationParameters = tokenValidationParameters;
     o.Events = new JwtBearerEvents
     {
         OnChallenge = context =>
         {
             context.Response.Headers.Add("Token-Error", context.ErrorDescription);
             return Task.CompletedTask;
         },
         OnAuthenticationFailed = context =>
         {
             var token = context.Request.Headers["Authorization"].ObjectToString().Replace("Bearer ", "");
             var jwtToken = (new JwtSecurityTokenHandler()).ReadJwtToken(token);

             if (jwtToken.Issuer != issuer)
             {
                 context.Response.Headers.Add("Token-Error-Iss", "issuer is wrong!");
             }

             if (jwtToken.Audiences.FirstOrDefault() != audience)
             {
                 context.Response.Headers.Add("Token-Error-Aud", "Audience is wrong!");
             }

             // 如果过期，则把<是否过期>添加到，返回头信息中
             if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
             {
                 context.Response.Headers.Add("Token-Expired", "true");
             }
             return Task.CompletedTask;
         }
     };
 })
 .AddScheme<AuthenticationSchemeOptions, ApiResponseHandler>(nameof(ApiResponseHandler), o => { });


// 注入权限处理器
builder.Services.AddScoped<IAuthorizationHandler, PermissionHandler>();
builder.Services.AddSingleton(permissionRequirement);
#endregion
#region     Hangfire

//注册Hangfire定时任务

var configuration = ConfigurationOptions.Parse(AppSettingsConstVars.RedisConfigConnectionString, true);
var db = ConnectionMultiplexer.Connect(configuration).GetDatabase();

builder.Services.AddHangfire(x => x.UseRedisStorage(ConnectionMultiplexer.Connect(configuration), new RedisStorageOptions()
{
    Db = db.Database, //建议根据
    SucceededListSize = 500,//后续列表中的最大可见后台作业，以防止它无限增长。
    DeletedListSize = 500,//删除列表中的最大可见后台作业，以防止其无限增长。
    InvisibilityTimeout = TimeSpan.FromMinutes(30),
}));

builder.Services.AddHangfireServer(options =>
{
    options.Queues = new[] { GlobalEnumVars.HangFireQueuesConfig.@default.ToString(), GlobalEnumVars.HangFireQueuesConfig.apis.ToString(), GlobalEnumVars.HangFireQueuesConfig.web.ToString(), GlobalEnumVars.HangFireQueuesConfig.recurring.ToString() };
    options.ServerTimeout = TimeSpan.FromMinutes(4);
    options.SchedulePollingInterval = TimeSpan.FromSeconds(15);//秒级任务需要配置短点，一般任务可以配置默认时间，默认15秒
    options.ShutdownTimeout = TimeSpan.FromMinutes(5); //超时时间
    options.WorkerCount = Math.Max(Environment.ProcessorCount, 20); //工作线程数，当前允许的最大线程，默认20
});
#endregion


//注册mvc，注册razor引擎视图
builder.Services.AddMvc(options =>
{
    //实体验证
    //options.Filters.Add<GlobalExceptionsFilter>();
    //异常处理
    // options.Filters.Add<GlobalExceptionsFilterForClent>();
    //Swagger剔除不需要加入api展示的列表
    //options.Conventions.Add(new ApiExplorerIgnores());
})
    .AddNewtonsoftJson(p =>
    {
        //数据格式首字母小写 不使用驼峰
        p.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
        //不使用驼峰样式的key
        p.SerializerSettings.ContractResolver = new DefaultContractResolver();
        //忽略循环引用
        p.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
        //设置时间格式（必须使用yyyy/MM/dd格式，因为ios系统不支持2018-03-29格式的时间，只识别2018/03/09这种格式。）
        p.SerializerSettings.DateFormatString = "yyyy/MM/dd HH:mm:ss";
    });


builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddScoped<IHttpContextUser, AspNetUser>();
//确保NLog.config中连接字符串与appsettings.json中同步
NLogUtil.EnsureNlogConfig("NLog.config");
//其他项目启动时需要做的事情
NLogUtil.WriteAll(NLog.LogLevel.Trace, LogType.Web, "接口启动", "接口启动成功");


#region AutoFac
//AutoFac
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
builder.Host.ConfigureContainer<ContainerBuilder>(builder =>
{
    builder.RegisterModule(new AutofacModuleRegister());
    var controllerBaseType = typeof(ControllerBase);
    builder.RegisterAssemblyTypes(typeof(Program).Assembly).Where(t => controllerBaseType.IsAssignableFrom(t) & t != controllerBaseType).PropertiesAutowired();
});
//服务配置中加入AutoFac控制器替换规则。
builder.Services.Replace(ServiceDescriptor.Transient<IControllerActivator, ServiceBasedControllerActivator>());

#endregion
var app = builder.Build();

app.UseSwagger().UseSwaggerUI(c =>
{
    //根据版本名称倒序 遍历展示
    typeof(CustomApiVersion.ApiVersions).GetEnumNames().OrderByDescending(e => e).ToList().ForEach(
        version =>
        {
            c.SwaggerEndpoint($"/swagger/{version}/swagger.json", $"Doc {version}");
        });
    //设置默认跳转到swagger-ui
    c.RoutePrefix = "doc";
    //c.RoutePrefix = string.Empty;
});


app.UseHttpsRedirection();
// 先开启认证
app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

#region Hangfire定时任务

//授权
var filter = new BasicAuthAuthorizationFilter(
    new BasicAuthAuthorizationFilterOptions
    {
        SslRedirect = false,
        // Require secure connection for dashboard
        RequireSsl = false,
        // Case sensitive login checking
        LoginCaseSensitive = false,
        // Users
        Users = new[]
        {
                        new BasicAuthAuthorizationUser
                        {
                            Login = AppSettingsConstVars.HangFireLogin,
                            PasswordClear = AppSettingsConstVars.HangFirePassWord
                        }
        }
    });
var options = new DashboardOptions
{
    AppPath = "/",//返回时跳转的地址
    DisplayStorageConnectionString = false,//是否显示数据库连接信息
    Authorization = new[]
    {
                    filter
                },
    IsReadOnlyFunc = Context =>
    {
        return false;//是否只读面板
    }
};

app.UseHangfireDashboard("/job", options); //可以改变Dashboard的url
HangfireDispose.HangfireService();

#endregion


app.Run("http://*80");
