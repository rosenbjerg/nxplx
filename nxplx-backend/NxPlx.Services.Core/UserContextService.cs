using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NxPlx.Application.Core;
using NxPlx.Models;
using NxPlx.Services.Database;

namespace NxPlx.Core.Services
{
    public class UserContextService
    {
        private readonly DatabaseContext _databaseContext;
        private readonly OperationContext _operationContext;

        public UserContextService(DatabaseContext databaseContext, OperationContext operationContext)
        {
            _databaseContext = databaseContext;
            _operationContext = operationContext;
        }

        private User? _user;

        public async Task<User> GetUser()
        {
            return _user ??= await _databaseContext.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == _operationContext.Session.UserId);
        }
    }
}