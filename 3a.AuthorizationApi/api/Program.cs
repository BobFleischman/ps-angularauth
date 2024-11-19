using Globomantics.Api.Models;
using Globomantics.Api.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using System.Reflection.Metadata.Ecma335;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.Resource;
using Microsoft.VisualBasic;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<HouseRepository>();
builder.Services.AddSingleton<BidRepository>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(o => 
    {
        o.Authority = "https://sts.windows.net/a02df413-10fe-4ec3-b64d-0b43d22fdb92"; // Bob's Tenant
        //o.Audience = "api://f73a7c30-4824-4baa-84a6-d4b5bcf27b59"; // azure app registration
        o.Audience = "api://0aa25e76-0106-411d-af37-512a1cda2527"; // azure api registration
        //o.Authority = "https://globomanticsidp20241112163928new.azurewebsites.net";
        //o.Authority = "https://localhost:5001"; 
        //o.Audience = "globomantics"; 
    })
    //.AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"))
    ;
builder.Services.AddAuthorization();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

var scopeRequiredByApi = app.Configuration["AzureAd:Scopes"];

app.MapGet("/hello", (HttpContext httpContext) => {
    Console.WriteLine("In Hello");
    Console.WriteLine(httpContext.User);
    return Results.Ok("API Hello");
    });
app.MapGet("/houses", [Authorize](HouseRepository repo) => repo.GetAll());
app.MapGet("/houses/{id:int}", [Authorize](int id, HouseRepository repo) => repo.GetHouse(id));
app.MapPost("/houses", [Authorize](House house, HouseRepository repo) =>
{
    repo.Add(house);
    return Results.Created($"/houses/{house.Id}", house);
});

app.MapGet("/houses/{id:int}/bids", [Authorize](int id, BidRepository repo) => repo.GetBids(id));
app.MapPost("houses/{id:int}/bids", [Authorize](Bid bid, BidRepository repo) =>
{
    repo.Add(bid);
    return Results.Created($"/houses/{bid.HouseId}/bids", bid);
});

app.Run();
