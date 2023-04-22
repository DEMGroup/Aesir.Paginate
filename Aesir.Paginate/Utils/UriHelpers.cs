using System.Web;
using Microsoft.AspNetCore.WebUtilities;

namespace Aesir.Paginate.Utils;

internal static class UriHelpers
{
    public static Uri GetPageUri(Uri baseUri, int pageNumber, int pageSize)
    {
        var modifiedUri = QueryHelpers.AddQueryString(
            baseUri.ToString(),
            "pageNumber",
            pageNumber.ToString()
        );
        modifiedUri = QueryHelpers.AddQueryString(modifiedUri, "pageSize", pageSize.ToString());
        return new Uri(modifiedUri);
    }

    public static Uri AddQueryParamArray(Uri uri, string param, IEnumerable<string> values)
    {
        var url = uri.ToString();
        foreach (var val in values)
        {
            url = AppendQueryParam(url, param, val);
        }

        return new Uri(url);
    }

    private static string AppendQueryParam(string url, string paramName, string paramValue)
    {
        var uriBuilder = new UriBuilder(url);
        var query = HttpUtility.ParseQueryString(uriBuilder.Query);
        query.Add(paramName, paramValue);
        uriBuilder.Query = query.ToString();
        return uriBuilder.ToString();
    }
}