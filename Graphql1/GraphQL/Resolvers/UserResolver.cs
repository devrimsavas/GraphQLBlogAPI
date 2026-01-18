using Microsoft.EntityFrameworkCore;
using Graphql1.models;
namespace Graphql1.GraphQL.Resolvers
{
    [ExtendObjectType(typeof(User))]
    public class UserResolver
    {
        public async Task<List<Blog>> Blogs([Parent] User user, [Service] AppDbContext db)
        {
           return await db.Blogs.Where(b=>b.UserId==user.Id).ToListAsync();
        }
        
        
    }
}
