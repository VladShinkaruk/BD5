using Microsoft.AspNetCore.Identity;
using System;

namespace WebCityEvents.Models
{
    public class ApplicationUser : IdentityUser
    {
        public DateTime RegistrationDate { get; set; }
    }
}
