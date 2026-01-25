using Graphql1.models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using HotChocolate.Authorization;

namespace Graphql1.GraphQL.Mutation
{
    [ExtendObjectType("Mutation")]
    public class AdminCreateMutation
    {
        [Authorize(Roles = ["Admin"])]
        public async Task<User> CreateAdmin(AddAdminInput adminInput, [Service] AppDbContext db)
        {
            var isAdminExisted = await db.Users.FirstOrDefaultAsync(u => u.Role == "Admin");
            if (isAdminExisted!=null)
            {
                throw new GraphQLException("Cannot Created Admin Account Again");
            }
            var admin = new User
            {
                Role = "Admin",
                Name = adminInput.Name,
                Surname = adminInput.Surname,
                Email = adminInput.Email,
                CreatedAt = DateTime.Now
            };
            //hash 
            var hasher = new PasswordHasher<User>();
            admin.Password=hasher.HashPassword(admin,adminInput.PlainPassword);
            db.Users.Add(admin);
            await db.SaveChangesAsync();
            return admin;
        }
        [Authorize(Roles = ["Admin"])]
        public async Task<User>UpdateAdmin(UpdateAdminInput updateAdminInput, [Service] AppDbContext db)
        {
            if (updateAdminInput == null)
                throw new GraphQLException("invalid Entry");

            //check by email 
            var admin = await db.Users.FirstOrDefaultAsync(u => u.Role == "Admin") ?? throw new GraphQLException("Admin does not exist");
            
           
           
            

            admin.Name = updateAdminInput.Name ?? admin.Name;
            admin.Surname = updateAdminInput.Surname ?? admin.Surname;
            admin.Email=updateAdminInput.Email ?? admin.Email;  

            //password
            if (!string.IsNullOrWhiteSpace(updateAdminInput.PlainPassword))
            {
                var hasher= new PasswordHasher<User>(); 
                admin.Password=hasher.HashPassword(admin,updateAdminInput.PlainPassword);
            }
           
            db.Users.Update(admin);
            await db.SaveChangesAsync();
            return admin;
            

        }
    }

    public record AddAdminInput(string Name,string Surname,string Email,string PlainPassword);
    public record UpdateAdminInput(string Name,string Surname,string Email,string PlainPassword);


}
