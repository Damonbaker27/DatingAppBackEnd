using System.Security.Claims;

namespace API.Extensions
{
    public static class ClaimsPrincipleExtentions
    {
        public static string GetUsername(this ClaimsPrincipal user)
        {
            //gets name from 
            return user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }
    }
}
