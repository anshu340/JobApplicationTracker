using Microsoft.AspNetCore.Http;
namespace JobApplicationTracker.Service.Services.Interfaces;

public interface ICookieService
{
    void AppendCookies(HttpResponse response, string authToken);
}
