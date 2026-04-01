using ChatIllinoisHelper.Data.DataContext;
using ChatIllinoisHelper.Data.DataHelper;
using ChatIllinoisHelper.Data.Logic;
using ChatIllinoisHelperBackend.Components;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApp(builder.Configuration.GetSection("AzureAd"));
builder.Services.AddControllersWithViews()
    .AddMicrosoftIdentityUI();

builder.Services.AddCors(options => {
    options.AddDefaultPolicy(policy => {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddRazorPages(options => {
    options.Conventions.AllowAnonymousToFolder("/img");
});

builder.Services.AddWebOptimizer(pipeline => {
    pipeline.AddJavaScriptBundle("/js/site.js", "/wwwroot/js/*.js").UseContentRoot();
    pipeline.AddCssBundle("/css/site.css", "/wwwroot/css/*.css").UseContentRoot();
});


var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContextFactory<ChatContext>(options => options.UseSqlServer(connectionString));
builder.Services.AddScoped<ChatRepository>();
builder.Services.AddScoped<ChatItemHelper>();

// Add user secrets in development
if (builder.Environment.IsDevelopment()) {
    builder.Configuration.AddUserSecrets<Program>();
}

// Add ChatbotInterface service
builder.Services.AddScoped(provider => {
    var configuration = provider.GetRequiredService<IConfiguration>();
    var url = configuration["ChatbotUrl"];
    var helper = provider.GetRequiredService<ChatItemHelper>();
    return new ChatbotInterface(url, helper);
});

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) {
    app.UseMigrationsEndPoint();
} else {
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseCors();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Lifetime.ApplicationStarted.Register(() => {
    var factory = app.Services.GetService<IServiceScopeFactory>() ?? throw new NullReferenceException("service scope factory is null");
    using var serviceScope = factory.CreateScope();
    // Ensure the database is created
    var context = serviceScope.ServiceProvider.GetRequiredService<ChatContext>();
    context.Database.Migrate();
});

// Anonymous API endpoint for chat
app.MapGet("/api/chat", async (HttpContext context, ILogger<Program> logger) => {
    var query = context.Request.Query["q"].ToString();
    var source = context.Request.Query["source"].ToString();

    if (string.IsNullOrEmpty(query)) {
        return Results.BadRequest("Query parameter 'q' is required.");
    }

    if (string.IsNullOrEmpty(source)) {
        return Results.BadRequest("Query parameter 'source' is required.");
    }

    try {
        logger.LogInformation("Processing query: {Query} from source: {Source}", query, source);

        var chatbot = context.RequestServices.GetRequiredService<ChatbotInterface>();
        var htmlResponse = await chatbot.SendMessage(query, source);

        return Results.Content(htmlResponse, "text/html");
    } catch (Exception ex) {
        logger.LogError(ex, "Error processing chat request");
        return Results.Content($"<h1>Error</h1><p>{ex.Message}</p>", "text/html", statusCode: 500);
    }
}).AllowAnonymous();

app.Run();
