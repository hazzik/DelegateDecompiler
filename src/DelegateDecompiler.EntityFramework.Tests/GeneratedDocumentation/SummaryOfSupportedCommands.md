Summary of supported commands
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
- Supported
  * [Select](../TestGroup05BasicFeatures/Test01Select.cs) (8 tests)
  * [Select Async](../TestGroup05BasicFeatures/Test02SelectAsync.cs) (3 tests)
  * [Equals And Not Equals](../TestGroup05BasicFeatures/Test03EqualsAndNotEquals.cs) (4 tests)
  * [Nullable](../TestGroup05BasicFeatures/Test04Nullable.cs) (5 tests)
  * [Where](../TestGroup05BasicFeatures/Test05Where.cs) (5 tests)
  * [Single](../TestGroup05BasicFeatures/Test10Single.cs) (1 tests)
  * [Single Async](../TestGroup05BasicFeatures/Test11SingleAsync.cs) (1 tests)

### Group: Order Take
- Supported
  * [Order By](../TestGroup10OrderTake/Test01OrderBy.cs) (3 tests)
  * [Skip Take](../TestGroup10OrderTake/Test02SkipTake.cs) (3 tests)

### Group: Quantifier Operators
- Supported
  * [Any](../TestGroup12QuantifierOperators/Test01Any.cs) (2 tests)
  * [All](../TestGroup12QuantifierOperators/Test02All.cs) (2 tests)
  * [Contains](../TestGroup12QuantifierOperators/Test03Contains.cs) (1 tests)

### Group: Aggregation
- Supported
  * [Count](../TestGroup15Aggregation/Test01Count.cs) (6 tests)
  * [Sum](../TestGroup15Aggregation/Test02Sum.cs) (2 tests)

### Group: Types
- Supported
  * [Strings](../TestGroup50Types/Test01Strings.cs) (4 tests)
  * [DateTime](../TestGroup50Types/Test05DateTime.cs) (1 tests)


The End
