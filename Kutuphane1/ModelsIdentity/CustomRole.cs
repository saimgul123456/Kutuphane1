using Microsoft.AspNetCore.Identity;

namespace Kutuphane1.ModelsIdentity
{
    public class CustomRole : IdentityRole
    {
        public string Detail { get; set; }
    }
}
