# QueryDesigner
С QueryDesigner вы можете создавать сложные IQueryable фильтры. Эти фильтры строятся в деревья выражений, поэтому их можно применять как в локальных коллекциях, так и в интегрируемых запросах Entity Framework / Linq2SQL.
Данный проект в первую очередь направлен на построение фильтров коллекций динамическим способом, полученных вне среды .NET, например с помощью JavaScript в ASP.NET.

## Базовое использование
Пусть мы имеем сущность пользователя...
```csharp
public class User 
{
  public int Id { get; set; }
  public string Name { get; set; }
  public int Age { get; set; }
}
```

... и запрос получения всех пользователей.
```csharp
IQueryable<User> query = dataAccess.MyUsers;
```

Отлично! Теперь давайте создадим фильтр для них.

Важно, чтобы все члены сущности, которые можно фильтровать, были **свойствами**!
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
Само собой, мы получаем довольно громоздкий код, который вряд ли кто-то будет использовать в таком виде, но, в JSON формате он весьма практичен:
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
Готово! По умолчанию, в Request методы Skip и Take не вызываются, если не заданы соответствующие поля.

## Сложные типы
Давайте расширим имеющуюся сущность пользователя и добавим еще одну:
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
Теперь каждый пользователь может иметь автомобили. Почему Cars в сущности User имеет тип **IEnumerable**, а не **IQueryable**? Это сделано только для удобства, ко всем IEnumerable коллекциям перед построением фильтра применяется метод **AsQueryable**.

Хорошо, давайте выберем из них только тех, кто имеет спорткары, способные развивать скорость выше 300км/ч, для удобства я буду писать сразу в JSON:
```json
{
	"Where": {
		"Field": "Cars.MaxSpeed",
		"FilterType": "GreaterThan",
		"Value": 300
	}
}
```
Field поддерживает обращение к свойствам членов сущностей с неограниченной вложенностью. Аналогично работает сортировка.

## Поддерживаемые методы фильтрации
На данный момент FilterType позволяет фильтровать следующими способами:
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
* InCollection
* NotInCollection
* Any
* NotAny

## Дополнительная информация
При построении Where фильтрации используется TreeFilter, наследуемый от WhereFilter.
Когда свойство OperatorType равняется **None**, то конструктор выражения обращается к полям реализации WhereFilter, иначе к коллекции Operands. Это позволяет строить фильтры любой вложенности.

Если Вы заметите какие либо ошибки или у Вас есть предложения - пожалуйста, дайте мне знать. Спасибо!
