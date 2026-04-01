using cinema;
using cinema.Models;
using cinema.Services;
using DemoSession1_WebAPI.Converters;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using cinema.Helpers;

var builder = WebApplication.CreateBuilder(args);

// CORS, Controllers, and Session
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors();
builder.Services.AddControllersWithViews();
builder.Services.AddSession();
builder.Services.AddControllers().AddJsonOptions(opt =>
{
    // Thêm DateTimeConverter
    opt.JsonSerializerOptions.Converters.Add(new DateTimeConverter());
    opt.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    opt.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
});

// Database connection
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrEmpty(connectionString))
    throw new InvalidOperationException("Connection string 'DefaultConnection' is not found.");

var configuredServerVersion = builder.Configuration["Database:ServerVersion"];
if (!Version.TryParse(configuredServerVersion, out var mysqlVersion))
{
    mysqlVersion = new Version(8, 0, 28);
}
var serverVersion = new MySqlServerVersion(mysqlVersion);

// Configure EF Core with Pomelo and MySQL Connector
builder.Services.AddDbContext<MyDbContext>(opts =>
    opts.UseLazyLoadingProxies()
        .UseMySql(
            connectionString,
            serverVersion,
            mySqlOpts => mySqlOpts
                .EnableRetryOnFailure(1, TimeSpan.FromSeconds(2), null)
                .CommandTimeout(60)
        )
);

// Register application services
builder.Services.AddScoped<AccountService, AccountServiceImpl>();
builder.Services.AddScoped<MovieService, MovieServiceImpl>();
builder.Services.AddScoped<ShowTimeService, ShowTimeServiceImpl>();
builder.Services.AddScoped<ComboService, ComboServiceImpl>();
builder.Services.AddScoped<BookingService, BookingServiceImpl>();
builder.Services.AddScoped<PaymentService, PaymentServiceImpl>();
builder.Services.AddScoped<CinemaService, CinemaServiceImpl>();
builder.Services.AddScoped<ChatService, ChatServiceImpl>();
builder.Services.AddScoped<RatingService, RatingServiceImpl>();
builder.Services.AddScoped<RoomService, RoomServiceImpl>();
builder.Services.AddScoped<FollowService, FollowServiceImpl>();
builder.Services.AddScoped<SeatService, SeatServiceImpl>();
builder.Services.AddScoped<SubService, SubServiceImpl>();
builder.Services.AddScoped<MailHelper>();
builder.Services.AddScoped<SMSHelper>();
builder.Services.AddHttpClient<TmdbMovieSyncService>();
// builder.Services.AddHostedService<MyBackgroundService>(); // TẮT email thông báo lịch chiếu
builder.Services.Configure<ZaloPayOptions>(builder.Configuration.GetSection("Payment:ZaloPay"));
builder.Services.AddHttpClient<ZaloPayService>();

// SignalR for realtime seat reservation
builder.Services.AddSignalR();

// Background service to cleanup expired seat reservations
builder.Services.AddHostedService<SeatReservationCleanupService>();
builder.Services.AddHostedService<ShowtimeCleanupService>();
builder.Services.AddHostedService<NowPlayingAutoSyncService>();


// JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });

var app = builder.Build();


app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Cinema API v1");
});
// Middleware pipeline
app.UseCors(c => c
    .AllowAnyHeader()
    .AllowAnyMethod()
    .SetIsOriginAllowed(_ => true)
    .AllowCredentials()
);
app.UseStaticFiles();
app.UseSession();
app.UseWebSockets();
app.UseAuthentication();
app.UseAuthorization();

// WebSocket endpoint
app.Use(async (context, next) =>
{
    if (context.Request.Path == "/ws")
    {
        if (context.WebSockets.IsWebSocketRequest)
        {
            var webSocket = await context.WebSockets.AcceptWebSocketAsync();
            await HandleWebSocketAsync(webSocket);
        }
        else
        {
            context.Response.StatusCode = 400;
        }
    }
    else
    {
        await next();
    }
});

// MVC Route
app.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action}"
);

// SignalR Hub for seat reservation
app.MapHub<SeatHub>("/seatHub");

await app.RunAsync();

static async Task HandleWebSocketAsync(WebSocket webSocket)
{
    var buffer = new byte[1024 * 4];
    WebSocketManager.AddClient(webSocket);
    try
    {
        while (webSocket.State == WebSocketState.Open)
        {
            var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
            Console.WriteLine($"Received message: {message}");
            foreach (var client in WebSocketManager.GetAllClients())
            {
                if (client != webSocket && client.State == WebSocketState.Open)
                {
                    await client.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(message)), WebSocketMessageType.Text, true, CancellationToken.None);
                }
            }
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"WebSocket exception: {ex.Message}");
    }
    finally
    {
        WebSocketManager.RemoveClient(webSocket);
    }
}
