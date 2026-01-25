using Microsoft.EntityFrameworkCore;
using Graphql1.models;
using HotChocolate.Types;
using System.Security.Claims;
using HotChocolate.Authorization;
using System.IdentityModel.Tokens.Jwt;

namespace Graphql1.GraphQL.Mutation
{
    [ExtendObjectType("Mutation")]
    public class BlogMutation
    {
        [Authorize]
        public async Task<Blog> CreateBlog([Service] AppDbContext db,BlogCreateInput blogCreateInput)
        {
            if (blogCreateInput == null)
            {
                throw new GraphQLException("Blog Cannot be empty");
            }
            var existedCategory=await db.Categories.FirstOrDefaultAsync(c=>c.Id==blogCreateInput.CategoryId);
            if (existedCategory == null)
            {
                throw new GraphQLException("Category does not exist");
            }
            var existedUser=await db.Users.FirstOrDefaultAsync(u=>u.Id==blogCreateInput.UserId);    
            if (existedUser == null)
            {
                throw new GraphQLException("User Does not Exist");
            }
            var blog = new Blog()
            {
                Content = blogCreateInput.Content,
                CategoryId = blogCreateInput.CategoryId,
                UserId = blogCreateInput.UserId,
            };
            db.Blogs.Add(blog);
            await db.SaveChangesAsync();
            return blog;

        }
        [Authorize]

        public async Task<Blog> DeleteBlog([Service] AppDbContext db,DeleteBlogInput deleteBlogInput,ClaimsPrincipal claims)
        {
            var userIdClaim = claims.FindFirst(JwtRegisteredClaimNames.Sub);
            if (userIdClaim == null)
                throw new GraphQLException("Unauthorized");
            var currentUserId=int.Parse(userIdClaim.Value);
           
            if (deleteBlogInput == null)
            {
                throw new GraphQLException($"{nameof(Blog)} cannot be null");
            }
            var existedBlog=await db.Blogs.FirstOrDefaultAsync(b=>b.Id==deleteBlogInput.BlogId);
            if (existedBlog == null)
            {
                throw new GraphQLException("Blog does not exist");
            }
            if (existedBlog.UserId != currentUserId)
                throw new GraphQLException("You can only delete your own blog");
            db.Blogs.Remove(existedBlog);
            await db.SaveChangesAsync();
            return existedBlog;
        }
        [Authorize]
        public async Task<Blog> UpdateBlog([Service] AppDbContext db,int blogId ,UpdateBlogInput updateBlogInput,ClaimsPrincipal claims)
        {
            var userIdClaim = claims.FindFirst(JwtRegisteredClaimNames.Sub);
            if (userIdClaim == null)
                throw new GraphQLException("Unauthorized");
            var currentUserId=int.Parse(userIdClaim.Value);

            var existedBlog=await db.Blogs.FirstOrDefaultAsync(b=>b.Id==blogId);  
            if (existedBlog == null)
            {
                throw new GraphQLException("Blog does not exist");
            }
            //authorizatino check !!!!!!
            if (existedBlog.UserId != currentUserId)
                throw new GraphQLException("You can only update your own blog");
            //category check 
            var existedCategoryId=await db.Categories.FirstOrDefaultAsync(c=>c.Id==updateBlogInput.CategoryId);
            if (existedCategoryId == null)
            {
                throw new GraphQLException("Category does not exist");
            }
            existedBlog.Content = updateBlogInput.Content;
            existedBlog.CategoryId = updateBlogInput.CategoryId;
            db.Blogs.Update(existedBlog);
            await db.SaveChangesAsync();
            return existedBlog;

        }




    }


    public record BlogCreateInput(string Content, int CategoryId, int UserId);
    public record DeleteBlogInput(int BlogId);
    public record UpdateBlogInput(string Content, int CategoryId);



}
