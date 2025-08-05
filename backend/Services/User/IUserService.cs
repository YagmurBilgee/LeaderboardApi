using System;
namespace WebApplication1.Services.User
{
    public interface IUserService
    {
        string? GetUserRank(string course, string username);
    }
}
