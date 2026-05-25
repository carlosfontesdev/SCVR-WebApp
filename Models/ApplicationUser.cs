using Microsoft.AspNetCore.Identity;

namespace _01.Carlos.Fontes.Models
{
    public class ApplicationUser : IdentityUser
    {
       
        public bool IsAdmin { get; set; }
        public bool IsSocio { get; set; }
        public int NumeroSocio { get; set; }
    }
}
