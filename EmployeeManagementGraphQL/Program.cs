using GraphQL;
using GraphQL.Types;
using GraphQL.Server;
using EmployeeManagementGraphQL.Data;
using EmployeeManagementGraphQL.Data.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Controllers (se vuoi mantenere anche REST)
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' non trovata in configurazione/User Secrets.");

builder.Services.AddDbContext<EntityDatabaseContext>(options =>
    options.UseSqlServer(connectionString, sqlOptions =>
        sqlOptions.EnableRetryOnFailure()));

builder.Services.AddScoped<EmployeeRepository>();

// GraphQL
builder.Services.AddGraphQL(options =>
    {
        options.EnableMetrics = false;
    })
    .AddSystemTextJson()
    .AddGraphTypes(ServiceLifetime.Scoped);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<EntityDatabaseContext>();
    dbContext.Database.EnsureCreated();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseGraphQLGraphiQL("/ui/graphiql");
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// Endpoint GraphQL
app.UseGraphQL<ISchema>("/graphql");

app.Run();
