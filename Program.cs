using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProjectPrn222.Models;
using ProjectPrn222.Service.Iterface;
using ProjectPrn222.Service.Implement;
using ProjectPrn222.Hubs;

var builder = WebApplication.CreateBuilder(args);

//them dich vu SignalR
builder.Services.AddSignalR();

//DI
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<ICloudinaryService, CloudinaryService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<IVourcherService, VourcherService>();

//session
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDistributedMemoryCache();

// Cấu hình Identity với ApplicationUser
builder.Services.AddIdentity<ApplicationUser, ApplicationRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();


//setting mật khẩu cho Identity
builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireDigit = false;                // Không yêu cầu mật khẩu phải chứa số
    options.Password.RequireLowercase = false;            // Không yêu cầu chữ cái thường
    options.Password.RequireUppercase = false;            // Không yêu cầu chữ cái hoa
    options.Password.RequireNonAlphanumeric = false;      // Không yêu cầu ký tự đặc biệt (ví dụ: @, #, !, ...)
    options.Password.RequiredLength = 6;                  // Yêu cầu mật khẩu có độ dài tối thiểu là 6 ký tự
});

builder.Services.ConfigureApplicationCookie(options =>
{
	options.LoginPath = "/Auth/Login";
	options.AccessDeniedPath = "/Home/PageNotFound";
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    await SeedRolesAndAdminAsync(scope.ServiceProvider);
}

static async Task SeedRolesAndAdminAsync(IServiceProvider serviceProvider)
{
    // Lấy RoleManager và UserManager từ ServiceProvider
    var roleManager = serviceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
    var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

    // Danh sách các vai trò cần tạo
    var roles = new[] { "Admin", "Staff", "Customer" };

    foreach (var role in roles)
    {
        // Kiểm tra và tạo vai trò nếu chưa tồn tại
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new ApplicationRole { Name = role});
        }
    }

    // Tạo tài khoản Admin mặc định nếu chưa tồn tại
    var adminEmail = "admin@example.com";
    var adminUser = await userManager.FindByEmailAsync(adminEmail);
    if (adminUser == null)
    {
        var newAdmin = new ApplicationUser
        {
            UserName = "admin",
            Email = adminEmail,
            EmailConfirmed = true
        };

        // Tạo tài khoản với mật khẩu mặc định
        var result = await userManager.CreateAsync(newAdmin, "Admin@123");
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(newAdmin, "Admin");
            Console.WriteLine("Admin user created successfully.");
        }
        else
        {
            foreach (var error in result.Errors)
            {
                Console.WriteLine($"Error creating admin user: {error.Description}");
            }
        }
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSession();
app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

//dinh nghia endPoint cho SignalR
app.MapHub<HubService>("/hubservice");
app.Run();
