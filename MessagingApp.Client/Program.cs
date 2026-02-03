using MessagingApp.Client.Components;
using MessagingApp.Client.Security;
using MessagingApp.Client.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;

var builder = WebApplication.CreateBuilder(args);

// Razor / Blazor
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// App services
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<CookieService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<ChatHubClient>();
builder.Services.AddScoped<MessageService>();
builder.Services.AddScoped<GroupMessageService>();
builder.Services.AddScoped<GroupService>();


// HttpClient (cookies enabled)
builder.Services.AddHttpClient("Api", client =>
{
    client.BaseAddress = new Uri("https://localhost:7222");
})
.ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
{
    UseCookies = true,
    CookieContainer = new System.Net.CookieContainer()
});

// In YOUR.Client/Program.cs
builder.Services.AddDevExpressBlazor();


//builder.Services.AddScoped(sp =>
//{
//    var nav = sp.GetRequiredService<NavigationManager>();
//    return new HttpClient { BaseAddress = new Uri(nav.BaseUri) };
//});


// ?? Authentication
builder.Services.AddAuthentication("JWTAuth")
    .AddScheme<CustomOption, JWTAuthenticationHandler>("JWTAuth", options => { });

// ?? Authorization (DEFAULT POLICY)
builder.Services.AddAuthorization(options =>
{
    options.DefaultPolicy = new AuthorizationPolicyBuilder()
        .AddAuthenticationSchemes("JWTAuth")
        .RequireAuthenticatedUser()
        .Build();
});

// Blazor auth state
builder.Services.AddScoped<JWTAuthenticationStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider, JWTAuthenticationStateProvider>();
builder.Services.AddCascadingAuthenticationState();

var app = builder.Build();

// Pipeline
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
