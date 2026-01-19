//LoginResult.cs output model 
namespace Graphql1.models
{
    public class LoginResult
    {
        public string Token { get; set; } = string.Empty;
        public int UserId { get; set; }
        public string UserEmail { get; set; }= string.Empty;

    }
}
