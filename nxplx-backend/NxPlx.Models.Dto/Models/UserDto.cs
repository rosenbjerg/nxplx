using System.Collections.Generic;

namespace NxPlx.Models.Dto.Models
{
    public class UserDto
    {
        public int id { get; set; }
        public string username { get; set; }
        public string email { get; set; }
        public bool isAdmin { get; set; }
        public bool passwordChanged { get; set; }
        public List<int> libraries { get; set; }
    }
}