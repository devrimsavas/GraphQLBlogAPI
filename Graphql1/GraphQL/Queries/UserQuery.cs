//BlogQuery.cs
using Graphql1.models;
using Microsoft.EntityFrameworkCore;
using HotChocolate.Types;
using Microsoft.Identity.Client;
namespace Graphql1.GraphQL.Queries
{
    [ExtendObjectType("Query")]
    public class UserQuery
    {
        public async Task<List<User>> Users([Service] AppDbContext db)
        {
            return await db.Users.ToListAsync();
        }
        public async Task<User?> UserById(int id, [Service] AppDbContext db)
        {
            var existedUser = await db.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (existedUser == null)
            {
                throw new GraphQLException("User does not exist");
            }
            return existedUser;
        }
        
    }
}
