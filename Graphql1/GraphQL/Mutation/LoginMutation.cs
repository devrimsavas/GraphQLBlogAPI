using Graphql1.models;
using Microsoft.EntityFrameworkCore;
using HotChocolate.Types;
using Microsoft.AspNetCore.Identity;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;

namespace Graphql1.GraphQL.Mutation
{
    [ExtendObjectType("Mutation")]
    public class LoginMutation
    {
        public async Task<LoginResult> Login([Service] AppDbContext db, [Service] JwtSettings jwtSettings,LoginInput loginInput)
        {
            var existedUser=await db.Users.FirstOrDefaultAsync(u=>u.Email == loginInput.Email);
            if (existedUser==null)
            {
                throw new GraphQLException("No user registered with email");
            }
            var passwordHasher = new PasswordHasher<User>();
            var verifyResult=passwordHasher.VerifyHashedPassword(existedUser,existedUser.Password,loginInput.Password);

            if (verifyResult !=PasswordVerificationResult.Success && verifyResult!=PasswordVerificationResult.SuccessRehashNeeded)
            {
                throw new GraphQLException("Invalid Email or Password");
            }

            //create token 
            var token=GenerateJwtToken(existedUser,jwtSettings);
            //return output
            return new LoginResult

            {
                Token = token,
                UserId = existedUser.Id,
                UserEmail = existedUser.Email
            };
        }
        public static string GenerateJwtToken(User user, JwtSettings jwtSettings)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub,user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email,user.Email),
                new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString())
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var jwt = new JwtSecurityToken
                (
                issuer:jwtSettings.Issuer,
                audience:jwtSettings.Audience,
                claims:claims,
                expires:DateTime.UtcNow.AddMinutes(jwtSettings.ExpiryMinutes),
                signingCredentials: creds
                );

            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }

    }
}
