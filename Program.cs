using System.Reflection;
using Piggyzen.Web.Features.Transaction;
using Piggyzen.Web.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Registrera CategoryMenuService
builder.Services.AddScoped<CategoryMenuService>();
// Register MediatR
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
builder.Configuration.AddJsonFile("appsettings.Development.json", optional: false, reloadOnChange: true);
builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient("Api", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ApiSettings:BaseUrl"]);
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");
});

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
