using System.ComponentModel.DataAnnotations;

namespace WebRental.Models
{
    public class UserForAuth
    {
        [MinLength(3)]
        [MaxLength(20)]
        public string Username { get; set; }
        [MinLength(6)]
        [MaxLength(20)]
        public string Password { get; set; }
    }
}