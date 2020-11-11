using System.Collections.Generic;

namespace NxPlx.Application.Models.Events
{
    public class GetLibraryAccessEvent : IEvent<List<int>>
    {
        public GetLibraryAccessEvent(int userId)
        {
            UserId = userId;
        }
        public int UserId { get; }
    }
}