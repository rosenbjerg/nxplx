using System;

namespace NxPlx.Models
{
    public interface IUserSession
    {
        DateTime Expiration { get; }
        string Id { get; }

        int UserId { get; }
        User User { get; }
        bool IsAdmin { get; }
        string UserAgent { get; }
    }
}