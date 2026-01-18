using Graphql1.models;
using Microsoft.EntityFrameworkCore;
using HotChocolate.Types;
using Microsoft.AspNetCore.Identity;

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
        
    }


    //user input record
    public record AddUserInput(string Name,string Surname,string Email, string PlainPassword);
}
