Detail of supported commands
============
## Documentation produced for DelegateDecompiler, version 0.11.1.0 on 09 December 2014 10:17

This file documents what linq commands **DelegateDecompiler** supports when
working with [Entity Framework v6.1](http://msdn.microsoft.com/en-us/data/aa937723) (EF).
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
  * Bool Equals Constant (line 32)
  * Bool Equals Static Variable (line 51)
  * Int Equals Constant (line 68)

#### [Select Async](../TestGroup05BasicFeatures/Test02SelectAsync.cs):
- **Not Supported**
  * Bool Equals Constant Async (line 30)
  * Bool Equals Static Variable To Array Async (line 49)
  * Int Equals Constant (line 66)

#### [Equals And Not Equals](../TestGroup05BasicFeatures/Test03EqualsAndNotEquals.cs):
- Supported
  * Int Equals Constant (line 32)
  * Int Equals Static Variable (line 50)
  * Int Equals String Length (line 67)
  * Int Not Equals String Length (line 84)

#### [Where](../TestGroup05BasicFeatures/Test05Where.cs):
- Supported
  * Where Bool Equals Constant (line 32)
  * Where Bool Equals Static Variable (line 51)
  * Where Int Equals Constant (line 68)

#### [Single](../TestGroup05BasicFeatures/Test10Single.cs):
- Supported
  * Single Int Equals Unique Value (line 40)

#### [Single Async](../TestGroup05BasicFeatures/Test11SingleAsync.cs):
- **Not Supported**
  * Single Int Equals Unique Value Async (line 33)


### Group: Order Take
#### [Order By](../TestGroup10OrderTake/Test01OrderBy.cs):
- Supported
  * Order By Children Count (line 33)
  * Order By Children Count Then By String Length (line 51)
  * Where Any Children Then Order By Children Count (line 69)

#### [Skip Take](../TestGroup10OrderTake/Test02SkipTake.cs):
- Supported
  * Order By Children Count Then Take (line 33)
  * Order By Children Count Then Skip And Take (line 51)
  * Where Any Children Then Order By Then Skip Take (line 69)


### Group: Quantifier Operators
#### [Any](../TestGroup12QuantifierOperators/Test01Any.cs):
- Supported
  * Any Children (line 32)
  * Any Children With Filter (line 49)

#### [All](../TestGroup12QuantifierOperators/Test02All.cs):
- Supported
  * Singleton All Filter (line 32)
  * All Filter On Children Int (line 49)

#### [Contains](../TestGroup12QuantifierOperators/Test03Contains.cs):
- Supported
  * String Contains Constant String With Filter (line 33)


### Group: Aggregation
#### [Count](../TestGroup15Aggregation/Test01Count.cs):
- Supported
  * Count Children (line 33)
  * Count Children With Filter (line 51)
  * Singleton Count Children With Filter (line 69)

#### [Sum](../TestGroup15Aggregation/Test02Sum.cs):
- Supported
  * Singleton Sum Children (line 33)
- **Not Supported**
  * Sum Count In Children Where Children Can Be None (line 47)


### Group: Types
#### [Strings](../TestGroup50Types/Test01Strings.cs):
- Supported
  * Concatenate Person Not Handle Null (line 32)
  * Concatenate Person Handle Null (line 49)
  * Concatenate Person Handle Name Order (line 68)

#### [DateTime](../TestGroup50Types/Test05DateTime.cs):
- Supported
  * DateTime Where Compare With Static Variable (line 35)



The End
