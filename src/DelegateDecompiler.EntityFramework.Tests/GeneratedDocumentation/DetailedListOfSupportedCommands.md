Detail of supported commands
============
## Documentation produced for DelegateDecompiler, version 0.24.0 on Saturday, 28 April 2018 22:35

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
  * Bool Equals Constant (line 34)
  * Bool Equals Static Variable (line 53)
  * Int Equals Constant (line 70)
  * Select Property Without Computed Attribute (line 87)
  * Select Method Without Computed Attribute (line 104)
  * Select Abstract Member Over Tph Hierarchy (line 121)
  * Select Abstract Member Over Tph Hierarchy After Restricting To Subtype (line 138)
  * Select Multiple Levels Of Abstract Members Over Tph Hierarchy (line 155)

#### [Select Async](../TestGroup05BasicFeatures/Test02SelectAsync.cs):
- Supported
  * Bool Equals Constant Async (line 34)
  * Bool Equals Static Variable To Array Async (line 53)
  * Int Equals Constant (line 70)

#### [Equals And Not Equals](../TestGroup05BasicFeatures/Test03EqualsAndNotEquals.cs):
- Supported
  * Int Equals Constant (line 32)
  * Int Equals Static Variable (line 50)
  * Int Equals String Length (line 67)
  * Int Not Equals String Length (line 84)

#### [Nullable](../TestGroup05BasicFeatures/Test04Nullable.cs):
- Supported
  * Property Is Null (line 35)
  * Bool Equals Static Variable (line 54)
  * Int Equals Constant (line 71)
  * Nullable Init (line 88)
  * Nullable Add (line 105)

#### [Where](../TestGroup05BasicFeatures/Test05Where.cs):
- Supported
  * Where Bool Equals Constant (line 33)
  * Where Bool Equals Static Variable (line 52)
  * Where Int Equals Constant (line 69)
  * Where Filters On Abstract Members Over Tph Hierarchy (line 86)
  * Where Filters On Multiple Levels Of Abstract Members Over Tph Hierarchy (line 103)

#### [Single](../TestGroup05BasicFeatures/Test10Single.cs):
- Supported
  * Single Int Equals Unique Value (line 40)

#### [Single Async](../TestGroup05BasicFeatures/Test11SingleAsync.cs):
- Supported
  * Single Int Equals Unique Value Async (line 41)


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
  * Count Children With Filter By Closure (line 69)
  * Count Children With Filter By External Closure (line 88)
  * Count Children With Filter By External Closure2 (line 108)
  * Singleton Count Children With Filter (line 126)

#### [Sum](../TestGroup15Aggregation/Test02Sum.cs):
- Supported
  * Singleton Sum Children (line 33)
  * Sum Count In Children Where Children Can Be None (line 51)


### Group: Types
#### [Strings](../TestGroup50Types/Test01Strings.cs):
- Supported
  * Concatenate Person Not Handle Null (line 32)
  * Concatenate Person Handle Null (line 49)
  * Concatenate Person Handle Name Order (line 68)
  * Generic Method Person Handle (line 85)

#### [DateTime](../TestGroup50Types/Test05DateTime.cs):
- Supported
  * DateTime Where Compare With Static Variable (line 35)



The End
