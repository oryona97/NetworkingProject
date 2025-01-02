using eBookStore.Repository;
using eBookStore.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddScoped<UserRepository>((serviceProvider) =>
{
	var configuration = serviceProvider.GetRequiredService<IConfiguration>();
	var logger = serviceProvider.GetRequiredService<ILogger<UserRepository>>();
	return new UserRepository(configuration.GetConnectionString("DefaultConnection"), logger);
});

// Register the background service
builder.Services.AddHostedService<NotificationBackgroundService>();

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
