using Microsoft.AspNetCore.Identity;

namespace CineVote.Models
{
    public class Users : IdentityUser
    {

        public string FullName { get; set; }
    }
}
