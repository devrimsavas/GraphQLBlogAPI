//Category.cs 
using System.ComponentModel.DataAnnotations;

namespace Graphql1.models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;    
        public List<Blog> Blogs { get; set; }=new List<Blog>(); 



    }
}
