# GraphQL Blog API — with JWT Auth & Role-Based Authorization

A blog platform API built with **GraphQL (HotChocolate)** on ASP.NET Core, combining JWT authentication and **role-based authorization enforced at the resolver level** — going beyond basic CRUD into properly secured GraphQL mutations.

## 🚀 What this demonstrates

- **GraphQL server** built with HotChocolate — queries, mutations, and a separate resolver layer for computed/nested fields
- **JWT authentication through GraphQL itself** — a `login` mutation validates credentials (with hashed password verification) and returns a signed JWT, rather than relying on a separate REST auth endpoint
- **Resolver-level role-based authorization**, with different rules per operation:
  - `[Authorize]` — logged-in users only (e.g. creating a blog post)
  - `[Authorize(Roles = ["Admin", "User"])]` — either role permitted (e.g. category mutations)
  - `[Authorize(Roles = ["Admin"])]` — admin-only operations (e.g. admin user creation)
- **EF Core + SQL Server**, with migrations for Users, Blogs, and Categories
- **A working frontend** (plain HTML/JS) that authenticates by sending raw GraphQL queries via `fetch` and storing the returned JWT

## 🛠 Tech Stack

- ASP.NET Core 9, HotChocolate (GraphQL server) + HotChocolate.AspNetCore.Authorization
- Entity Framework Core + SQL Server
- JWT Bearer authentication (`System.IdentityModel.Tokens.Jwt`)
- ASP.NET Identity's `PasswordHasher` for credential verification
- Plain HTML/CSS/JS frontend for testing the auth flow

## 📂 Project Structure

```
Graphql1/
├── GraphQL/
│   ├── Queries/
│   │   ├── BlogQuery.cs
│   │   ├── CategoryQuery.cs
│   │   └── UserQuery.cs
│   ├── Mutation/
│   │   ├── BlogMutation.cs          — [Authorize] required
│   │   ├── CategoryMutation.cs      — [Authorize(Roles = ["Admin","User"])]
│   │   ├── UserMutation.cs          — [Authorize(Roles = ["Admin","User"])]
│   │   ├── LoginMutation.cs         — public: validates credentials, issues JWT
│   │   └── AdminCreateMutation.cs   — [Authorize(Roles = ["Admin"])]
│   └── Resolvers/
│       ├── CategoryResolver.cs
│       └── UserResolver.cs
├── models/
│   ├── AppDbContext.cs
│   ├── Blog.cs / Category.cs / User.cs
│   └── JwtSettings.cs / LoginInput.cs / LoginResult.cs
├── Migrations/
└── frontend/
    ├── index.html
    ├── authscript.js              — sends the login mutation, stores the JWT
    └── style.css
```

## 🔑 Authentication & Authorization Flow

1. A client sends a `login` GraphQL mutation with email and password
2. `LoginMutation` looks up the user, verifies the password hash, and — if valid — signs a JWT containing the user's ID, email, and **role claim**
3. Subsequent GraphQL requests include the JWT as a Bearer token
4. HotChocolate's authorization middleware checks the `[Authorize]` attribute on each mutation/query field against the token's claims **before** the resolver runs — a request without the right role never reaches the business logic

Example login mutation (from the frontend):
```graphql
mutation {
  login(loginInput: { email: "user@example.com", password: "..." }) {
    token
    userId
    userEmail
    userName
    userRole
  }
}
```

## ▶️ Getting Started

1. **Configure the database** in `appsettings.json`:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=localhost;Database=graphqlblog;Trusted_Connection=True;TrustServerCertificate=True"
   },
   "JWTSettings": {
     "SecretKey": "<your-generated-secret>",
     "Issuer": "MyIssuer",
     "Audience": "MyAudience",
     "ExpiryMinutes": 60
   }
   ```
2. **Apply migrations**
   ```bash
   dotnet ef database update
   ```
3. **Run the API**
   ```bash
   dotnet run
   ```
   The GraphQL endpoint is available at `/graphql`, with the built-in Banana Cake Pop IDE for exploring the schema and running queries interactively.
4. **Try the frontend** — open `frontend/index.html` and log in against the running API to see the JWT flow end-to-end.

## 📝 Notes

This project focuses on a combination that's easy to get wrong: securing a GraphQL API properly, field by field, rather than locking down an entire endpoint the way a REST API typically would. It complements the JWT/REST authentication used elsewhere in the portfolio (e.g. [DevHouse4](https://github.com/devrimsavas/DevHouse4)) by applying the same principle — verified identity, enforced roles — to a fundamentally different API paradigm.
