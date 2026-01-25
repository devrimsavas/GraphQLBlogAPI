using Graphql1.models;
using Microsoft.EntityFrameworkCore;
using HotChocolate.Types;
using Microsoft.AspNetCore.Identity;
using HotChocolate.Authorization;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace Graphql1.GraphQL.Mutation
{
    [ExtendObjectType("Mutation")]
    public class UserMutation
    {
        public async Task<User> AddUser(AddUserInput userInput, [Service] AppDbContext db )
        {
            var existedUser=await db.Users.FirstOrDefaultAsync(u=>u.Email == userInput.Email);
            if (existedUser != null)
            {
                throw new GraphQLException("A user with same email already exist.");
            }
            var user = new User
            {
                Name = userInput.Name,
                Surname = userInput.Surname,
                Email = userInput.Email,
                //Password = userInput.PlainPassword,
                CreatedAt = DateTime.UtcNow,
            };

            //hash
            var hasher = new PasswordHasher<User>();
            user.Password=hasher.HashPassword(user,userInput.PlainPassword);
            db.Users.Add(user);
            await db.SaveChangesAsync();
            return user;
        }
        [Authorize(Roles = ["Admin","User"])]
        public async Task<User> DeleteUser(DeleteUserInput deleteUserInput, [Service] AppDbContext db,ClaimsPrincipal claims )
        {
           
            var existedUser= await db.Users.FirstOrDefaultAsync(u=>u.Id==deleteUserInput.Id) ?? throw new GraphQLException("User does not exist");
            var userIdClaim = claims.FindFirst(JwtRegisteredClaimNames.Sub);
            if (userIdClaim==null)
            {
                throw new GraphQLException("Unauthorized");
            }
            //user id and role
            var currentUserId=int.Parse(userIdClaim.Value);
            var roleClaim = claims.FindFirst(ClaimTypes.Role) ?? throw new GraphQLException("Unauthorized");
            var currentRole = roleClaim.Value;
            //check 
            if (currentRole != "Admin" && existedUser.Id != currentUserId)
                throw new GraphQLException("You can only delete your own account");

           

            db.Users.Remove(existedUser);
            await db.SaveChangesAsync();
            return existedUser;
        }


    }


    //user input record
    public record AddUserInput(string Name,string Surname,string Email, string PlainPassword);
    public record DeleteUserInput(int Id);
}
