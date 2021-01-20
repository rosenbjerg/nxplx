using System;
using System.ComponentModel.DataAnnotations;

namespace NxPlx.Application.Core.ValidationFilters
{
    [AttributeUsage(AttributeTargets.Property)]
    public class RequiredIfAttribute : RequiredAttribute
    {
        private readonly object _desiredValue;
        private string PropertyName { get; set; }

        public RequiredIfAttribute(string propertyName, object desiredValue)
        {
            _desiredValue = desiredValue;
            PropertyName = propertyName;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext context)
        {
            var instance = context.ObjectInstance;
            var type = instance.GetType();

            if (_desiredValue.Equals(type.GetProperty(PropertyName)!.GetValue(instance, null)))
            {
                return base.IsValid(value, context);
            }

            return ValidationResult.Success;
        }
    }
}