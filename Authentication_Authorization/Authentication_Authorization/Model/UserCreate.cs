using System.ComponentModel.DataAnnotations;

namespace Authentication_Authorization.Model
{
    public class UserCreate
    {
        [Key]
        public int UserId { get; set; }

        public  string ?UserName { get; set; }

        public string? Password { get; set; }

        public string? Role { get; set; }
    }
}
