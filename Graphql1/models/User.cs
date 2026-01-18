//User.cs 

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Graphql1.models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }=String.Empty;
        [Required]
        public string Surname { get; set; }=String.Empty;
        [Required]
        public string Email { get; set; } = String.Empty;
        [Required]
        public string Password { get; set; }= String.Empty; 
        public DateTime CreatedAt { get; set; }=DateTime.Now;
        public List<Blog> Blogs { get; set; }=new List<Blog>();
    }
}
