using CCSystem.Presentation.Configurations;
using CCSystem.Presentation.Helpers;
using CCSystem.Presentation.Services;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

//add cookies
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
{
    options.LoginPath = "/Authentications/Login";
    options.AccessDeniedPath = "/Authentications/Access-Denied";
    options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
    options.SlidingExpiration = true;
});

// Add services to the container.
builder.Services.AddRazorPages();

//register ApiEndpoints
builder.Services.AddSingleton<ApiEndpoints>();

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<CookieHelper>();
builder.Services.AddTransient<BearerTokenHandler>();


// 🔄 Register HttpClient with IHttpClientFactory
builder.Services.AddHttpClient("AuthenticationAPI", (serviceProvider, client) =>
{
    var apiEndpoints = serviceProvider.GetRequiredService<ApiEndpoints>();
    client.BaseAddress = new Uri(apiEndpoints.BaseUrl);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
    client.Timeout = TimeSpan.FromSeconds(30);
}).AddHttpMessageHandler<BearerTokenHandler>();

builder.Services.AddScoped<AuthenticationService>();
builder.Services.AddScoped<ServiceService>();

builder.Services.AddHttpClient<AccountService>("AccountAPI", (serviceProvider, client) =>
{
    var apiEndpoints = serviceProvider.GetRequiredService<ApiEndpoints>();
    client.BaseAddress = new Uri(apiEndpoints.BaseUrl);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
    client.Timeout = TimeSpan.FromSeconds(30);
}).AddHttpMessageHandler<BearerTokenHandler>();
builder.Services.AddScoped<AccountService>();




var app = builder.Build();

// Configure the HTTP request pipelidotnne.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.MapRazorPages();

app.UseAuthentication();
app.UseAuthorization();

app.Run();
