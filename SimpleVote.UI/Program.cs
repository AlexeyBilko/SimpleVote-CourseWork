using System.Net;
using LeadSub.Models;
using ServiceLayer.Extensions;

var builder = WebApplication.CreateBuilder(args);

var emailConfig = builder.Configuration
    .GetSection("EmailConfiguration")
    .Get<EmailConfiguration>();
builder.Services.AddSingleton(emailConfig);

// Add services to the container.
builder.Services.AddControllersWithViews();
string str = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddAppDbContext(str);
builder.Services.AddRepositoryDependencies();
builder.Services.AddServicesDependencies();

string identityConnection = builder.Configuration.GetConnectionString("IdentityConnection");
builder.Services.ConfigureIdentityOptions(identityConnection);

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options => {

    //options.IdleTimeout = TimeSpan.FromSeconds(40);

});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseCors(x => x
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader());

//app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSession();

app.UseStatusCodePages(context => {
    var response = context.HttpContext.Response;
    if (response.StatusCode == (int)HttpStatusCode.Unauthorized ||
        response.StatusCode == (int)HttpStatusCode.Forbidden)
        response.Redirect("/Account/Login");
    if (response.StatusCode == 404)
        response.Redirect("/error");
    if (response.StatusCode == 500)
        response.Redirect("/Home/Index");
    return Task.CompletedTask;
});

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
