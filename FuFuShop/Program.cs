
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

//��ӱ���·����ȡ֧��
builder.Services.AddSingleton(new AppSettingsHelper(builder.Environment.ContentRootPath));
builder.Services.AddSingleton(new LogLockHelper(builder.Environment.ContentRootPath));
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


#region   SqlSugar
//ע�� ORM
SugarIocServices.AddSqlSugar(new IocConfig()
{
    //���ݿ�����
    ConnectionString = AppSettingsConstVars.DbSqlConnection,
    //�ж����ݿ�����
    DbType = AppSettingsConstVars.DbDbType == IocDbType.MySql.ToString() ? IocDbType.MySql : IocDbType.SqlServer,
    //�Ƿ����Զ��ر����ݿ�����-//�����trueҪ�ֶ�close
    IsAutoCloseConnection = true,
});

//���ò���
builder.Services.ConfigurationSugar(db =>
{
    db.CurrentConnectionConfig.InitKeyType = InitKeyType.Attribute;

    //ִ��SQL �����¼����ɼ��sql����ʱ���Σ���Ҫ�ɿ�����
    db.Aop.OnLogExecuting = (sql, p) =>
    {
        NLogUtil.WriteFileLog(NLog.LogLevel.Error, LogType.Other, "SqlSugarִ��SQL�����¼���ӡSql", sql);
    };

    //ִ��SQL �����¼�
    db.Aop.OnError = (exp) =>
    {
        NLogUtil.WriteFileLog(NLog.LogLevel.Error, LogType.Other, "SqlSugar", "ִ��SQL�����¼�", exp);
    };

    //���ø������Ӳ���
    //db.CurrentConnectionConfig.XXXX=XXXX
    //db.CurrentConnectionConfig.MoreSetting=new MoreSetting(){}
    //��д����ȶ����������
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
        //���������������
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

// AutoMapper֧��
builder.Services.AddAutoMapper(typeof(AutoMapperConfiguration));

// ����Payment ����ע��(֧����֧��/΢��֧��)
builder.Services.AddAlipay();
builder.Services.AddWeChatPay();

// �� appsettings.json �� ����ѡ��
builder.Services.Configure<WeChatPayOptions>(builder.Configuration.GetSection("WeChatPay"));
builder.Services.Configure<AlipayOptions>(builder.Configuration.GetSection("Alipay"));

//ע���Զ���΢�Žӿ������ļ�
builder.Services.Configure<WeChatOptions>(builder.Configuration.GetSection(nameof(WeChatOptions)));


// ע�빤�� HTTP �ͻ���
builder.Services.AddHttpClient();
builder.Services.AddSingleton<IWeChatApiHttpClientFactory, WeChatApiHttpClientFactory>();


#region     Hangfire


//var configuration = ConfigurationOptions.Parse(AppSettingsConstVars.RedisConfigConnectionString, true);
//_redis = ConnectionMultiplexer.Connect(configuration);
//var db = _redis.GetDatabase();

//services.AddHangfire(x => x.UseRedisStorage(_redis, new RedisStorageOptions()
//{
//    Db = db.Database, //�������
//    SucceededListSize = 500,//�����б��е����ɼ���̨��ҵ���Է�ֹ������������
//    DeletedListSize = 500,//ɾ���б��е����ɼ���̨��ҵ���Է�ֹ������������
//    InvisibilityTimeout = TimeSpan.FromMinutes(30),
//}));


//builder.Services.AddHangfireServer(options =>
//{
//    options.Queues = new[] { GlobalEnumVars.HangFireQueuesConfig.@default.ToString(), GlobalEnumVars.HangFireQueuesConfig.apis.ToString(), GlobalEnumVars.HangFireQueuesConfig.web.ToString(), GlobalEnumVars.HangFireQueuesConfig.recurring.ToString() };
//    options.ServerTimeout = TimeSpan.FromMinutes(4);
//    options.SchedulePollingInterval = TimeSpan.FromSeconds(15);//�뼶������Ҫ���ö̵㣬һ�������������Ĭ��ʱ�䣬Ĭ��15��
//    options.ShutdownTimeout = TimeSpan.FromMinutes(5); //��ʱʱ��
//    options.WorkerCount = Math.Max(Environment.ProcessorCount, 20); //�����߳�������ǰ���������̣߳�Ĭ��20
//});

#endregion


//ע��mvc��ע��razor������ͼ
builder.Services.AddMvc(options =>
{
    //ʵ����֤
    options.Filters.Add<GlobalExceptionsFilter>();
    //�쳣����
    // options.Filters.Add<GlobalExceptionsFilterForClent>();
    //Swagger�޳�����Ҫ����apiչʾ���б�
    //options.Conventions.Add(new ApiExplorerIgnores());
})
    .AddNewtonsoftJson(p =>
    {
        //���ݸ�ʽ����ĸСд ��ʹ���շ�
        p.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
        //��ʹ���շ���ʽ��key
        p.SerializerSettings.ContractResolver = new DefaultContractResolver();
        //����ѭ������
        p.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
        //����ʱ���ʽ������ʹ��yyyy/MM/dd��ʽ����Ϊiosϵͳ��֧��2018-03-29��ʽ��ʱ�䣬ֻʶ��2018/03/09���ָ�ʽ����
        p.SerializerSettings.DateFormatString = "yyyy/MM/dd HH:mm:ss";
    });


//ȷ��NLog.config�������ַ�����appsettings.json��ͬ��
NLogUtil.EnsureNlogConfig("NLog.config");
//������Ŀ����ʱ��Ҫ��������
NLogUtil.WriteAll(NLog.LogLevel.Trace, LogType.Web, "�ӿ�����", "�ӿ������ɹ�");


#region AutoFac
//AutoFac
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
builder.Host.ConfigureContainer<ContainerBuilder>(builder =>
{
    builder.RegisterModule(new AutofacModuleRegister());
    var controllerBaseType = typeof(ControllerBase);
    builder.RegisterAssemblyTypes(typeof(Program).Assembly).Where(t => controllerBaseType.IsAssignableFrom(t) & t != controllerBaseType).PropertiesAutowired();
});
//���������м���AutoFac�������滻����
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
