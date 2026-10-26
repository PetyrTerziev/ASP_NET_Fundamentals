using System.Globalization;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;

using GuardPost.Common;

using GuardPost.Web.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

// ---- Services (Dependency Injection container) ----
builder.Services.AddApplicationDbContext(builder.Configuration);
builder.Services.AddApplicationServices();

builder.Services.AddControllersWithViews(options =>
{
    // Every POST/PUT/DELETE form must carry an anti-forgery token.
    options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());

    // Bulgarian texts for the built-in model-binding errors.
    var messages = options.ModelBindingMessageProvider;
    messages.SetAttemptedValueIsInvalidAccessor((value, field) => string.Format(ValidationMessages.ValueIsInvalid, value, field));
    messages.SetValueIsInvalidAccessor(value => string.Format(ValidationMessages.ValueIsInvalid, value, string.Empty));
    messages.SetValueMustBeANumberAccessor(field => string.Format(ValidationMessages.ValueMustBeANumber, field));
    messages.SetValueMustNotBeNullAccessor(_ => ValidationMessages.ValueRequired);
    messages.SetMissingKeyOrValueAccessor(() => ValidationMessages.ValueRequired);
});

var app = builder.Build();

// ---- HTTP request pipeline ----
// Bulgarian culture – month and day names, number formatting.
var culture = new CultureInfo("bg-BG");
app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture(culture),
    SupportedCultures = new[] { culture },
    SupportedUICultures = new[] { culture },
});

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/Home/HttpError", "?code={0}");

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

await app.PrepareDatabaseAsync();

await app.RunAsync();
