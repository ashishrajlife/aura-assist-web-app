using Microsoft.AspNetCore.Authorization;
using System.Data;

namespace aura_assist_prod.Helpers
{
    public class AuthorizeRolesAttribute : AuthorizeAttribute
    {
        public AuthorizeRolesAttribute(params string[] roles)
        {
            Roles = string.Join(",", roles);
        }
    }
}