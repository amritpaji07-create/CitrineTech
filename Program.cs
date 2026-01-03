using CitrineTechData.DataContext;
using CitrineTechData.DataModel;
using CitrineTechService.Implementation;
using CitrineTechService.Interface;
using CitrineTechTeam.Implementation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<ApplicationDBContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register Interface and Implementation
builder.Services.AddScoped<IBlogService, BlogService>();
builder.Services.AddScoped<ICareerService, CareerService>();
builder.Services.AddScoped<IContactService, ContactService>();
builder.Services.AddScoped<IPartnersService, PartnersService>();
builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddScoped<IServiceService, ServiceService>();
builder.Services.AddScoped<ISolutionService, SolutionService>();
builder.Services.AddScoped<ISubSolutionService, SubSolutionService>();
builder.Services.AddScoped<ISubServiceService, SubServiceService>();
builder.Services.AddScoped<ITeamService, TeamService>();
builder.Services.AddScoped<ITestimonialService, TestimonialService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IPatronsService, PatronsService>();

// Service registrations
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.Cookie.Name = ".MySampleMVCWeb.Session";
    options.Cookie.SecurePolicy = CookieSecurePolicy.None; // For HTTP
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

app.UseAuthorization();

app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
