Detail With Sql of supported commands
============
## Documentation produced for DelegateDecompiler, version 0.24.0 on Sunday, 29 April 2018 02:03

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


#### [Select Async](../TestGroup05BasicFeatures/Test02SelectAsync.cs):
- Supported
  * Bool Equals Constant Async (line 39)
     * T-Sql executed is

```SQL

```

  * Bool Equals Static Variable To Array Async (line 58)
     * T-Sql executed is

```SQL

```

  * Int Equals Constant (line 75)
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

#### [Skip Take](../TestGroup10OrderTake/Test02SkipTake.cs):


### Group: Quantifier Operators
#### [Any](../TestGroup12QuantifierOperators/Test01Any.cs):

#### [All](../TestGroup12QuantifierOperators/Test02All.cs):
- Supported
  * Singleton All Filter (line 32)
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

#### [Sum](../TestGroup15Aggregation/Test02Sum.cs):


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
