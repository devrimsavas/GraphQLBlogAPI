//Blog.cs

using System.ComponentModel.DataAnnotations;
namespace Graphql1.models
{
    public class Blog
    {
        [Key]
        public int Id { get; set; }
        public string Content { get; set; }=string.Empty;
        [Required]
        public Category Category { get; set; }
        public int CategoryId { get; set; }
        public User? User { get; set; }
        public int UserId { get; set; }

       
    }
}
