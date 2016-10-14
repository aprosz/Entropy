using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Mvc.LocalizationSample.Web
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter,
        AllowMultiple = false)]
    public class MaxLengthTwoAttribute : SelfContainedValidationAttribute
    {
        private const string _errorMessage = "MaxLengthTwo";

        public MaxLengthTwoAttribute()
        {
            ErrorMessage = _errorMessage;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var result = base.IsValid(value, validationContext);

            if (result.ErrorMessage != null)
            {
                result.ErrorMessage = GetErrorMessage(validationContext);
            }

            return result;
        }

        public override bool IsValid(object value)
        {
            var stringValue = value as string;
            if (stringValue == null)
            {
                return false;
            }

            return stringValue.Length < 2;
        }

        public override void AddValidation(ClientModelValidationContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            MergeAttribute(context.Attributes, "data-val", "true");

            var errorMessage = GetErrorMessage(context);

            MergeAttribute(context.Attributes, "data-val-maxlength", errorMessage);
            MergeAttribute(context.Attributes, "data-val-maxlength-max", "2");
        }
    }
}
