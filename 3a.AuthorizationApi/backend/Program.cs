using Duende.Bff;
using Duende.Bff.Yarp;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
const string entraIdScheme = "EntraIdOpenIDConnect";

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSpaYarp();

builder.Services.AddBff(o => o.ManagementBasePath = "/account")
    .AddServerSideSessions()
    .AddRemoteApis();

builder.Services.AddScoped<IUserService, GloboUserService>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    //options.DefaultChallengeScheme = entraIdScheme; //OpenIdConnectDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
    options.DefaultSignOutScheme = OpenIdConnectDefaults.AuthenticationScheme;

}).AddOpenIdConnect(options =>
{
    options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.Authority = "https://login.microsoftonline.com/a02df413-10fe-4ec3-b64d-0b43d22fdb92/v2.0";
    options.ResponseType = OpenIdConnectResponseType.Code;
    options.UsePkce = true;
    options.Scope.Add(OpenIdConnectScope.OpenIdProfile);
    options.Scope.Add(OpenIdConnectScope.Email);
    // options.Scope.Add("ctry");
    //options.Scope.Add("api://f73a7c30-4824-4baa-84a6-d4b5bcf27b59/Editor.Admin");
    //options.Scope.Add("api://f73a7c30-4824-4baa-84a6-d4b5bcf27b59/HouseAdd");
    options.Scope.Add("api://0aa25e76-0106-411d-af37-512a1cda2527/Houses.Admin");
    
    options.CallbackPath = "/signin-oidc";
    options.SignedOutCallbackPath = "/signout-callback-oidc";
    
    options.ClientId = "f73a7c30-4824-4baa-84a6-d4b5bcf27b59";
    options.ClientSecret = "1Rb8Q~KkJ~bwXlJsyC0w3VDp9I0aGO95VwFlealE";
    options.MapInboundClaims = false;
    //options.TokenValidationParameters.NameClaimType = JwtRegisteredClaimNames.Name;
    //options.TokenValidationParameters.RoleClaimType = "role";
    options.SaveTokens = true;

}).AddCookie(CookieAuthenticationDefaults.AuthenticationScheme);

//builder.Services.AddAuthentication(o =>
//    {
//        o.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
//        o.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
//    })
//    .AddCookie(o =>
//    {
//        o.Cookie.Name = "__Host-spa";
//        o.Cookie.SameSite = SameSiteMode.Strict;
//        o.Events.OnRedirectToLogin = (context) =>
//        {
//            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
//            return Task.CompletedTask;
//        };
//    })
//    .AddOpenIdConnect(options =>
//    {

//        options.Authority = "https://globomanticsidp20241112163928new.azurewebsites.net"; 
//        //options.Authority = "https://localhost:5001";

//        options.ClientId = "angular";
//        //Store in application secrets
//        options.ClientSecret = "49C1A7E1-0C79-4A89-A3D6-A37998FB86B0";
//        options.ResponseType = "code";
//        options.Scope.Add("roles");
//        options.Scope.Add("globoapi");
//        options.Scope.Add("authapi");
//        options.Scope.Add("offline_access");
//        options.SaveTokens = true;
//    });

var app = builder.Build();

app.UseBff();
app.MapBffManagementEndpoints();

app.MapRemoteBffApiEndpoint("/api", "https://globomanticsapi20241113172150new.azurewebsites.net")//;
    .RequireAccessToken();

//app.MapRemoteBffApiEndpoint("/api", "https://localhost:7165")//;
//    .RequireAccessToken();
app.UseSpaYarp();

app.Run();