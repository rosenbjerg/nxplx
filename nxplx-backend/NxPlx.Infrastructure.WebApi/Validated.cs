using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Red;
using Red.Validation;
using Validation;

namespace NxPlx.WebApi
{
    public static class Validated
    {
        private const int MinUsernameLength = 4;
        private const int MaxUsernameLength = 20;
        
        private const int MinPasswordLength = 6;
        private const int MaxPasswordLength = 50;

        private static bool CorrectLength(string str, int min, int max)
        {
            return str.Length >= min && str.Length <= max;
        }
        
        public static readonly Func<Request, Response, Task<HandlerType>> LoginForm = ValidatorBuilder.New()
            .RequiresString("username", s => CorrectLength(s, MinUsernameLength, MaxUsernameLength))
            .RequiresString("password", s => CorrectLength(s, MinPasswordLength, MaxPasswordLength))
            .BuildRedFormMiddleware();

        public static readonly Func<Request, Response, Task<HandlerType>> CreateUserForm = ValidatorBuilder.New()
            .RequiresString("username", s => CorrectLength(s, MinUsernameLength, MaxUsernameLength))
            .CanHaveStringWithPattern("email", new Regex("[^@]+@[^.]+(\\.[^.]+)+", RegexOptions.Compiled))
            .RequiresString("privileges", s => s == "admin" || s == "user")
            .RequiresString("password1", s => CorrectLength(s, MinPasswordLength, MaxPasswordLength))
            .RequiresString("password2", s => CorrectLength(s, MinPasswordLength, MaxPasswordLength))
            .BuildRedFormMiddleware();

        public static readonly Func<Request, Response, Task<HandlerType>> RequireUserIdQuery = ValidatorBuilder.New()
            .RequiresInteger("userId")
            .BuildRedQueryMiddleware();
        
        public static readonly Func<Request, Response, Task<HandlerType>> SetUserPermissionsForm = ValidatorBuilder.New()
            .RequiresInteger("userId")
            .CanHaveIntegers("libraries", 0, 100)
            .BuildRedFormMiddleware();

        public static readonly Func<Request, Response, Task<HandlerType>> ChangePasswordForm = ValidatorBuilder.New()
            .RequiresString("oldPassword", s => CorrectLength(s, MinPasswordLength, MaxPasswordLength))
            .RequiresString("password1", s => CorrectLength(s, MinPasswordLength, MaxPasswordLength))
            .RequiresString("password2", s => CorrectLength(s, MinPasswordLength, MaxPasswordLength))
            .BuildRedFormMiddleware();
    }
}