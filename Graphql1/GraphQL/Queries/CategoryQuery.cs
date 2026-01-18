
//CategoryQuery.cs 
using Graphql1.models;
using Microsoft.EntityFrameworkCore;
using HotChocolate.Types;

namespace Graphql1.GraphQL.Queries
{
    [ExtendObjectType("Query")]
    public class CategoryQuery
    {
        public async Task<List<Category>> Categories([Service] AppDbContext db)
        {
            return await db.Categories.ToListAsync();

        }

        public async Task<Category?> CategoryByName(string name, [Service] AppDbContext db)
        {
            return await db.Categories.FirstOrDefaultAsync(c => c.Name == name);
        }
    }
}
