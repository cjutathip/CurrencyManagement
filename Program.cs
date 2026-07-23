using CurrencyManagement.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddSingleton<CurrencyService>();

builder.Services.AddHttpClient<IFxRateService, FxRateService>();

var app = builder.Build();

// ==========================
// Seed Currency Master
// ==========================
using (IServiceScope scope = app.Services.CreateScope())
{
    CurrencyService currencyService =
        scope.ServiceProvider.GetRequiredService<CurrencyService>();

    IFxRateService fxRateService =
        scope.ServiceProvider.GetRequiredService<IFxRateService>();

    await currencyService.StartupSeedAsync(fxRateService);
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Currency}/{action=Index}/{id?}");

app.Run();