using System.Collections.Generic;
using NxPlx.Application.Models;
using NxPlx.Infrastructure.Events.Events;

namespace NxPlx.Domain.Events.Film
{
    public class CreateUserCommand : IDomainCommand<UserDto>
    {
        public CreateUserCommand(string username, string? email, bool isAdmin, List<int>? libraryIds, string password)
        {
            Username = username;
            Email = email;
            IsAdmin = isAdmin;
            LibraryIds = libraryIds;
            Password = password;
        }

        public string Username { get; }
        public string? Email { get; }
        public bool IsAdmin { get; }
        public List<int>? LibraryIds { get; }
        public string Password { get; }
    }
}