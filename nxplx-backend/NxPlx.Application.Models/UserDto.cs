using System.Collections.Generic;

namespace NxPlx.Application.Models
{
    public class UserDto : IDto
    {
        public int Id { get; set; }
        public string Username { get; set; } = null!;
        public string Email { get; set; } = null!;
        public bool Admin { get; set; }
        public bool HasChangedPassword { get; set; }
        public List<int> Libraries { get; set; } = null!;
    }
}