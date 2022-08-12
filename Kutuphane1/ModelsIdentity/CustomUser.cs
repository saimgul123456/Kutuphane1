using Microsoft.AspNetCore.Identity;
using System;

namespace Kutuphane1.ModelsIdentity
{
    public class CustomUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ProfilePicture { get; set; }
        public string ActivationCode { get; set; }
        public DateTime Birthdate { get; set; }

    }
}
