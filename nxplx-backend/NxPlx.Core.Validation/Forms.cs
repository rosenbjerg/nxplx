using System;
using System.Text.RegularExpressions;
using Validation;

namespace NxPlx.Core.Validation
{
    public static class Validators
    {
        private const int MinUsernameLength = 4;
        private const int MaxUsernameLength = 20;
        
        private const int MinPasswordLength = 6;
        private const int MaxPasswordLength = 50;

        private static readonly Validator LoginForm = ValidatorBuilder.New()
            .RequiresString("username", s => s.Length >= MinUsernameLength && s.Length <= MaxUsernameLength)
            .RequiresString("password", s => s.Length >= MinPasswordLength && s.Length <= MaxPasswordLength)
            .Build();

        private static readonly Validator RegisterForm = ValidatorBuilder.New()
            .RequiresString("username", s => s.Length > MinUsernameLength && s.Length < MaxUsernameLength)
            .RequiresString("password", s => s.Length > MinPasswordLength && s.Length < MaxPasswordLength)
            .CanHaveStringWithPattern("email", new Regex("[^@]+@[^.]+(\\.[^.]+)+", RegexOptions.Compiled))
            .RequiresString("privileges", s => s == "admin" || s == "user")
            .CanHaveIntegers("libraries", 0, 100)
            .Build();

        private static readonly Validator SetUserPermissionsForm = ValidatorBuilder.New()
            .RequiresInteger("userId")
            .CanHaveIntegers("libraries", 0, 100)
            .Build();

        private static readonly Validator ChangePasswordForm = ValidatorBuilder.New()
            .RequiresString("oldPassword", s => s.Length > MinPasswordLength && s.Length < MaxPasswordLength)
            .RequiresString("password1", s => s.Length > MinPasswordLength && s.Length < MaxPasswordLength)
            .RequiresString("password2", s => s.Length > MinPasswordLength && s.Length < MaxPasswordLength)
            .Build();
        
        public static Validator Select(Forms form)
        {
            switch (form)
            {
                case Forms.Login: return LoginForm;
                case Forms.CreateUser: return RegisterForm;
                case Forms.SetPermisssions: return SetUserPermissionsForm;
                case Forms.ChangePassword: return ChangePasswordForm;
                default:
                    throw new ArgumentOutOfRangeException(nameof(form), form, null);
            }
        }
    }
        
    public enum Forms
    {
        Login,
        CreateUser,
        SetPermisssions,
        ChangePassword
    }
}