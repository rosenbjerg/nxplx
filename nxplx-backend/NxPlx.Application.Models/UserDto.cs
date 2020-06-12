using System.Collections.Generic;

namespace NxPlx.Application.Models
{
    public class UserDto : IDto
    {
        public int id { get; set; }
        public string username { get; set; } = null!;
        public string email { get; set; } = null!;
        public bool isAdmin { get; set; }
        public bool passwordChanged { get; set; }
        public List<int> libraries { get; set; } = null!;
    }
}