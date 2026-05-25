using System.Text;
using System.Threading.RateLimiting;
using DotNetEnv;
using HotelBooking.API.Middleware;
using HotelBooking.Core.Entities;
using HotelBooking.Core.Helpers.Settings;
using HotelBooking.Core.Interfaces.Repositories;
using HotelBooking.Core.Interfaces.Services;
using HotelBooking.Infrastructure.Data;
using HotelBooking.Infrastructure.Repositories;
using HotelBooking.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.OpenApi;
using AutoMapper;

// Load .env FIRST — before anything else
Env.Load();

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog();

// ── Settings from Environment Variables ──────────────────────────────
var jwtSettings = new JwtSettings
{
    Secret = Environment.GetEnvironmentVariable("JWT_SECRET") ?? "default-256-bit-secret-minimum-32chars!",
    Issuer = Environment.GetEnvironmentVariable("JWT_ISSUER") ?? "HotelBookingAPI",
    Audience = Environment.GetEnvironmentVariable("JWT_AUDIENCE") ?? "HotelBookingClients",
    ExpiryMinutes = int.TryParse(Environment.GetEnvironmentVariable("JWT_EXPIRY_MINUTES"), out var em) ? em : 60,
    RefreshExpiryDays = int.TryParse(Environment.GetEnvironmentVariable("JWT_REFRESH_EXPIRY_DAYS"), out var red) ? red : 7
};

var razorpaySettings = new RazorpaySettings
{
    Enabled = bool.TryParse(Environment.GetEnvironmentVariable("RAZORPAY_ENABLED"), out var rpe) && rpe,
    KeyId = Environment.GetEnvironmentVariable("RAZORPAY_KEY_ID") ?? string.Empty,
    KeySecret = Environment.GetEnvironmentVariable("RAZORPAY_KEY_SECRET") ?? string.Empty,
    Currency = Environment.GetEnvironmentVariable("RAZORPAY_CURRENCY") ?? "INR"
};

var emailSettings = new EmailSettings
{
    Enabled = bool.TryParse(Environment.GetEnvironmentVariable("EMAIL_ENABLED"), out var ee) && ee,
    Host = Environment.GetEnvironmentVariable("EMAIL_HOST") ?? "smtp.gmail.com",
    Port = int.TryParse(Environment.GetEnvironmentVariable("EMAIL_PORT"), out var ep) ? ep : 587,
    Username = Environment.GetEnvironmentVariable("EMAIL_USERNAME") ?? string.Empty,
    Password = Environment.GetEnvironmentVariable("EMAIL_PASSWORD") ?? string.Empty,
    FromName = Environment.GetEnvironmentVariable("EMAIL_FROM_NAME") ?? "Hotel Booking",
    FromAddress = Environment.GetEnvironmentVariable("EMAIL_FROM_ADDRESS") ?? string.Empty
};

// Register settings as singletons
builder.Services.AddSingleton(jwtSettings);
builder.Services.AddSingleton(razorpaySettings);
builder.Services.AddSingleton(emailSettings);

// ── Database ─────────────────────────────────────────────────────────
var connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING")
    ?? "Server=localhost;Database=HotelBookingDb;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=True;";

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

// ── Identity ─────────────────────────────────────────────────────────
builder.Services.AddIdentity<User, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 8;
    options.User.RequireUniqueEmail = true;
    options.SignIn.RequireConfirmedEmail = false;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

// ── JWT Authentication ───────────────────────────────────────────────
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings.Issuer,
        ValidAudience = jwtSettings.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret)),
        ClockSkew = TimeSpan.Zero
    };
});

// ── Rate Limiting ────────────────────────────────────────────────────
builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("global", o =>
    {
        o.Window = TimeSpan.FromMinutes(1);
        o.PermitLimit = 100;
        o.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        o.QueueLimit = 0;
    });

    options.AddFixedWindowLimiter("auth", o =>
    {
        o.Window = TimeSpan.FromMinutes(1);
        o.PermitLimit = 10;
        o.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        o.QueueLimit = 0;
    });

    options.RejectionStatusCode = 429;
});

// ── Repositories ─────────────────────────────────────────────────────
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IHotelRepository, HotelRepository>();
builder.Services.AddScoped<IRoomRepository, RoomRepository>();
builder.Services.AddScoped<IBookingRepository, BookingRepository>();

// ── Services ─────────────────────────────────────────────────────────
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IHotelService, HotelService>();
builder.Services.AddScoped<IRoomService, RoomService>();
builder.Services.AddScoped<IBookingService, BookingService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IPromotionService, PromotionService>();
builder.Services.AddScoped<ILoyaltyService, LoyaltyService>();

// ── AutoMapper ───────────────────────────────────────────────────────
builder.Services.AddAutoMapper(cfg => {}, typeof(MappingProfile));

// ── Controllers & Swagger ────────────────────────────────────────────
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Hotel Booking API",
        Version = "v1",
        Description = "Production-ready Hotel Booking platform API"
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer' followed by a space and your JWT token."
    });

    options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecuritySchemeReference("Bearer"),
            new List<string>()
        }
    });
});

// ── CORS ─────────────────────────────────────────────────────────────
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// ── Middleware Pipeline ──────────────────────────────────────────────
app.UseMiddleware<ExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.UseRateLimiter();
app.MapControllers();

app.Run();
