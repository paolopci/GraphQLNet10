using GraphQL;
using GraphQL.Types;
using GraphQL.Server;
using EmployeeManagementGraphQL.Data;
using EmployeeManagementGraphQL.Data.Repositories;
using EmployeeManagementGraphQL.GraphQL.Queries;
using EmployeeManagementGraphQL.GraphQL.Schemas;
using EmployeeManagementGraphQL.GraphQL.Types;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Controllers (se vuoi mantenere anche REST)
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    { // per eliminare i loop circolari
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });
// builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' non trovata in configurazione/User Secrets.");

builder.Services.AddDbContext<EntityDatabaseContext>(options =>
    options.UseSqlServer(connectionString, sqlOptions =>
        sqlOptions.EnableRetryOnFailure()));

builder.Services.AddScoped<EmployeeRepository>();
builder.Services.AddScoped<EmployeeQuery>();
builder.Services.AddScoped<EmployeeGraphType>();
builder.Services.AddScoped<EmployeePagedResultGraphType>();
builder.Services.AddScoped<PageInfoGraphType>();
builder.Services.AddScoped<EmployeeSortFieldEnumType>();
builder.Services.AddScoped<SortDirectionEnumType>();
builder.Services.AddScoped<EmployeeSortInputType>();
builder.Services.AddScoped<EmployeeSchema>();
builder.Services.AddScoped<ISchema>(sp => sp.GetRequiredService<EmployeeSchema>());

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
    // EnsureCreated() crea database e schema direttamente 
    // dal modello EF (DbSet<Employee>, DbSet<Review>), senza migration.
    // È utile per demo/prototipi/test rapidi.
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
app.UseGraphQL<EmployeeSchema>("/graphql");

app.Run();
