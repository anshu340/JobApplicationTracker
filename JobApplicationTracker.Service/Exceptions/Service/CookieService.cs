﻿using JobApplicationTracker.Service.Configuration;
using JobApplicationTracker.Service.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace JobApplicationTracker.Service.Services.Service;

public class CookieService : ICookieService
{
    private readonly JwtSettings _jwtSettings;

    public CookieService(IOptions<JwtSettings> jwtSettings)
    {
        _jwtSettings = jwtSettings.Value;
    }
    public void AppendCookies(HttpResponse response, string authToken)
    {
        CookieOptions options = BuildCookies();
        response.Cookies.Append("authToken",authToken);
    }


    private CookieOptions BuildCookies()
    {
        return new CookieOptions()
        {
            HttpOnly = false,
            SameSite = SameSiteMode.None,
            Secure = true,
            IsEssential = true,
            Expires = DateTime.UtcNow.AddMinutes(Convert.ToInt32(_jwtSettings.ExpireMinutes))
        };
    }
}
