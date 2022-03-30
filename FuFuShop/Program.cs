using CoreCms.Net.WeChat.Service.HttpClients;
using Essensoft.Paylink.Alipay;
using Essensoft.Paylink.WeChatPay;
using FuFuShop.Common.AppSettings;
using FuFuShop.Common.Helper;
using FuFuShop.Common.Loging;
using FuFuShop.Model.ViewModels.Mapping;
using FuFuShop.WeChat.Options;
using FuFuShop.WeChat.Services.HttpClients;
using Hangfire;
using Hangfire.MySql;
using Hangfire.MySql.src;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using SqlSugar;
using SqlSugar.IOC;
using System.Data;

//ȷ��NLog.config�������ַ�����appsettings.json��ͬ��
NLogUtil.EnsureNlogConfig("NLog.config");
//������Ŀ����ʱ��Ҫ��������
NLogUtil.WriteAll(NLog.LogLevel.Trace, LogType.Web, "�ӿ�����", "�ӿ������ɹ�");
var builder = WebApplication.CreateBuilder(args);


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


#region
//ע��Hangfire��ʱ����


builder.Services.AddHangfire(x => x.UseStorage(new MySqlStorage(AppSettingsConstVars.DbSqlConnection, new MySqlStorageOptions
        {
           // TransactionIsolationLevel = IsolationLevel.ReadCommitted, // ������뼶��Ĭ���Ƕ�ȡ���ύ��
            QueuePollInterval = TimeSpan.FromSeconds(15),             //- ��ҵ������ѯ�����Ĭ��ֵΪ15�롣
            //JobExpirationCheckInterval = TimeSpan.FromHours(1),       //- ��ҵ���ڼ������������ڼ�¼����Ĭ��ֵΪ1Сʱ��
            //CountersAggregateInterval = TimeSpan.FromMinutes(5),      //- �ۺϼ������ļ����Ĭ��Ϊ5���ӡ�
           // PrepareSchemaIfNecessary = true,                          //- �������Ϊtrue���򴴽����ݿ��Ĭ����true��
           // DashboardJobListLimit = 500,                            //- �Ǳ����ҵ�б����ơ�Ĭ��ֵΪ50000��
           // TransactionTimeout = TimeSpan.FromMinutes(1),             //- ���׳�ʱ��Ĭ��Ϊ1���ӡ�
            //TablesPrefix = "Hangfire"                                  //- ���ݿ��б��ǰ׺��Ĭ��Ϊnone
        })));


builder.Services.AddHangfireServer(options =>
{
    options.Queues = new[] { GlobalEnumVars.HangFireQueuesConfig.@default.ToString(), GlobalEnumVars.HangFireQueuesConfig.apis.ToString(), GlobalEnumVars.HangFireQueuesConfig.web.ToString(), GlobalEnumVars.HangFireQueuesConfig.recurring.ToString() };
    options.ServerTimeout = TimeSpan.FromMinutes(4);
    options.SchedulePollingInterval = TimeSpan.FromSeconds(15);//�뼶������Ҫ���ö̵㣬һ�������������Ĭ��ʱ�䣬Ĭ��15��
    options.ShutdownTimeout = TimeSpan.FromMinutes(5); //��ʱʱ��
    options.WorkerCount = Math.Max(Environment.ProcessorCount, 20); //�����߳�������ǰ���������̣߳�Ĭ��20
});

#endregion


//ע��mvc��ע��razor������ͼ
builder.Services.AddMvc(options =>
{
    //ʵ����֤
    //options.Filters.Add<RequiredErrorForClent>();
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
                    //p.SerializerSettings.ContractResolver = new DefaultContractResolver();
                    //����ѭ������
                    p.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                    //����ʱ���ʽ������ʹ��yyyy/MM/dd��ʽ����Ϊiosϵͳ��֧��2018-03-29��ʽ��ʱ�䣬ֻʶ��2018/03/09���ָ�ʽ����
                    p.SerializerSettings.DateFormatString = "yyyy/MM/dd HH:mm:ss";
    });
//��ӱ���·����ȡ֧��
builder.Services.AddSingleton(new AppSettingsHelper(builder.Environment.ContentRootPath));
builder.Services.AddSingleton(new LogLockHelper(builder.Environment.ContentRootPath));
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
