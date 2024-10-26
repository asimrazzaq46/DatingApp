using System;
using System.Security.Claims;

namespace API.Extensions;

public static class ClaimsPrincipalExtension
{
    public static string GetUserName(this ClaimsPrincipal user)
    {
        var username = user.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new Exception("Username is not Found in token.");

        return username;
    }

}
