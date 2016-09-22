# QueryDesigner
С QueryDesigner вы можете создавать сложные IQueryable фильтры. Полученные фильтры преобразовываются в деревья выражений, которые можно применять в локальных коллекциях, так и в интегрируемых запросах Entity Framework / Linq2SQL.
Данный проект в первую очередь направлен на использовании фильтров, переданныъ с пользовательского интерфейса.

## Базовое использование
Пусть мы имеем модель пользователя...
```scharp
public class User 
{
  public int Id { get; set; }
  public string Name { get; set; }
  public int Age { get; set; }
}
```

... и запрос получения всех пользователей.
```scharp
IQueryable<User> query = dataAccess.MyUsers;
```

Отлично! Теперь давайте создадим фильтр для них.
Допустим я хочу из всех пользователей получить только тех, у которых Id > 0 || ( Name == "Alex" && Age >= 21), а потом отсортировать их по Name по убыванию, после по Id по возрастанию.

Получаем вот такой фильтр:
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
Where фильтр имеет древовидную структуру бесконечной вложенности, а OrderBy бесконечный листинг.
Само собой, мы получаем довольно громоздкий код, который врядли кто-то будет воспроизводить, но, в JSON формате мы имеем весьма привлекательную версию:
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

Теперь применим фильтр к выборке:
```csharp
  query = query.Request(filter);
```
или
```csharp
  query = query.Where(filter.Where).OrderBy(filter.OrderBy).Skip(filter.Skip).Take(filter.Take);
```
По умолчанию, в Request методы Skip и Take не вызываются, если не заданы соответствующие поля.
