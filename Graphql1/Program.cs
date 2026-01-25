using Microsoft.EntityFrameworkCore;
using Graphql1.models;
using Graphql1.GraphQL.Queries;
using Graphql1.GraphQL.Resolvers;
using Graphql1.GraphQL.Mutation;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using HotChocolate.Authorization;


var builder = WebApplication.CreateBuilder(args);
//CORS 
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyHeader()
        .AllowAnyMethod()
        .AllowAnyOrigin();

    });

});

//JWT 
var jwtSettings = new JwtSettings();
builder.Configuration.GetSection("JWTSettings").Bind(jwtSettings);
builder.Services.AddSingleton(jwtSettings);
//JWT authentification 
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings.Issuer,
        ValidAudience = jwtSettings.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8
            .GetBytes(jwtSettings.SecretKey ?? "")
            ),
    };

});
//Authorization 
builder.Services.AddAuthorization();


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
    .AddTypeExtension<LoginMutation>()
    .AddTypeExtension<AdminCreateMutation>()
    .AddAuthorization()

    .ModifyRequestOptions(opt =>
    {
        opt.IncludeExceptionDetails = true;
    });




var app = builder.Build();
app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/", () => "Hello World!");
app.MapGraphQL();

app.Run();
