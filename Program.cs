using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using TameShop.Models;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddRazorPages();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddControllers();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "TameShop API V1");
    });
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication(); 
app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();

app.Run();