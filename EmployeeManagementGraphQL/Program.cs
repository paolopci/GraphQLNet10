using GraphQL;
using GraphQL.Types;
using GraphQL.Server;

var builder = WebApplication.CreateBuilder(args);

// Controllers (se vuoi mantenere anche REST)
builder.Services.AddControllers();

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
    app.UseGraphiQL("/ui/graphiql", "/graphql");
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// Endpoint GraphQL
app.UseGraphQL<ISchema>("/graphql");

app.Run();