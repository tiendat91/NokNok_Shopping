using EmailService;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.IdentityModel.Tokens;
using MyRazorPage.Models;
using MyRazorPage.Services;
using SignalR;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSignalR();

//add session
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(5);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
{
    options.LoginPath = "/account/siginingoogle";
}).AddGoogle(options =>
{
    options.ClientId = "597575640582-l3u44vh5o6g5u10ikrhcdre559ajgga7.apps.googleusercontent.com";
    options.ClientSecret = "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";
}
);

// Add services to the container.
//Bổ sung service làm việc với các page vào container Kestrel- không gian làm việc của web server
builder.Services.AddRazorPages();

//bổ sung database
builder.Services.AddDbContext<PRN221DBContext>();
//configure email
var emailConfig = builder.Configuration
        .GetSection("EmailConfiguration")
        .Get<EmailConfiguration>();
builder.Services.AddSingleton(emailConfig);
builder.Services.AddScoped<IEmailSender, EmailSender>();

//Confige file submit
builder.Services.Configure<FormOptions>(o =>
{
    o.ValueLengthLimit = int.MaxValue;
    o.MultipartBodyLengthLimit = int.MaxValue;
    o.MemoryBufferThreshold = int.MaxValue;
});


//convert to html string
//builder.Services.AddScoped<IViewRenderService, ViewRenderService>();

builder.Services.AddControllers();

var app = builder.Build();
app.UseSession();
app.UseRouting();
app.UseAuthentication();

app.UseAuthorization();
app.UseEndpoints(endpoints =>
{
    endpoints.MapRazorPages();
});

app.UseStatusCodePagesWithRedirects("/account/pagenotfound");

app.UseStaticFiles(); //add các public file từ wwwroot
app.MapRazorPages(); //ánh xạ razot page
app.MapHub<HubServer>("/hub");

app.Run();
