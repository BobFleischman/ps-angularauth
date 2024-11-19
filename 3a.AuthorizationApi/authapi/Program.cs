using Globomantics.Backend.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<UserRepository>();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(o => 
    {
        //o.Authority = "https://globomanticsidp20241112163928new.azurewebsites.net";
        o.Authority = "https://sts.windows.net/a02df413-10fe-4ec3-b64d-0b43d22fdb92";
        //o.Authority = "https://localhost:5001"; 
        // o.Audience = "globoauthapi";
        o.Audience = "api://f73a7c30-4824-4baa-84a6-d4b5bcf27b59";
        o.Audience = "api://0aa25e76-0106-411d-af37-512a1cda2527";
        o.MapInboundClaims = false;
    });
builder.Services.AddAuthorization();

var app = builder.Build();

app.MapGet("/user/authzdata/{applicationId}", [Authorize](HttpContext httpContext, UserRepository repo, 
    ClaimsPrincipal user, int applicationId) =>
{
    Console.WriteLine("Seeking authz data for user: " + user.FindFirstValue("email") + " and applicationId " + applicationId );
    var sub = user.FindFirstValue("email");
    if (sub is null)
        return [];
    var az = repo.GetAuthzData(applicationId, sub);
    foreach (var item in az)
    {
        Console.WriteLine(item.Type + " - " + item.Value);
    }
    return repo.GetAuthzData(applicationId, sub);
});

app.UseAuthentication();
app.UseAuthorization();

app.Run();
