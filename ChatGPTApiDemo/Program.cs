using OpenAI.Chat;
using OpenAI.Files;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
var apiKey = builder.Configuration["OpenAI:ApiKey"];
// Register OpenAI services
//builder.Services.AddSingleton(new ChatClient(model: "gpt-4o-mini", apiKey: apiKey));
builder.Services.AddSingleton(new ChatClient(model: "gpt-5", apiKey: apiKey));

builder.Services.AddSingleton(new OpenAIFileClient(apiKey));

builder.Services.AddHttpClient();

//builder.Services.Configure<IISServerOptions>(options =>
//{
//    options.MaxRequestBodySize = int.MaxValue;
//});

//builder.WebHost.ConfigureKestrel(serverOptions =>
//{
//    serverOptions.Limits.KeepAliveTimeout = TimeSpan.FromMinutes(10);
//    serverOptions.Limits.RequestHeadersTimeout = TimeSpan.FromMinutes(10);
//});
////builder.WebHost.UseUrls("http://68.183.205.62:" + (Environment.GetEnvironmentVariable("PORT") ?? "8080"));
//68.183.205.62

var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
builder.WebHost.UseUrls($"http://*:{port}");


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

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Chat}/{action=Index}/{id?}");

app.Run();



