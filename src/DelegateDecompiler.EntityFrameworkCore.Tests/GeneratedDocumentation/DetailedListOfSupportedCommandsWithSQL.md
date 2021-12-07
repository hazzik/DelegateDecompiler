Detail With Sql of supported commands
============
## Documentation produced for DelegateDecompiler, version 0.29.1 on Wednesday, 08 December 2021 12:44

This file documents what linq commands **DelegateDecompiler** supports when
working with [Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/) (EF).
EF has one of the best implementations for converting Linq `IQueryable<>` commands into database
access commands, in EF's case T-SQL. Therefore it is a good candidate for using in our tests.

This documentation was produced by compaired direct EF Linq queries against the same query implemented
as a DelegateDecompiler's `Computed` properties. This produces a Supported/Not Supported flag
on each command type tested. Tests are groups and ordered to try and make finding things
easier.

So, if you want to use DelegateDecompiler and are not sure whether the linq command
you want to use will work then clone this project and write your own tests.
(See [How to add a test](HowToAddMoreTests.md) documentation on how to do this). 
If there is a problem then please fork the repository and add your own tests. 
That will make it much easier to diagnose your issue.

*Note: The test suite has only recently been set up and has only a handful of tests at the moment.
More will appear as we move forward.*


### Group: Basic Features
#### [Select](../TestGroup05BasicFeatures/Test01Select.cs):
- Supported
  * Bool Equals Constant (line 34)
     * T-Sql executed is

```SQL

```

  * Bool Equals Static Variable (line 53)
     * T-Sql executed is

```SQL

```

  * Int Equals Constant (line 70)
     * T-Sql executed is

```SQL

```

  * Select Property Without Computed Attribute (line 87)
     * T-Sql executed is

```SQL

```

  * Select Method Without Computed Attribute (line 104)
     * T-Sql executed is

```SQL

```

  * Select Abstract Member Over Tph Hierarchy (line 121)
     * T-Sql executed is

```SQL

```

  * Select Abstract Member Over Tph Hierarchy After Restricting To Subtype (line 138)
     * T-Sql executed is

```SQL

```

  * Select Abstract Member Over Tph Hierarchy With Generic Classes After Restricting To Subtype (line 156)
     * T-Sql executed is

```SQL

```

  * Select Abstract Member With Condition On It Over Tph Hierarchy With Generic Classes After Restricting To Subtype (line 181)
     * T-Sql executed is

```SQL

```

  * Select Multiple Levels Of Abstract Members Over Tph Hierarchy (line 199)
     * T-Sql executed is

```SQL

```


#### [Select Async](../TestGroup05BasicFeatures/Test02SelectAsync.cs):
- Supported
  * Async (line 39)
     * T-Sql executed is

```SQL

```

  * Bool Equals Constant Async (line 75)
     * T-Sql executed is

```SQL

```

  * Decompile Upfront Bool Equals Constant Async (line 92)
     * T-Sql executed is

```SQL

```

  * Bool Equals Static Variable To Array Async (line 111)
     * T-Sql executed is

```SQL

```

  * Int Equals Constant (line 128)
     * T-Sql executed is

```SQL

```


#### [Equals And Not Equals](../TestGroup05BasicFeatures/Test03EqualsAndNotEquals.cs):
- Supported
  * Int Equals Constant (line 32)
     * T-Sql executed is

```SQL

```

  * Int Equals Static Variable (line 50)
     * T-Sql executed is

```SQL

```

  * Int Equals String Length (line 67)
     * T-Sql executed is

```SQL

```

  * Int Not Equals String Length (line 84)
     * T-Sql executed is

```SQL

```


#### [Nullable](../TestGroup05BasicFeatures/Test04Nullable.cs):
- Supported
  * Property Is Null (line 35)
     * T-Sql executed is

```SQL

```

  * Bool Equals Static Variable (line 54)
     * T-Sql executed is

```SQL

```

  * Int Equals Constant (line 71)
     * T-Sql executed is

```SQL

```

  * Nullable Init (line 88)
     * T-Sql executed is

```SQL

```

  * Nullable Add (line 105)
     * T-Sql executed is

```SQL

```


#### [Where](../TestGroup05BasicFeatures/Test05Where.cs):
- Supported
  * Where Bool Equals Constant (line 33)
     * T-Sql executed is

```SQL

```

  * Where Bool Equals Static Variable (line 52)
     * T-Sql executed is

```SQL

```

  * Where Int Equals Constant (line 69)
     * T-Sql executed is

```SQL

```

  * Where Filters On Abstract Members Over Tph Hierarchy (line 86)
     * T-Sql executed is

```SQL

```

  * Where Filters On Multiple Levels Of Abstract Members Over Tph Hierarchy (line 104)
     * T-Sql executed is

```SQL

```


#### [Single](../TestGroup05BasicFeatures/Test10Single.cs):
- Supported
  * Single Int Equals Unique Value (line 40)
     * T-Sql executed is

```SQL

```


#### [Single Async](../TestGroup05BasicFeatures/Test11SingleAsync.cs):
- Supported
  * Single Int Equals Unique Value Async (line 46)
     * T-Sql executed is

```SQL

```



### Group: Order Take
#### [Order By](../TestGroup10OrderTake/Test01OrderBy.cs):
- Supported
  * Order By Children Count (line 33)
     * T-Sql executed is

```SQL

```

  * Order By Children Count Then By String Length (line 51)
     * T-Sql executed is

```SQL

```

  * Where Any Children Then Order By Children Count (line 69)
     * T-Sql executed is

```SQL

```


#### [Skip Take](../TestGroup10OrderTake/Test02SkipTake.cs):
- Supported
  * Order By Children Count Then Take (line 33)
     * T-Sql executed is

```SQL

```

  * Order By Children Count Then Skip And Take (line 51)
     * T-Sql executed is

```SQL

```

  * Where Any Children Then Order By Then Skip Take (line 69)
     * T-Sql executed is

```SQL

```



### Group: Quantifier Operators
#### [Any](../TestGroup12QuantifierOperators/Test01Any.cs):
- Supported
  * Any Children (line 32)
     * T-Sql executed is

```SQL

```

  * Any Children With Filter (line 49)
     * T-Sql executed is

```SQL

```


#### [All](../TestGroup12QuantifierOperators/Test02All.cs):
- Supported
  * Singleton All Filter (line 32)
     * T-Sql executed is

```SQL

```

  * All Filter On Children Int (line 49)
     * T-Sql executed is

```SQL

```


#### [Contains](../TestGroup12QuantifierOperators/Test03Contains.cs):
- Supported
  * String Contains Constant String With Filter (line 33)
     * T-Sql executed is

```SQL

```



### Group: Aggregation
#### [Count](../TestGroup15Aggregation/Test01Count.cs):
- Supported
  * Count Children (line 33)
     * T-Sql executed is

```SQL

```

  * Count Children With Filter (line 51)
     * T-Sql executed is

```SQL

```

  * Count Children With Filter By Closure (line 69)
     * T-Sql executed is

```SQL

```

  * Count Children With Filter By External Closure (line 88)
     * T-Sql executed is

```SQL

```

  * Count Children With Filter By External Closure2 (line 108)
     * T-Sql executed is

```SQL

```

  * Singleton Count Children With Filter (line 126)
     * T-Sql executed is

```SQL

```


#### [Sum](../TestGroup15Aggregation/Test02Sum.cs):
- Supported
  * Singleton Sum Children (line 34)
     * T-Sql executed is

```SQL

```

  * Sum Count In Children Where Children Can Be None (line 53)
     * T-Sql executed is

```SQL

```


#### [Count Async](../TestGroup15Aggregation/Test03CountAsync.cs):
- Supported
  * Count Children Async (line 40)
     * T-Sql executed is

```SQL

```

  * Count Children With Filter Async (line 58)
     * T-Sql executed is

```SQL

```

  * Count Children With Filter By Closure Async (line 76)
     * T-Sql executed is

```SQL

```

  * Count Children With Filter By External Closure Async (line 95)
     * T-Sql executed is

```SQL

```

  * Count Children With Filter By External Closure2 Async (line 115)
     * T-Sql executed is

```SQL

```

  * Singleton Count Children With Filter Async (line 133)
     * T-Sql executed is

```SQL

```



### Group: Types
#### [Strings](../TestGroup50Types/Test01Strings.cs):
- Supported
  * Concatenate Person Not Handle Null (line 32)
     * T-Sql executed is

```SQL

```

  * Concatenate Person Handle Null (line 49)
     * T-Sql executed is

```SQL

```

  * Concatenate Person Handle Name Order (line 68)
     * T-Sql executed is

```SQL

```

  * Generic Method Person Handle (line 85)
     * T-Sql executed is

```SQL

```


#### [DateTime](../TestGroup50Types/Test05DateTime.cs):
- Supported
  * DateTime Where Compare With Static Variable (line 35)
     * T-Sql executed is

```SQL

```




The End
