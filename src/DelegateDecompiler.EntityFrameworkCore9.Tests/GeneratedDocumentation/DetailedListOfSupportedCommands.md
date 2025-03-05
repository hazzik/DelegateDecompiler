Detail of supported commands
============
## Documentation produced for DelegateDecompiler, version 0.34.2.0 on Wednesday, 05 March 2025 13:06

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
  * Bool Equals Constant (line 38)
  * Bool Equals Static Variable (line 61)
  * Int Equals Constant (line 82)
  * Select Property Without Computed Attribute (line 103)
  * Select Method Without Computed Attribute (line 124)
  * Select Abstract Member Over Tph Hierarchy (line 145)
  * Select Abstract Member Over Tph Hierarchy After Restricting To Subtype (line 166)
  * Select Abstract Member Over Tph Hierarchy With Generic Classes After Restricting To Subtype (line 184)
  * Select Abstract Member With Condition On It Over Tph Hierarchy With Generic Classes After Restricting To Subtype (line 212)
  * Select Multiple Levels Of Abstract Members Over Tph Hierarchy (line 234)
  * Select Select Many (line 256)

#### [Select Async](../TestGroup05BasicFeatures/Test02SelectAsync.cs):
- Supported
  * Async (line 43)
  * Bool Equals Constant Async (line 83)
  * Decompile Upfront Bool Equals Constant Async (line 104)
  * Bool Equals Static Variable To Array Async (line 127)
  * Int Equals Constant (line 148)

#### [Equals And Not Equals](../TestGroup05BasicFeatures/Test03EqualsAndNotEquals.cs):
- Supported
  * Int Equals Constant (line 36)
  * Int Equals Static Variable (line 58)
  * Int Equals String Length (line 79)
  * Int Not Equals String Length (line 100)

#### [Nullable](../TestGroup05BasicFeatures/Test04Nullable.cs):
- Supported
  * Property Is Null (line 39)
  * Bool Equals Static Variable (line 62)
  * Int Equals Constant (line 83)
  * Nullable Init (line 104)
  * Nullable Add (line 125)

#### [Where](../TestGroup05BasicFeatures/Test05Where.cs):
- Supported
  * Where Bool Equals Constant (line 37)
  * Where Bool Equals Static Variable (line 60)
  * Where Int Equals Constant (line 81)
  * Where Filters On Abstract Members Over Tph Hierarchy (line 102)

#### [Single](../TestGroup05BasicFeatures/Test10Single.cs):
- Supported
  * Single Int Equals Unique Value (line 42)

#### [Single Async](../TestGroup05BasicFeatures/Test11SingleAsync.cs):
- Supported
  * Single Int Equals Unique Value Async (line 48)


### Group: Order Take
#### [Order By](../TestGroup10OrderTake/Test01OrderBy.cs):
- Supported
  * Order By Children Count (line 37)
  * Order By Children Count Then By String Length (line 59)
  * Where Any Children Then Order By Children Count (line 81)

#### [Skip Take](../TestGroup10OrderTake/Test02SkipTake.cs):
- Supported
  * Order By Children Count Then Take (line 37)
  * Order By Children Count Then Skip And Take (line 59)
  * Where Any Children Then Order By Then Skip Take (line 81)


### Group: Quantifier Operators
#### [Any](../TestGroup12QuantifierOperators/Test01Any.cs):
- Supported
  * Any Children (line 36)
  * Any Children With Filter (line 57)

#### [All](../TestGroup12QuantifierOperators/Test02All.cs):
- Supported
  * Singleton All Filter (line 36)
  * All Filter On Children Int (line 57)

#### [Contains](../TestGroup12QuantifierOperators/Test03Contains.cs):
- Supported
  * String Contains Constant String With Filter (line 37)


### Group: Aggregation
#### [Count](../TestGroup15Aggregation/Test01Count.cs):
- Supported
  * Count Children (line 37)
  * Count Children With Filter (line 59)
  * Count Children With Filter By Closure (line 81)
  * Count Children With Filter By External Closure (line 104)
  * Count Children With Filter By External Closure2 (line 128)
  * Singleton Count Children With Filter (line 150)

#### [Sum](../TestGroup15Aggregation/Test02Sum.cs):
- Supported
  * Sum Count In Children Where Children Can Be None (line 61)

#### [Count Async](../TestGroup15Aggregation/Test03CountAsync.cs):
- Supported
  * Count Children Async (line 44)
  * Count Children With Filter Async (line 66)
  * Count Children With Filter By Closure Async (line 88)
  * Count Children With Filter By External Closure Async (line 111)
  * Count Children With Filter By External Closure2 Async (line 135)
  * Singleton Count Children With Filter Async (line 157)


### Group: Types
#### [Strings](../TestGroup50Types/Test01Strings.cs):
- Supported
  * Concatenate Person Not Handle Null (line 36)
  * Concatenate Person Handle Null (line 57)
  * Concatenate Person Handle Name Order (line 80)
  * Select Generic Method Person Handle (line 101)
  * Filter Generic Method Person Handle (line 119)

#### [DateTime](../TestGroup50Types/Test05DateTime.cs):
- Supported
  * DateTime Where Compare With Static Variable (line 39)


### Group: Additional Features
#### [Nested Expressions](../TestGroup90AdditionalFeatures/Test01NestedExpressions.cs):
- Supported
  * Subquery As Context Extension Method (line 72)



The End
