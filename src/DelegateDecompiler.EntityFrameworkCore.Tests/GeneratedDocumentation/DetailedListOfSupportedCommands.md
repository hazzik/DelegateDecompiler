Detail of supported commands
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
  * Bool Equals Static Variable (line 53)
  * Int Equals Constant (line 70)
  * Select Property Without Computed Attribute (line 87)
  * Select Method Without Computed Attribute (line 104)

#### [Select Async](../TestGroup05BasicFeatures/Test02SelectAsync.cs):
- Supported
  * Bool Equals Constant Async (line 39)
  * Bool Equals Static Variable To Array Async (line 58)
  * Int Equals Constant (line 75)

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

#### [Single](../TestGroup05BasicFeatures/Test10Single.cs):
- Supported
  * Single Int Equals Unique Value (line 40)

#### [Single Async](../TestGroup05BasicFeatures/Test11SingleAsync.cs):
- Supported
  * Single Int Equals Unique Value Async (line 46)


### Group: Order Take
#### [Order By](../TestGroup10OrderTake/Test01OrderBy.cs):

#### [Skip Take](../TestGroup10OrderTake/Test02SkipTake.cs):


### Group: Quantifier Operators
#### [Any](../TestGroup12QuantifierOperators/Test01Any.cs):

#### [All](../TestGroup12QuantifierOperators/Test02All.cs):
- Supported
  * Singleton All Filter (line 32)

#### [Contains](../TestGroup12QuantifierOperators/Test03Contains.cs):
- Supported
  * String Contains Constant String With Filter (line 33)


### Group: Aggregation
#### [Count](../TestGroup15Aggregation/Test01Count.cs):

#### [Sum](../TestGroup15Aggregation/Test02Sum.cs):


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
