using System.Web.Http;

namespace KrasnyyOktyabr.ApplicationNet48.Modules.API.Attributes;

public class ApiRoutePrefixAttribute(string prefix) : RoutePrefixAttribute(prefix)
{
    public override string Prefix => "api/" + base.Prefix;
}