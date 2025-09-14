
# Aesir.Paginate

<div align="center">
<img src="https://user-images.githubusercontent.com/11881500/233813775-56bc5716-72ed-46a5-8c01-d9c582aae11f.jpg" width="500"/>
</div>
<div align="center">
  <i>Paginate - A filtering, sorting and paging library.</i>
</div>

## Getting Started

Install Aesir.Paginate:

`dotnet add package Aesir.Paginate`

The filter object is what controls the pagination, filtering and sorting:

```csharp
var filter = new DataFilterBase {
 MaxPageSize = 30,
 PageSize = 30,
 PageNumber = 1
};

var pagedOrders = _db.Orders.PageAndSortData(UriHelpers.CreateRoute(HttpContext.Request), filter);
```

### Filtering the data

Currently Aesir.Paginate only supports "Contains" as a filtering option.

```csharp
var filter = new DataFilterBase {
 MaxPageSize = 30,
 PageSize = 30,
 PageNumber = 1
};

filter.FilterBy.Add($"{nameof(Order.Id)}:1") // Find any orders where Id contains 1
filter.FilterBy.Add($"{nameof(Order.Note)}:[a,b]") // Also only find orders where the note contains a OR b
var pagedOrders = _db.Orders.PageAndSortData(UriHelpers.CreateRoute(HttpContext.Request), filter);
```

### Sorting the data

Aesir.Paginate supports "Contains", "StartsWith", and "EndsWith" as filtering options.

```csharp
var filter = new DataFilterBase {
 MaxPageSize = 30,
 PageSize = 30,
 PageNumber = 1
};

filter.OrderBy.Add($"{nameof(Order.Id)},desc") // Order by Id descending
filter.OrderBy.Add($"{nameof(Order.Note)}") // Then order by note ascending
var pagedOrders = _db.Orders.PageAndSortData(UriHelpers.CreateRoute(HttpContext.Request), filter);
```
