# QueryDesigner
[![NuGet](https://img.shields.io/nuget/v/querydesigner.svg?maxAge=259200&style=flat)](http://www.nuget.org/packages/QueryDesigner/)

With QueryDesigner you can create complex IQueryable filters. These filters are built in expression trees, so they can be used in both local collections and integrable queries in Entity Framework or Linq2SQL. The main target of the project is to building a filtering  of collection produced outside the .NET environment, for example with JavaScript in ASP.NET project, in a dynamyc way. 

## Install
```
PM> Install-Package QueryDesigner
```

## Basic usage
Let's say we have a user entity...
```csharp
public class User 
{
  public int Id { get; set; }
  public string Name { get; set; }
  public int Age { get; set; }
}
```

...and all users request.
```csharp
IQueryable<User> query = dataAccess.MyUsers;
```

Excellent! Now let's create a filter for them.

It's important that all members of the entities that could be filtered, should be a properties. Let's say that we want get only users which have Id > 0 || (Name == "Alex" && Age> = 21), and then sort them by Name descending and after - ascending by Id.

It turns out this filter:
```csharp
var filter = new FilterContainer
            {
                Where = new TreeFilter
                {
                    OperatorType = TreeFilterType.Or,
                    Operands = new List<TreeFilter>
                    {
                        new TreeFilter
                        {
                            Field = "Id",
                            FilterType = WhereFilterType.GreaterThan,
                            Value = 0
                        },

                        new TreeFilter
                        {
                            OperatorType = TreeFilterType.And,
                            Operands = new List<TreeFilter>
                            {
                                new TreeFilter
                                {
                                    Field = "Name",
                                    FilterType = WhereFilterType.Equal,
                                    Value = "Alex"
                                },
                                new TreeFilter
                                {
                                    Field = "Age",
                                    FilterType = WhereFilterType.GreaterThanOrEqual,
                                    Value = 21
                                }
                            }
                        }
                    }
                },

                OrderBy = new List<OrderFilter>
                {
                    new OrderFilter
                    {
                        Field = "Name",
                        Order = OrderFilterType.Desc
                    },

                    new OrderFilter
                    {
                        Field = "Id",
                    }
                }
            };
```
"Where" filter has a tree structure of infinite nesting, and OrderBy endless listing.

**Is important: you must construct each filter with only one implementation of a class: either TreeFilter or WhereFilter.** That is, use either OperatorType and Operands or Field, FilterType and Value.

Of course, we got quite uncomfortable code for anyone who will use this form, but in JSON format it is very practical:
```json
{
	"Where": {
		"OperatorType": "Or",
		"Operands": [
			{
				"Field": "Id",
				"FilterType": "GreaterThan",
				"Value": 0
			},
			{
				"OperatorType": "And",
				"Operands": [
				  {
				    "Field": "Name",
				    "FilterType": "Equal",
				    "Value": "Alex"
				  },
				  {
				    "Field": "Age",
				    "FilterType": "GreaterThanOrEqual",
				    "Value": 21
				  }
				]
			}
		]
	},
	"OrderBy": [
		{
			"Field": "Name",
		},
		{
			"Field": "Flag",
			"Order": "Desc"
		}
	],
}
```

Now apply filter to fetch the data:
```csharp
  query = query.Request(filter);
```
or
```csharp
  query = query.Where(filter.Where).OrderBy(filter.OrderBy).Skip(filter.Skip).Take(filter.Take);
```
Complete! By default, the Request Skip and Take methods are not called if you do not set the appropriate fields.

## Complex types
Let's extend existing user entity and add another:
```csharp
public class User 
{
  public int Id { get; set; }
  public string Name { get; set; }
  public int Age { get; set; }
  public IEnumerable<Car> Cars { get; set; }
}

public class Car
{
  public int CarId { get; set; }
  public string Model { get; set; } 
  public int MaxSpeed { get; set; }
}

```
Now every user can have cars. Why Cars from User is of type **IEnumerable**, rather than **IQueryable**? This is for convenience, to all IEnumerable collections is applied **AsQueryable** method.

Okay, now select users only who have sport cars capable of speeds up to 300 km / hour, for convenience I presented in JSON:
```json
{
	"Where": {
		"Field": "Cars.MaxSpeed",
		"FilterType": "GreaterThan",
		"Value": 300
	}
}
```
Field supports the appeal to the properties of the members of the entity with unlimited nesting. Similarly works sorting.

## Filtering methods
Currently FilterType allows you to filter by the following ways:

1. Applied to a single element:
  * Equal
  * NotEqual
  * LessThan
  * GreaterThan
  * LessThanOrEqual
  * GreaterThanOrEqual
  * Contains
  * NotContains
  * StartsWith
  * NotStartsWith
2. Applied to the listed items without using Value:
  * Any
  * NotAny
  
## Entity members
Available types for single member entities, **which can be filtered**:
* DateTime
* TimeSpan
* bool
* int
* uint
* long
* ulong
* Guid
* double
* float
* decimal
* char
* string
* any enumerations

...and them Nullable analogues.

##Additional Information
When building a filter using Where TreeFilter, inherited from WhereFilter. When OperatorType property is equal to None, the expression of the designer refers to the fields of implementation WhereFilter, otherwise Operands to the collection. It allows you to build any nesting filters.

If you notice any errors or have any suggestions - please let me know. Thank you!
