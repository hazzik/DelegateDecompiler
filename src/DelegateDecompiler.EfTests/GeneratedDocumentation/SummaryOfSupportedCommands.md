Summary of supported commands
============
## Documentation produced for DelegateDecompiler, version 0.11.1.0 on 10 November 2014 09:02

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
If there is a problem then please send us your test in the EfTest format as it will be easier 
for us to debug.

*Note: The test suite has only recently been set up and has only a handful of tests at the moment.
More will appear as we move forward.*


### Group: Logical Operators
- Supported
  * [Boolean](../TestGroup05LogicalOperators/Test01Boolean.cs) (3 tests)

### Group: Equality Operators
- Supported
  * [Equals And Not Equals](../TestGroup06EqualityOperators/Test01EqualsAndNotEquals.cs) (4 tests)

### Group: Quantifier Operators
- Supported
  * [Any](../TestGroup12QuantifierOperators/Test01Any.cs) (2 tests)
  * [All](../TestGroup12QuantifierOperators/Test02All.cs) (2 tests)
  * [Contains](../TestGroup12QuantifierOperators/Test03Contains.cs) (1 tests)

### Group: Aggregation
- Supported
  * [Count](../TestGroup15Aggregation/Test01Count.cs) (3 tests)
- Partially Supported
  * [Sum](../TestGroup15Aggregation/Test02Sum.cs) (1 of 2 tests passed)

### Group: Types
- Supported
  * [Strings](../TestGroup50Types/Test01Strings.cs) (3 tests)
  * [DateTime](../TestGroup50Types/Test05DateTime.cs) (1 tests)


The End
