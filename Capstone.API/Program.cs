using AutoMapper;
using Capstone.API.Extentions;
using Capstone.Common.Jwt;
using Capstone.DataAccess;
using Capstone.DataAccess.Entities;
using Capstone.DataAccess.Repository.Implements;
using Capstone.DataAccess.Repository.Interfaces;
using Capstone.Service.LoggerService;
using Capstone.Service.UserService;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;
using System.Reflection;
using System.Text;
using Capstone.Service.Mapping;
using Capstone.Service.ProjectService;
using Capstone.Service.PermissionSchemaService;
using static System.Reflection.Metadata.BlobBuilder;
using Capstone.Service.TicketService;
using Capstone.Service.IterationService;
using Capstone.API.Extentions.AuthorizeMiddleware;
using Microsoft.AspNetCore.Authorization;
using Capstone.API.Helper;
using Capstone.Service.StatusService;
using Capstone.Service.AttachmentServices;
using Capstone.Service.TicketCommentService;
using Capstone.Service.RoleService;
using Capstone.Service.TaskService;
using Capstone.Service.ProjectMemberService;

static async System.Threading.Tasks.Task InitializeDatabase(IApplicationBuilder app)
{
    using var scope = app.ApplicationServices.GetService<IServiceScopeFactory>()?.CreateScope();
    if (scope != null)
    {
        await scope.ServiceProvider.GetRequiredService<CapstoneContext>().Database.MigrateAsync();
    }
}
static IEdmModel GetEdmModel()
{
    ODataConventionModelBuilder builder = new();
    builder.EntitySet<Attachment>("Attachments");
    builder.EntitySet<Board>("Boards");
    builder.EntitySet<Notification>("Notifications");
    builder.EntitySet<User>("Users");
    builder.EntitySet<Permission>("Permissions");
    builder.EntitySet<Project>("Projects");
    builder.EntitySet<Role>("Roles");
    builder.EntitySet<Capstone.DataAccess.Entities.Task>("Tasks");
    builder.EntitySet<TaskComment>("TaskComments");
    builder.EntitySet<TaskHistory>("TaskHistorys");
    builder.EntitySet<TaskType>("TaskTypes");
    builder.EntitySet<Status>("TaskStatus");
    builder.EntitySet<PriorityLevel>("PriorityLevels");
    builder.EntitySet<ProjectMember>("ProjectMembers");

    return builder.GetEdmModel();
}
var builder = WebApplication.CreateBuilder(args);
var mapperConfig = new MapperConfiguration(mc =>
{
    mc.AddProfile(new MappingProfile());
});
IMapper mapper = mapperConfig.CreateMapper();
builder.Services.AddSingleton(mapper);
var configuration = builder.Configuration;
builder.Services.AddDbContext<CapstoneContext>(opt =>
{
    opt.UseSqlServer(configuration.GetConnectionString("DBConnString"));
});
// Add services to the container.

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddScoped<IProjectRepository, ProjectRepository>();

builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<ISchemaRepository, SchemaRepository>();

builder.Services.AddScoped<IProjectMemberRepository, ProjectMemberRepository>();
builder.Services.AddScoped<IBoardRepository, BoardRepository>();
builder.Services.AddScoped<IPermissionRepository, PermissionRepository>();

builder.Services.AddScoped<ITaskService, TaskService>();
builder.Services.AddScoped<ITicketRepository, TicketRepository>();

builder.Services.AddScoped<ITicketTypeRepository, TicketTypeRepository>();
builder.Services.AddScoped<ITicketHistoryRepository, TicketHistoryRepository>();
builder.Services.AddScoped<ITicketStatusRepository, TicketStatusRepository>();

builder.Services.AddScoped<IInterationRepository, InteratationRepository>();
builder.Services.AddScoped<IIterationService, IterationService>();

builder.Services.AddScoped<IBoardRepository, BoardRepository>();

builder.Services.AddScoped<IPermissionSchemaRepository, PermissionSchemaRepository>();
builder.Services.AddScoped<IPermissionSchemaService, PermissionSchemaService>();

builder.Services.AddScoped<IStatusRepository, StatusRepository>();
builder.Services.AddScoped<IStatusService, StatusService>();

builder.Services.AddScoped<IAttachmentRepository, AttachmentRepository>();
builder.Services.AddScoped<IAttachmentServices, AttachmentServices>();

builder.Services.AddScoped<ITicketCommentRepository, TicketCommentRepository>();
builder.Services.AddScoped<ITicketCommentService, TicketCommentService>();

builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IRoleService, RoleService>();

builder.Services.AddScoped<IProjectMemberRepository, ProjectMemberRepository>();
builder.Services.AddScoped<IProjectMemberService, ProjectMemberService>();

builder.Services.AddScoped<IMailHelper, MailHelper>();

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddSingleton<ILoggerManager, LoggerManager>();
builder.Services.AddControllers().AddOData(opt => opt.AddRouteComponents("odata", GetEdmModel()).Filter().Select().Expand().Count().OrderBy().SetMaxTop(100));
builder.Services.AddControllers()
                .AddFluentValidation(options =>
                {
                    // Validate child properties and root collection elements
                    options.ImplicitlyValidateChildProperties = true;
					options.ImplicitlyValidateRootCollectionElements = true;

                    // Automatic registration of validators in assembly
                    options.RegisterValidatorsFromAssembly(Assembly.GetExecutingAssembly());
                });

builder.Services.AddCors(p => p.AddPolicy("corspolicy", build =>
{
    build.WithOrigins("*").AllowAnyMethod().AllowAnyHeader();
}));

//add authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(
           options =>
           {
               options.RequireHttpsMetadata = false;
               options.SaveToken = true;
               options.TokenValidationParameters = new TokenValidationParameters
               {
                   ValidateIssuer = true,
                   ValidateAudience = true,
                   ValidIssuer = JwtConstant.Issuer,
                   ValidAudience = JwtConstant.Audience,
                   IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtConstant.Key)),
               };
           }
       );
//// Add Google OAuth authentication
//builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//    .AddJwtBearer(options =>
//    {
//        options.Authority = "https://accounts.google.com";
//        options.Audience = configuration["Authentication:Google:ClientId"]; 
//    });

//// Add authorization policies if needed
//builder.Services.AddAuthorization(options =>
//{
//    options.AddPolicy("GoogleDriveAccess", policy =>
//    {
//        policy.RequireAuthenticatedUser();
//        policy.RequireClaim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier");
//        // Add more requirements as needed
//    });
//});

builder.Services.AddHttpContextAccessor();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAuthorization(
    options =>
    {
        options.AddPolicy("CanView", policy =>
        {
            policy.Requirements.Add(new PermissionRequirement(null));
        });
    }
    );
var app = builder.Build();
var logger = app.Services.GetRequiredService<ILoggerManager>();
app.ConfigureExceptionHandler(logger);

app.UseSwagger();
app.UseSwaggerUI();

app.UseRouting();

app.UseCors("corspolicy");

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
