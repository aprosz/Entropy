using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;

namespace Mvc.LocalizationSample.Web
{
    public abstract class SelfContainedValidationAttribute : ValidationAttribute, IClientModelValidator
    {
        public abstract void AddValidation(ClientModelValidationContext context);

        private IStringLocalizer GetStringLocalizer(
            MvcDataAnnotationsLocalizationOptions options,
            IStringLocalizerFactory factory,
            Type modelType)
        {
            var provider = options.DataAnnotationLocalizerProvider;
            if (factory != null && provider != null)
            {
                return provider(modelType, factory);
            }
            else
            {
                return null;
            }
        }

        protected string GetErrorMessage(ClientModelValidationContext context)
        {
            var services = context.ActionContext.HttpContext.RequestServices;
            var options = services.GetRequiredService<IOptions<MvcDataAnnotationsLocalizationOptions>>();
            var factory = services.GetService<IStringLocalizerFactory>();
            var modelType = context.ModelMetadata.ContainerType ?? context.ModelMetadata.ModelType;

            var stringLocalizer = GetStringLocalizer(options.Value, factory, modelType);
            var displayName = context.ModelMetadata.GetDisplayName();

            return GetErrorMessage(displayName, stringLocalizer);
        }

        protected string GetErrorMessage(ValidationContext validationContext)
        {
            var options = validationContext.GetRequiredService<IOptions<MvcDataAnnotationsLocalizationOptions>>();
            var factory = validationContext.GetService<IStringLocalizerFactory>();
            var modelType = validationContext.ObjectType;
            var stringLocalizer = GetStringLocalizer(options.Value, factory, modelType);

            return GetErrorMessage(validationContext.DisplayName, stringLocalizer);
        }

        private string GetErrorMessage(string displayName, IStringLocalizer stringLocalizer)
        {
            if (stringLocalizer != null &&
                !string.IsNullOrEmpty(ErrorMessage) &&
                string.IsNullOrEmpty(ErrorMessageResourceName) &&
                ErrorMessageResourceType == null)
            {
                return stringLocalizer[ErrorMessage, displayName];
            }

            return FormatErrorMessage(displayName);
        }

        protected static bool MergeAttribute(IDictionary<string, string> attributes, string key, string value)
        {
            if (attributes.ContainsKey(key))
            {
                return false;
            }

            attributes.Add(key, value);
            return true;
        }
    }
}
