
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Essensoft.Paylink.Alipay;
using Essensoft.Paylink.WeChatPay;
using FuFuShop.Common.AppSettings;
using FuFuShop.Common.AutoFac;
using FuFuShop.Common.Helper;
using FuFuShop.Common.Loging;
using FuFuShop.Filter;
using FuFuShop.Model.ViewModels.Mapping;
using FuFuShop.WeChat.Options;
using FuFuShop.WeChat.Service.HttpClients;
using FuFuShop.WeChat.Services.HttpClients;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using SqlSugar;
using SqlSugar.IOC;



var builder = WebApplication.CreateBuilder(args);

//添加本地路径获取支持
builder.Services.AddSingleton(new AppSettingsHelper(builder.Environment.ContentRootPath));
builder.Services.AddSingleton(new LogLockHelper(builder.Environment.ContentRootPath));
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


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


#region     Hangfire


//var configuration = ConfigurationOptions.Parse(AppSettingsConstVars.RedisConfigConnectionString, true);
//_redis = ConnectionMultiplexer.Connect(configuration);
//var db = _redis.GetDatabase();

//services.AddHangfire(x => x.UseRedisStorage(_redis, new RedisStorageOptions()
//{
//    Db = db.Database, //建议根据
//    SucceededListSize = 500,//后续列表中的最大可见后台作业，以防止它无限增长。
//    DeletedListSize = 500,//删除列表中的最大可见后台作业，以防止其无限增长。
//    InvisibilityTimeout = TimeSpan.FromMinutes(30),
//}));


//builder.Services.AddHangfireServer(options =>
//{
//    options.Queues = new[] { GlobalEnumVars.HangFireQueuesConfig.@default.ToString(), GlobalEnumVars.HangFireQueuesConfig.apis.ToString(), GlobalEnumVars.HangFireQueuesConfig.web.ToString(), GlobalEnumVars.HangFireQueuesConfig.recurring.ToString() };
//    options.ServerTimeout = TimeSpan.FromMinutes(4);
//    options.SchedulePollingInterval = TimeSpan.FromSeconds(15);//秒级任务需要配置短点，一般任务可以配置默认时间，默认15秒
//    options.ShutdownTimeout = TimeSpan.FromMinutes(5); //超时时间
//    options.WorkerCount = Math.Max(Environment.ProcessorCount, 20); //工作线程数，当前允许的最大线程，默认20
//});

#endregion


//注册mvc，注册razor引擎视图
builder.Services.AddMvc(options =>
{
    //实体验证
    options.Filters.Add<GlobalExceptionsFilter>();
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

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
