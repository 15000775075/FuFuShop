
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

//��ӱ���·����ȡ֧��
builder.Services.AddSingleton(new AppSettingsHelper(builder.Environment.ContentRootPath));
builder.Services.AddSingleton(new LogLockHelper(builder.Environment.ContentRootPath));
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();


#region Swagger
builder.Services.AddSwaggerGen((s) =>
{
    //������ȫ���İ汾�����ĵ���Ϣչʾ
    typeof(CustomApiVersion.ApiVersions).GetEnumNames().ToList().ForEach(version =>
    {
        s.SwaggerDoc(version, new OpenApiInfo
        {
            Version = version,
            Title = $"FuFuShop�ӿ��ĵ�",
            Description = $"FuFuShopHTTP API " + version,
            Contact = new OpenApiContact { Name = "FuFuShopApi", Email = "", Url = new Uri("http://www.baidu.com") },
        });
        s.OrderActionsBy(o => o.RelativePath);
    });

    try
    {
        //����API XML�ĵ�
        var basePath = AppContext.BaseDirectory;
        var xmlPath = Path.Combine(basePath, "FuFuShop.xml");
        s.IncludeXmlComments(xmlPath);
    }
    catch (Exception ex)
    {
        NLogUtil.WriteFileLog(NLog.LogLevel.Error, LogType.Swagger, "Swagger", "Swagger����ʧ�ܣ�Doc.xml��ʧ�����鲢������", ex);
    }

    // ������ȨС��
    s.OperationFilter<AddResponseHeadersFilter>();
    s.OperationFilter<AppendAuthorizeToSummaryOperationFilter>();

    // ��header�����token�����ݵ���̨
    s.OperationFilter<SecurityRequirementsOperationFilter>();

    // ������ oauth2
    s.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        Description = "JWT��Ȩ(���ݽ�������ͷ�н��д���) ֱ�����¿�������Bearer {token}��ע������֮����һ���ո�\"",
        Name = "Authorization",//jwtĬ�ϵĲ�������
        In = ParameterLocation.Header,//jwtĬ�ϴ��Authorization��Ϣ��λ��(����ͷ��)
        Type = SecuritySchemeType.ApiKey
    });

});

#endregion

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


#region Redis
builder.Services.AddTransient<IRedisOperationRepository, RedisOperationRepository>();

// ��������Redis������Ȼ����Ӱ����Ŀ�����ٶȣ����ǲ��������е�ʱ�򱨴������Ǻ����
builder.Services.AddSingleton<ConnectionMultiplexer>(sp =>
{
    //��ȡ�����ַ���
    string redisConfiguration = AppSettingsConstVars.RedisConfigConnectionString;

    var configuration = ConfigurationOptions.Parse(redisConfiguration, true);

    configuration.ResolveDns = true;

    return ConnectionMultiplexer.Connect(configuration);
});
#endregion

#region   Authorization
//��ȡ�����ļ�
var symmetricKeyAsBase64 = AppSettingsConstVars.JwtConfigSecretKey;
var keyByteArray = Encoding.ASCII.GetBytes(symmetricKeyAsBase64);
var signingKey = new SymmetricSecurityKey(keyByteArray);
var issuer = AppSettingsConstVars.JwtConfigIssuer;
var audience = AppSettingsConstVars.JwtConfigAudience;

var signingCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

// ���Ҫ���ݿ⶯̬�󶨣������������գ���ߴ������ﶯ̬��ֵ
var permission = new List<PermissionItem>();

// ��ɫ��ӿڵ�Ȩ��Ҫ�����
var permissionRequirement = new PermissionRequirement(
    "/api/denied",// �ܾ���Ȩ����ת��ַ��Ŀǰ���ã�
    permission,
    ClaimTypes.Role,//���ڽ�ɫ����Ȩ
    issuer,//������
    audience,//����
    signingCredentials,//ǩ��ƾ��
    expiration: TimeSpan.FromSeconds(60 * 60 * 24)//�ӿڵĹ���ʱ��
    );


// ���ӵĲ�����Ȩ
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(Permissions.Name, policy => policy.Requirements.Add(permissionRequirement));
});


// ������֤����
var tokenValidationParameters = new TokenValidationParameters
{
    ValidateIssuerSigningKey = true, //�Ƿ���֤SecurityKey
    IssuerSigningKey = signingKey, //�õ�SecurityKey
    ValidateIssuer = true, //�Ƿ���֤Issuer
    ValidIssuer = issuer,//������
    ValidateAudience = true, //�Ƿ���֤Audience
    ValidAudience = audience,//������
    ValidateLifetime = true, //�Ƿ���֤ʧЧʱ��
    ClockSkew = TimeSpan.FromSeconds(60),
    RequireExpirationTime = true,
};

// core�Դ��ٷ�JWT��֤������Bearer��֤
builder.Services.AddAuthentication(o =>
{
    o.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    o.DefaultChallengeScheme = nameof(ApiResponseHandler);
    o.DefaultForbidScheme = nameof(ApiResponseHandler);
})
 // ���JwtBearer����
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

             // ������ڣ����<�Ƿ����>��ӵ�������ͷ��Ϣ��
             if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
             {
                 context.Response.Headers.Add("Token-Expired", "true");
             }
             return Task.CompletedTask;
         }
     };
 })
 .AddScheme<AuthenticationSchemeOptions, ApiResponseHandler>(nameof(ApiResponseHandler), o => { });


// ע��Ȩ�޴�����
builder.Services.AddScoped<IAuthorizationHandler, PermissionHandler>();
builder.Services.AddSingleton(permissionRequirement);
#endregion
#region     Hangfire

//ע��Hangfire��ʱ����

var configuration = ConfigurationOptions.Parse(AppSettingsConstVars.RedisConfigConnectionString, true);
var db = ConnectionMultiplexer.Connect(configuration).GetDatabase();

builder.Services.AddHangfire(x => x.UseRedisStorage(ConnectionMultiplexer.Connect(configuration), new RedisStorageOptions()
{
    Db = db.Database, //�������
    SucceededListSize = 500,//�����б��е����ɼ���̨��ҵ���Է�ֹ������������
    DeletedListSize = 500,//ɾ���б��е����ɼ���̨��ҵ���Է�ֹ������������
    InvisibilityTimeout = TimeSpan.FromMinutes(30),
}));

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
    //options.Filters.Add<GlobalExceptionsFilter>();
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


builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddScoped<IHttpContextUser, AspNetUser>();
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

app.UseSwagger().UseSwaggerUI(c =>
{
    //���ݰ汾���Ƶ��� ����չʾ
    typeof(CustomApiVersion.ApiVersions).GetEnumNames().OrderByDescending(e => e).ToList().ForEach(
        version =>
        {
            c.SwaggerEndpoint($"/swagger/{version}/swagger.json", $"Doc {version}");
        });
    //����Ĭ����ת��swagger-ui
    c.RoutePrefix = "doc";
    //c.RoutePrefix = string.Empty;
});


app.UseHttpsRedirection();
// �ȿ�����֤
app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

#region Hangfire��ʱ����

//��Ȩ
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
    AppPath = "/",//����ʱ��ת�ĵ�ַ
    DisplayStorageConnectionString = false,//�Ƿ���ʾ���ݿ�������Ϣ
    Authorization = new[]
    {
                    filter
                },
    IsReadOnlyFunc = Context =>
    {
        return false;//�Ƿ�ֻ�����
    }
};

app.UseHangfireDashboard("/job", options); //���Ըı�Dashboard��url
HangfireDispose.HangfireService();

#endregion


app.Run("http://*80");
