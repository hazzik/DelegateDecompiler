Detail of supported commands
============
## Documentation produced for DelegateDecompiler, version 0.11.1.0 on 10 November 2014 08:37

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
(See [How to add a test](https://github.com/hazzik/DelegateDecompiler/tree/master/src/DelegateDecompiler.EfTests/GeneratedDocumentation/HowToAddMoreTests.md)
documentation on how to do this). 
If there is a problem then please send us your test in the EfTest format as it will be easier 
for us to debug.

*Note: The test suite has only recently been set up and has only a handful of tests at the moment.
More will appear as we move forward.*


### Group: Logical Operators
#### [Boolean](https://github.com/hazzik/DelegateDecompiler/tree/master/src/DelegateDecompiler.EfTests/TestGroup05LogicalOperators/Test01Boolean.cs):
- Supported
  * Bool Equals Constant (line 32)
  * Bool Equals Static Variable (line 51)
  * Int Equals Constant (line 68)


### Group: Equality Operators
#### [Equals And Not Equals](https://github.com/hazzik/DelegateDecompiler/tree/master/src/DelegateDecompiler.EfTests/TestGroup06EqualityOperators/Test01EqualsAndNotEquals.cs):
- Supported
  * Int Equals Constant (line 32)
  * Int Equals Static Variable (line 50)
  * Int Equals String Length (line 67)
  * Int Not Equals String Length (line 84)


### Group: Quantifier Operators
#### [Any](https://github.com/hazzik/DelegateDecompiler/tree/master/src/DelegateDecompiler.EfTests/TestGroup12QuantifierOperators/Test01Any.cs):
- Supported
  * Any Children (line 32)
  * Any Children With Filter (line 49)

#### [All](https://github.com/hazzik/DelegateDecompiler/tree/master/src/DelegateDecompiler.EfTests/TestGroup12QuantifierOperators/Test02All.cs):
- Supported
  * Singleton All Filter (line 32)
  * All Filter On Children Int (line 49)

#### [Contains](https://github.com/hazzik/DelegateDecompiler/tree/master/src/DelegateDecompiler.EfTests/TestGroup12QuantifierOperators/Test03Contains.cs):
- Supported
  * String Contains Constant String With Filter (line 33)


### Group: Aggregation
#### [Count](https://github.com/hazzik/DelegateDecompiler/tree/master/src/DelegateDecompiler.EfTests/TestGroup15Aggregation/Test01Count.cs):
- Supported
  * Count Children (line 33)
  * Count Children With Filter (line 51)
  * Singleton Count Children With Filter (line 69)

#### [Sum](https://github.com/hazzik/DelegateDecompiler/tree/master/src/DelegateDecompiler.EfTests/TestGroup15Aggregation/Test02Sum.cs):
- Supported
  * Singleton Sum Children (line 33)
- Not Supported
  * Sum Count In Children Where Children Can Be None (line 47)


### Group: Types
#### [Strings](https://github.com/hazzik/DelegateDecompiler/tree/master/src/DelegateDecompiler.EfTests/TestGroup50Types/Test01Strings.cs):
- Supported
  * Concatenate Person Not Handle Null (line 32)
  * Concatenate Person Handle Null (line 49)
  * Concatenate Person Handle Name Order (line 68)

#### [DateTime](https://github.com/hazzik/DelegateDecompiler/tree/master/src/DelegateDecompiler.EfTests/TestGroup50Types/Test05DateTime.cs):
- Supported
  * DateTime Where Compare With Static Variable (line 35)



The End
