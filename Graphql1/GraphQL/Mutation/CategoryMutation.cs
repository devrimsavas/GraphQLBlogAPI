using Graphql1.models;
using Microsoft.EntityFrameworkCore;
using HotChocolate.Types;

namespace Graphql1.GraphQL.Mutation
{
    [ExtendObjectType("Mutation")]
    public class CategoryMutation
    {
        public async Task<Category> AddCategory(AddCategoryInput categoryInput, [Service] AppDbContext db)
        {
            var newCategory=await db.Categories.FirstOrDefaultAsync(c=>c.Name==categoryInput.Name); 
            if (newCategory != null)
            {
                throw new GraphQLException("This Categody Already Exists;");
            }
            if (categoryInput.Name.Length<5)
            {
                throw new GraphQLException("Category name must be greater than 5 chars");

            }
            var category = new Category { Name = categoryInput.Name };
            await db.Categories.AddAsync(category);
            await db.SaveChangesAsync();
            return category;
        }

        public async Task<Category> DeleteCategory(int categoryId, [Service] AppDbContext db)
        {
            var category=await db.Categories.FirstOrDefaultAsync(c=>c.Id==categoryId);
            if (category==null)
            {
                throw new GraphQLException("Category does not exit");
            }
            //category has blogs
            var categoryWithProduct=await db.Blogs.FirstOrDefaultAsync(b=>b.CategoryId==categoryId);    
            if (categoryWithProduct!=null)
            {
                throw new GraphQLException("Category has associated blogs.CannotDelete.");
            }
            db.Categories.Remove(category);
            await db.SaveChangesAsync();
            return category;

        }
    }


    //category input record
    public record AddCategoryInput(string Name);
}
