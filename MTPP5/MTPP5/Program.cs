using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using MTPP5.Areas.Identity;
using MTPP5.Data;
using MTPP5.Data.Entities;
using MTPP5.Hubs;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("sqlite");

builder.Services
    .AddDbContext<ApplicationDbContext>
    (
        options =>
        {
            options.UseSqlite(connectionString);
        }
    );

builder.Services
    .AddDbContextFactory<ApplicationDbContext>
    (
        options =>
        {
            options.UseSqlite(connectionString);
        },
        ServiceLifetime.Scoped
    );

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services
    .AddDefaultIdentity<User>
    (
        options =>
        {
            options.SignIn.RequireConfirmedAccount = false;
            options.SignIn.RequireConfirmedEmail = false;
            options.SignIn.RequireConfirmedPhoneNumber = false;

            options.Password.RequireDigit = false;
            options.Password.RequireUppercase = false;
            options.Password.RequireLowercase = false;
            options.Password.RequireNonAlphanumeric = false;
        }
    )
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddScoped<AuthenticationStateProvider, RevalidatingIdentityAuthenticationStateProvider<User>>();

builder.Services.AddResponseCompression
(
    opts =>
    {
        opts.MimeTypes =
            ResponseCompressionDefaults.MimeTypes.Concat(new[] { "application/octet-stream" });
    }
);

var app = builder.Build();

app.UseResponseCompression();

if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapBlazorHub();
app.MapHub<AppHub>(AppHub.Route);
app.MapFallbackToPage("/_Host");

app.Run();
