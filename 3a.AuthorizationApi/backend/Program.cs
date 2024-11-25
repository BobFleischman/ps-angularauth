using Duende.Bff;
using Duende.Bff.Yarp;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
// const string entraIdScheme = "EntraIdOpenIDConnect";

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSpaYarp();

builder.Services.AddBff(o => o.ManagementBasePath = "/account")
    .AddServerSideSessions()
    .AddRemoteApis();

// builder.Services.AddScoped<IUserService, GloboUserService>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    //options.DefaultChallengeScheme = entraIdScheme; //OpenIdConnectDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
    options.DefaultSignOutScheme = OpenIdConnectDefaults.AuthenticationScheme;

}).AddOpenIdConnect(options =>
{
    options.ClientId = "f73a7c30-4824-4baa-84a6-d4b5bcf27b59";
    options.ClientSecret = "1Rb8Q~KkJ~bwXlJsyC0w3VDp9I0aGO95VwFlealE";
    options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.Authority = "https://login.microsoftonline.com/a02df413-10fe-4ec3-b64d-0b43d22fdb92/v2.0";
    //options.ResponseType = OpenIdConnectResponseType.Code;
    options.ResponseType = OpenIdConnectResponseType.CodeIdTokenToken;
    options.UsePkce = true;

    options.Scope.Add(OpenIdConnectScope.OpenIdProfile);
    options.Scope.Add(OpenIdConnectScope.Email);

    //options.Scope.Add("api://0aa25e76-0106-411d-af37-512a1cda2527"); // does NOT work    
    options.Scope.Add("api://53369cff-a1d2-4587-a07d-4d6e61a1a395/Weather.Read");
    //options.Scope.Add("api://0aa25e76-0106-411d-af37-512a1cda2527/DataRole.Seeker");
    options.Scope.Add("api://0aa25e76-0106-411d-af37-512a1cda2527/Data.Review");

    options.CallbackPath = "/signin-oidc";
    options.SignedOutCallbackPath = "/signout-callback-oidc";
    
    options.MapInboundClaims = false;
    options.SaveTokens = true;
    // options.GetClaimsFromUserInfoEndpoint = true;

    //options.Events = new OpenIdConnectEvents
    //{
    //    OnAuthorizationCodeReceived = context =>
    //    {
    //        // You can log or manipulate the authorization code here if needed
    //        Console.WriteLine("Authorization Code Received");
    //        Console.WriteLine(context.TokenEndpointRequest.Code);
    //        return Task.CompletedTask;
    //    },
    //    OnTokenResponseReceived = context =>
    //    {
    //        // Handle the token response if needed
    //        Console.WriteLine("Token Response Received");
    //        Console.WriteLine(context.TokenEndpointResponse.AccessToken);
    //        return Task.CompletedTask;
    //    }
    //};

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

app.MapRemoteBffApiEndpoint("/api2", "https://dmdataservice2.azurewebsites.net")//;
    .RequireAccessToken();

app.MapGet("/hello", async (HttpContext httpContext) =>
{
    Console.WriteLine("In Backend Hello");
    //var tkn = await httpContext.GetClientAccessTokenAsync();
    //Console.WriteLine(tkn);
    foreach (var claim in httpContext.User.Claims)
    {
        Console.WriteLine($"{claim.Type}: {claim.Value}");
    }
    return Results.Ok("Backend API Hello");
});

app.UseSpaYarp();

app.Run();