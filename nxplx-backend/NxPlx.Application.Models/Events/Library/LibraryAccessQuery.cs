using System.Collections.Generic;

namespace NxPlx.Application.Models.Events.Library
{
    public class LibraryAccessQuery : IQuery<List<int>?>
    {
        public LibraryAccessQuery(int userId)
        {
            UserId = userId;
        }
        public int UserId { get; }
    }
}