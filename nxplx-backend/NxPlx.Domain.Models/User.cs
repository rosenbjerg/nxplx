using System;
using System.Collections.Generic;

namespace NxPlx.Domain.Models
{
    public class User : EntityBase
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public bool HasChangedPassword { get; set; }
        public bool Admin { get; set; }
        public List<int> LibraryAccessIds { get; set; }
        public DateTime? LastLogin { get; set; }
        public DateTime? LastOnline { get; set; }
    }
}