using Microsoft.AspNetCore.Authorization;

namespace AtonWebAPI.Authentication;

public class BasicAuthorizationAttribute : AuthorizeAttribute
{
    public BasicAuthorizationAttribute()
    {
        Policy = "BasicAuthentication";
    }
}