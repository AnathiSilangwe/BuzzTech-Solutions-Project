using BusinessBuzz.Core.Services;
using BusinessBuzz.Infrastructure.Data;
using BusinessBuzz.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddSession();

builder.Services.AddDbContext<BusinessBuzzDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("BusinessBuzzConnection")));

builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<IJourneyService, JourneyService>();
builder.Services.AddScoped<IBuzzGuideContextService, BuzzGuideContextService>();
builder.Services.AddScoped<IBuzzGuideService, BuzzGuideService>();
builder.Services.AddScoped<IBuzzGuideConversationService, BuzzGuideConversationService>();
builder.Services.AddScoped<IDigiDocsService, DigiDocsService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();
app.UseSession();

app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.Run();
