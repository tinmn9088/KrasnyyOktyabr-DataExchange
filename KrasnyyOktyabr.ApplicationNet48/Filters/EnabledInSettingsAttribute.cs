using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Reflection;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace KrasnyyOktyabr.ApplicationNet48.Filters;

public class EnabledInSettingsAttribute(Type settingsType = null, string propertyName = null) : ActionFilterAttribute
{
    private static readonly char[] s_propertyNameSeparator = ['.'];

    public override void OnActionExecuting(HttpActionContext actionContext)
    {
        if (settingsType is null)
        {
            return;
        }

        bool attributeIsPresent = actionContext.ControllerContext.Controller.GetType().IsDefined(typeof(EnabledInSettingsAttribute), false);

        if (!attributeIsPresent)
        {
            return;
        }

        object settings = actionContext.ControllerContext.Configuration.DependencyResolver.GetService(settingsType);

        if (settings is null)
        {
            actionContext.Response = GetErrorResponseMessage();
            return;
        }

        bool? enabled = GetPropertyValue(settings, propertyName) as bool?;

        if (enabled is not true)
        {
            actionContext.Response = GetErrorResponseMessage();
        }
    }

    private static object GetPropertyValue(object source, string name)
    {
        string[] path = name.Split(s_propertyNameSeparator, 2);
        bool isNested = path.Length > 1;

        PropertyInfo property = source.GetType().GetProperty(isNested ? path[0] : name) ?? throw new FailedToGetPropertyValueException(isNested ? path[0] : name);

        object value = property.GetValue(source);

        return isNested
            ? GetPropertyValue(value, path[1])
            : value;
    }

    private static HttpResponseMessage GetErrorResponseMessage()
    {
        HttpResponseMessage message = new(HttpStatusCode.NotFound);

        Dictionary<string, string> content = new()
        {
            { "Message", "This HTTP resource is disabled" },
            { "MessageDetail", "Can be enabled in settings" },
        };

        message.Content = JsonContent.Create(content);

        return message;
    }

    public class FailedToGetPropertyValueException : Exception
    {
        internal FailedToGetPropertyValueException(string propertyName)
            : base($"Property '{propertyName}' not found")
        {
        }
    }
}
