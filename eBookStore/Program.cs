using Stripe;
using eBookStore.Repository;
using eBookStore.Models;
using eBookStore.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

//stripe configuration settings
builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("Stripe"));
StripeConfiguration.ApiKey = builder.Configuration["Stripe:SecretKey"];

builder.Services.AddScoped<UserRepository>((serviceProvider) =>
{
	var configuration = serviceProvider.GetRequiredService<IConfiguration>();
	var logger = serviceProvider.GetRequiredService<ILogger<UserRepository>>();
	return new UserRepository(configuration.GetConnectionString("DefaultConnection"), logger);
});

// Register the background service
builder.Services.AddHostedService<NotificationBackgroundService>();

// Register the new SaleEndDateCheckerService
builder.Services.AddHostedService<SaleEndDateCheckerService>();

// Add session support
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
	options.IdleTimeout = TimeSpan.FromMinutes(30);
	options.Cookie.HttpOnly = true;
	options.Cookie.IsEssential = true;
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
app.UseSession();
app.UseAuthorization();
app.MapControllerRoute(
	name: "auth",
	pattern: "auth/{action}",
	defaults: new { controller = "Auth", action = "Login" }
);
app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=showLogIn}/{id?}");

app.Run();
