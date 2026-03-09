using EmployeeManagementGraphQL.Mvc.GraphQL;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
var graphQlBaseUrl = builder.Configuration["GraphQl:BaseUrl"];
if (string.IsNullOrWhiteSpace(graphQlBaseUrl))
{
    graphQlBaseUrl = "http://localhost:5232";
}

builder.Services.AddHttpClient<IEmployeeGraphQlClient, EmployeeGraphQlClient>(httpClient =>
{
    // Endpoint configurabile via appsettings, con fallback locale per demo.
    httpClient.BaseAddress = new Uri(graphQlBaseUrl, UriKind.Absolute);
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
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Employees}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
