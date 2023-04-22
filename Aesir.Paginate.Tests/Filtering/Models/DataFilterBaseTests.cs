using Aesir.Paginate.Filtering.Models;

namespace Aesir.Paginate.Tests.Filtering.Models;

public class DataFilterBaseTests
{
    [Fact]
    public void MaxPageSize_ShouldHaveADefaultValue()
    {
        var filter = new DataFilterBase();
        Assert.True(filter.MaxPageSize > 0);
    }

    [Fact]
    public void MaxPageSize_ShouldBeEditable()
    {
        var filter = new DataFilterBase
        {
            MaxPageSize = 1
        };
        Assert.Equal(1, filter.MaxPageSize);

        filter.MaxPageSize = 2;
        Assert.Equal(2, filter.MaxPageSize);
    }

    [Fact]
    public void PageNumber_ShouldStartAt1()
    {
        var filter = new DataFilterBase();
        Assert.Equal(1, filter.PageNumber);
    }

    [Fact]
    public void PageNumber_ShouldBeEditable()
    {
        var filter = new DataFilterBase
        {
            PageNumber = 2
        };
        Assert.Equal(2, filter.PageNumber);

        filter.PageNumber = 3;
        Assert.Equal(3, filter.PageNumber);
    }

    [Fact]
    public void PageSize_ShouldHaveADefaultValue()
    {
        var filter = new DataFilterBase();
        Assert.True(filter.PageSize > 0);
    }

    [Fact]
    public void PageSize_ShouldNotBeGreaterThanMaxPageSize()
    {
        var filter = new DataFilterBase();
        filter.PageSize = filter.MaxPageSize + 1;
        Assert.True(filter.PageSize == filter.MaxPageSize);
    }

    [Fact]
    public void FilterBy_ShouldNotHaveDefaultValue()
    {
        var filter = new DataFilterBase();
        Assert.Empty(filter.FilterBy);
    }

    [Fact]
    public void OrderBy_ShouldNotHaveDefaultValue()
    {
        var filter = new DataFilterBase();
        Assert.Empty(filter.OrderBy);
    }
}