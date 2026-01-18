using Microsoft.EntityFrameworkCore;
using Graphql1.models;

namespace Graphql1.GraphQL.Queries
{
    [ExtendObjectType("Query")]
    public class BlogQuery
    {
        
        public async Task<List<Blog>> Blogs([Service] AppDbContext db)
        {
            return await db.Blogs.ToListAsync();


        }
        public async Task<List<Blog>> BlogsByCategory([Service] AppDbContext db,int categoryId)
        {
            var existedCategories=await db.Categories.FirstOrDefaultAsync(c=>c.Id==categoryId);  
            if (existedCategories==null)
            {
                throw new GraphQLException("Category does not exist");
            }
            var existedBlogs=await db.Blogs.Where(b=>b.CategoryId==categoryId).ToListAsync();
            if (!existedBlogs.Any() ) // attention ToListAsync never returns "null" 
            {
                throw new GraphQLException($"There is not any blog with category id {categoryId}");
            }
            return existedBlogs;
            
        }

    }
}
