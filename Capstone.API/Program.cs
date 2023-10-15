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
using static System.Reflection.Metadata.BlobBuilder;

static async Task InitializeDatabase(IApplicationBuilder app)
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
	builder.EntitySet<Ticket>("Tickets");
	builder.EntitySet<TicketComment>("TicketComments");
	builder.EntitySet<TicketHistory>("TicketHistorys");
	builder.EntitySet<TicketType>("TicketTypes");
	builder.EntitySet<TicketStatus>("TicketStatuss");
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

builder.Services.AddScoped<IRoleRepository,RoleRepository>();
builder.Services.AddScoped<IPermissionSchemaRepository,PermissionSchemaRepository>();

builder.Services.AddScoped<IProjectMemberRepository,ProjectMemberRepository>();
builder.Services.AddScoped<IBoardRepository,BoardRepository>();



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

builder.Services.AddHttpContextAccessor();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
var logger = app.Services.GetRequiredService<ILoggerManager>();
app.ConfigureExceptionHandler(logger);

app.UseSwagger();
app.UseSwaggerUI();

app.UseRouting();

app.UseCors("corspolicy");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
