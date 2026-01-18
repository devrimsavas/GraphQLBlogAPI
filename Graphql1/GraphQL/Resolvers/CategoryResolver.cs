using Microsoft.EntityFrameworkCore;
using HotChocolate;
using HotChocolate.Types;
using Graphql1.GraphQL;
using Graphql1.models;
using Graphql1.GraphQL.Queries;
using Graphql1.GraphQL.Resolvers;

namespace Graphql1.GraphQL.Resolvers
{
    [ExtendObjectType(typeof(Category))]
    public class CategoryResolver
    {
        public async Task<List<Blog>> Blogs([Parent] Category category, [Service]AppDbContext db)
        {
            return await db.Blogs
                .Where(b=>b.CategoryId== category.Id)   
                .ToListAsync(); 

        }
    }
}
