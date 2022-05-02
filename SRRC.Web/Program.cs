using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SRRC.DataLayer.Database;
using SRRC.DomainClasses.Entities.Chat;
using SRRC.Model;
using SRRC.Service.Repository.Authentication;
using SRRC.UOW;
using SRRC.Web.Hubs;
using Microsoft.AspNetCore.Hosting;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSignalR();

builder.Services.AddControllersWithViews();

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddSingleton<ISecurityService, SecurityService>();


builder.Services.AddOptions<BearerTokensOptions>()
    .Bind(builder.Configuration.GetSection("BearerTokens"))
    .Validate(
        bearerTokens =>
        {
            return bearerTokens.AccessTokenExpirationMinutes < bearerTokens.RefreshTokenExpirationMinutes;
        },
        "RefreshTokenExpirationMinutes is less than AccessTokenExpirationMinutes. Obtaining new tokens using the refresh token should happen only if the access token has expired.");
builder.Services.AddOptions<ApiSettings>()
    .Bind(builder.Configuration.GetSection("ApiSettings"));

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddScoped<IAntiForgeryCookieService, AntiForgeryCookieService>();

builder.Services.AddScoped<IUsersService, UsersService>();
builder.Services.AddScoped<IRolesService, RolesService>();
builder.Services.AddSingleton<ISecurityService, SecurityService>();
builder.Services.AddScoped<IDbInitializerService, DbInitializerService>();
builder.Services.AddScoped<ITokenStoreService, TokenStoreService>();
builder.Services.AddScoped<ITokenValidatorService, TokenValidatorService>();
builder.Services.AddScoped<ITokenFactoryService, TokenFactoryService>();


builder.Services.AddDbContext<SRRCDbContext>(options =>
{
    options.UseSqlite(
        builder.Configuration.GetConnectionString("SRRCConnectionString"),
        serverDbContextOptionsBuilder =>
        {
            var minutes = (int)TimeSpan.FromMinutes(3).TotalSeconds;
            serverDbContextOptionsBuilder.CommandTimeout(minutes);
            // serverDbContextOptionsBuilder.EnableRetryOnFailure();
        });
});

// Only needed for custom roles.
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(CustomRoles.Admin, policy => policy.RequireRole(CustomRoles.Admin));
    options.AddPolicy(CustomRoles.User, policy => policy.RequireRole(CustomRoles.User));
    options.AddPolicy(CustomRoles.Editor, policy => policy.RequireRole(CustomRoles.Editor));
    options.AddPolicy(CustomRoles.Employee, policy => policy.RequireRole(CustomRoles.Employee));
});

// Needed for jwt auth.
builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(cfg =>
    {
        cfg.RequireHttpsMetadata = false;
        cfg.SaveToken = true;
        cfg.TokenValidationParameters = new TokenValidationParameters
        {
            ValidIssuer = builder.Configuration["BearerTokens:Issuer"], // site that makes the token
            ValidateIssuer = false, // TODO: change this to avoid forwarding attacks
            ValidAudience = builder.Configuration["BearerTokens:Audience"], // site that consumes the token
            ValidateAudience = false, // TODO: change this to avoid forwarding attacks
            IssuerSigningKey =
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["BearerTokens:Key"])),
            ValidateIssuerSigningKey = true, // verify signature to avoid tampering
            ValidateLifetime = true, // validate the expiration
            ClockSkew = TimeSpan.Zero // tolerance for the expiration date
        };
        cfg.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                var logger = context.HttpContext.RequestServices.GetRequiredService<ILoggerFactory>()
                    .CreateLogger(nameof(JwtBearerEvents));
                logger.LogError("Authentication failed.", context.Exception);
                return Task.CompletedTask;
            },
            OnTokenValidated = context =>
            {
                var tokenValidatorService = context.HttpContext.RequestServices
                    .GetRequiredService<ITokenValidatorService>();
                return tokenValidatorService.ValidateAsync(context);
            },
            OnMessageReceived = context => { return Task.CompletedTask; },
            OnChallenge = context =>
            {
                var logger = context.HttpContext.RequestServices.GetRequiredService<ILoggerFactory>()
                    .CreateLogger(nameof(JwtBearerEvents));
                logger.LogError("OnChallenge error", context.Error, context.ErrorDescription);
                return Task.CompletedTask;
            }
        };
    });


builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy",
        builder => builder
            .WithOrigins(
                "https://localhost:44452") //Note:  The URL must be specified without a trailing slash (/).
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials());
});
var app = builder.Build();
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.MapHub<ChatHub>("/chat");
app.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action=Index}/{id?}");

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapHub<ChatHub>("/chat");
    // endpoints.MapConnectionHandler<ChatHub>("/chat");
});

var scopeFactory = app.Services.GetRequiredService<IServiceScopeFactory>();
using (var scope = scopeFactory.CreateScope())
{
    var dbInitializer = scope.ServiceProvider.GetService<IDbInitializerService>();
    dbInitializer.SeedData();
}


app.MapFallbackToFile("index.html"); ;

app.Run();
