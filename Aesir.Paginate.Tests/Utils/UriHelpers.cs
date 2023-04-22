using Aesir.Paginate.Utils;

namespace Aesir.Paginate.Tests.Utils;

public class UriHelpersTests
{
    [Fact]
    public void GetPageUri_ReturnsExpectedUri()
    {
        var baseUri = new Uri("https://example.com/products");
        const int pageNumber = 1;
        const int pageSize = 10;
        var result = UriHelpers.GetPageUri(baseUri, pageNumber, pageSize);
        Assert.Equal("https://example.com/products?pageNumber=1&pageSize=10", result.ToString());
    }

    [Fact]
    public void AddQueryParamArray_AddsSingleValue()
    {
        var uri = new Uri("https://example.com/search");
        const string param = "category";
        var values = new List<string> { "books" };
        var result = UriHelpers.AddQueryParamArray(uri, param, values);
        Assert.Equal("https://example.com/search?category=books", result.ToString());
    }

    [Fact]
    public void AddQueryParamArray_AddsMultipleValues()
    {
        var uri = new Uri("https://example.com/search");
        const string param = "category";
        var values = new List<string> { "books", "electronics" };
        var result = UriHelpers.AddQueryParamArray(uri, param, values);
        Assert.Equal("https://example.com/search?category=books&category=electronics", result.ToString());
    }

    [Fact]
    public void AddQueryParamArray_ExistingQueryParamIsPreserved()
    {
        var uri = new Uri("https://example.com/search?category=books");
        const string param = "sort";
        var values = new List<string> { "price" };
        var result = UriHelpers.AddQueryParamArray(uri, param, values);
        Assert.Equal("https://example.com/search?category=books&sort=price", result.ToString());
    }

    [Fact]
    public void AddQueryParamArray_UrlEncodedValues()
    {
        var uri = new Uri("https://example.com/search");
        const string param = "q";
        var values = new List<string> { "coffee & tea" };
        var result = UriHelpers.AddQueryParamArray(uri, param, values);
        Assert.Equal("https://example.com/search?q=coffee+%26+tea", result.ToString());
    }
}