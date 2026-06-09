using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Job4Students.Shared;

public class CurrentUserHelper(IHttpContextAccessor accessor)
{
    public bool IsAuthenticated
    {
        get
        {
            try
            {
                return accessor.HttpContext?.User?.Identity?.IsAuthenticated == true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }

    public int UserId
    {
        get
        {
            try
            {
                string? id = accessor.HttpContext?.User?.FindFirst("UserId")?.Value;
                return id != null ? int.Parse(id) : -1;
            }
            catch (Exception)
            {
                return -1;
            }
        }
    }

    public string Role
    {
        get
        {
            try
            {
                return accessor.HttpContext?.User?.FindFirst(ClaimTypes.Role)?.Value ?? string.Empty;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }
    }

    public string Name
    {
        get
        {
            try
            {
                return accessor.HttpContext?.User?.Identity?.Name ?? string.Empty;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }
    }

    public string Email
    {
        get
        {
            try
            {
                return accessor.HttpContext?.User?.FindFirst(ClaimTypes.Email)?.Value ?? string.Empty;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }
    }
}
