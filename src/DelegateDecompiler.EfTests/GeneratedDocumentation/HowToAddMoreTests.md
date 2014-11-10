# Documentation on the DelegateDecompiler.EfTests test harness

DelegateDecompiler has a test harness called 
[DelegateDecompiler.EfTests](https://github.com/hazzik/DelegateDecompiler/tree/master/src/DelegateDecompiler.EfTests) 
that allows checks on what DelegateDecompiler supports. After running all the tests it produces
documentation which details what linq commands DelegateDecompiler supports.

## The aims of EfTest

Building linq expressions to convert directly into database commands is quite difficult.
[Entity Framework](http://msdn.microsoft.com/en-us/data/aa937723) (EF) has certain rules 
and building complex Linq commands that will convert to valid SQL is quite difficult.
The only way to really know if you have succeeded is to Unit test every command.

DelegateDecompiler can really help form complex queries, but the 
problem of knowing what commands EF supports and what
commands DelegateDecompiler supports is a bit of a challenge. 
We could document what commands work but the documentation would get out of date. 

That is why EfTest has been written. It provides three key benefits:

1. You can see if the native EF command will work.
2. You can see if a DelegateDecompiler `[Computed]` version will work.
3. And best of all it produces documentation of what works/does not work.

## Overview of how EfTest works

The test suite uses Convention of Configuration to allow tests to be grouped and ordered 
so that the documentation is easy to find things in. In uses three main elements:

### 1. Test Groups

Test Groups are directories allow similar tests to be put together, e.g. 
the TestGroup05LogicalOperators will hold all Logical Operators.
Under that directory we expect to see Boolean tests, AND/OR tests.

The rules for the Test Group Name is:

- Start with the string 'TestGroup'
- Follow 'TestGroup' with two digits. These just set an order of running of the tests
- The rest of the name is then the string that will be displayed for the group, but with spaced on capital letters.

So, `TestGroup06EqualityOperators` will be run after TestGroup05 and any test methods in that
directory will be shown under the title **Equality Operators**.

### 2. Test Files

The test methods, which use the NUnit test harness, are held in files. These have a similar format to the 
Test Groups: 

- The Filename/Class name must start with the string 'Test'
- Follow 'Test' with two digits. These just set an order of running of the tests
- The rest of the name is then the string that will be displayed for the test, but with spaced on capital letters.

So, `Test01EqualsAndNotEquals` will run before Test02 and all tests in that file will be shown under the 
group title **Equals And Not Equals**.

### 3. The tests themselves

The test method must follow a standard form otherwise they will not build the documentation properly.
Most of these rules will be obvious from looking at an existing test. 
The rules are:

- There is a TestFixture at the top that runs at the start of any testing in this file. 
It sets up various things that are needed to log what happends.
- The Test name must start with 'Test' and the rest of the name is used 
at the display name for the test, spaces added again.
- Then the test sets up its own environment from which you can get a reference to the EF DbContext
- The test then follows a fixed flow:
  * Test a Linq version
  * Call `env.AboutToUseDelegateDecompiler();` to tell the system you are about to test a [Computed] version of the same thing.
  * Test the [Computed] version of the same command. You will have had to added a property to one of the various
EF classes such as EfParent, EfPerson etc.
- At the end it compares the two results. 
  * If Singleton then use `CompareAndLogSingleton`
  * If collection then use .ToList() on end of query and then test with `CompareAndLogList`
NOTE: Very important you use the correct type of compare, Singleton or collection, to get the best results.

Also, if you are testing a filter command like `.Where()` then you MUST to add a select statement to select 
a known, simple property like `ParentId`. That stops problems caused by other Computed properties. 
(If you don't the linq part of the test will fail!) 

Note: The Test don't have to have numbers in them as we can capture the line numbers and order on that.
We suggest that you put simpler tests first building up to more complex at the end. Makes the documentation
flow better.

A few things can happen when you run your tests:

1. Your Linq command throws an exception. Then you get a test error but nothing is included in the documentation
because if Linq doesn't work directly then DelegateDecompiler definately isn't going to work.
2. Your [Computed] property throws an exception. This shows as a test error and the command is
listed as **Not Supported** in the documentation.
3. The two answers don't match. Then you get a test error and the command is
listed as **Not Supported** in the documentation.
4. Everything works! No test error and that method is documented as **Supported**.

### Other things

The header text that goes at the start of the documentation comes from a file 
[here](https://github.com/hazzik/DelegateDecompiler/tree/master/src/DelegateDecompiler.EfTests/GeneratedDocumentation/DocumentationHeaderText.md).


## Things to watch out for

1. The system assumes that the classname and namespace match the names of the file and directories respectively. 
It uses this to provide links to that actual test as that is useful for people to see the example.
2. You can add new database tables or add new database properties, but you MUST update 
the file EfItems.DatabaseHelpers to initialise these items. You should also update the unit test 
in TestGroup01Infrastructure.Test01EfDatabase to test the database.
3. Make sure you use the correct compare method, i.e. `CompareAndLogSingleton` for single values 
and `CompareAndLogList` for lists. If you need a
new comparitor then just follow the pattern in the Helpers.CheckerAndLogger class.


