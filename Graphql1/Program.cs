using Microsoft.EntityFrameworkCore;
using Graphql1.models;
using Graphql1.GraphQL.Queries;
using Graphql1.GraphQL.Resolvers;
using Graphql1.GraphQL.Mutation;

var builder = WebApplication.CreateBuilder(args);
//appdbcontext 
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

//graphql 
builder.Services
    .AddGraphQLServer()
    .AddQueryType(d => d.Name("Query"))
    .AddTypeExtension<CategoryQuery>()
    .AddTypeExtension<UserQuery>()
    .AddTypeExtension<BlogQuery>()  

    .AddTypeExtension<CategoryResolver>()
    .AddTypeExtension<UserResolver>() 

    .AddMutationType(d=>d.Name("Mutation"))
    .AddTypeExtension<CategoryMutation>()
    .AddTypeExtension<UserMutation>()   
    .AddTypeExtension<BlogMutation>()

    .ModifyRequestOptions(opt =>
    {
        opt.IncludeExceptionDetails = true;
    });




var app = builder.Build();

app.MapGet("/", () => "Hello World!");
app.MapGraphQL();

app.Run();
