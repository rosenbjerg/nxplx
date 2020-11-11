using System.Collections.Generic;
using NxPlx.Application.Models.Film;

namespace NxPlx.Application.Models.Events
{
    public class CollectionDetailsLookupEvent : IEvent<MovieCollectionDto>
    {
        public CollectionDetailsLookupEvent(int collectionId)
        {
            CollectionId = collectionId;
        }

        public int CollectionId { get; }
    }
    public class RemoveUserEvent : IEvent<bool>
    {
        public RemoveUserEvent(string username)
        {
            Username = username;
        }

        public string Username { get; }
    }
    public class CreateUserEvent : IEvent<UserDto>
    {
        public CreateUserEvent(string username, string? email, bool isAdmin, List<int>? libraryIds, string password)
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
    public class ListUsersEvent : IEvent<IEnumerable<UserDto>>
    {
    }
}