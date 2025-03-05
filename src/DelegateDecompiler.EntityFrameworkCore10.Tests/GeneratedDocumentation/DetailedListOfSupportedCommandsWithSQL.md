Detail With Sql of supported commands
============
## Documentation produced for DelegateDecompiler, version 0.34.3.0 on Wednesday, 05 March 2025 18:24

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
     * T-Sql executed is

```SQL
SELECT [e].[ParentBool]
FROM [EfParents] AS [e]
```

  * Bool Equals Static Variable (line 61)
     * T-Sql executed is

```SQL
SELECT ~([e].[ParentBool] ^ @staticBool)
FROM [EfParents] AS [e]
```

  * Int Equals Constant (line 82)
     * T-Sql executed is

```SQL
SELECT ~CAST([e].[ParentInt] ^ 123 AS bit)
FROM [EfParents] AS [e]
```

  * Select Property Without Computed Attribute (line 103)
     * T-Sql executed is

```SQL
SELECT [e].[FirstName] + N' ' + COALESCE([e].[MiddleName], N'') + N' ' + [e].[LastName]
FROM [EfPersons] AS [e]
```

  * Select Method Without Computed Attribute (line 124)
     * T-Sql executed is

```SQL
SELECT [e].[FirstName] + N' ' + COALESCE([e].[MiddleName], N'') + N' ' + [e].[LastName]
FROM [EfPersons] AS [e]
```

  * Select Abstract Member Over Tph Hierarchy (line 145)
     * T-Sql executed is

```SQL
SELECT CASE
    WHEN [l].[Discriminator] = N'Person' THEN N'Human'
    WHEN [l].[Discriminator] = N'WhiteShark' THEN N'Carcharodon carcharias'
    WHEN [l].[Discriminator] = N'AtlanticCod' THEN N'Gadus morhua'
    WHEN [l].[Discriminator] = N'HoneyBee' THEN N'Apis mellifera'
    WHEN [l].[Discriminator] = N'Dog' THEN N'Canis lupus'
END
FROM [LivingBeeing] AS [l]
```

  * Select Abstract Member Over Tph Hierarchy After Restricting To Subtype (line 166)
     * T-Sql executed is

```SQL
SELECT CASE
    WHEN [l].[Discriminator] = N'HoneyBee' THEN N'Apis mellifera'
    WHEN [l].[Discriminator] = N'Dog' THEN N'Canis lupus'
END
FROM [LivingBeeing] AS [l]
WHERE [l].[Discriminator] IN (N'Dog', N'HoneyBee')
```

  * Select Abstract Member Over Tph Hierarchy With Generic Classes After Restricting To Subtype (line 184)
     * T-Sql executed is

```SQL
SELECT CASE
    WHEN [l].[Discriminator] = N'WhiteShark' THEN N'Carcharodon carcharias'
    WHEN [l].[Discriminator] = N'AtlanticCod' THEN N'Gadus morhua'
END AS [Species], CASE
    WHEN [l].[Discriminator] = N'WhiteShark' THEN N'Fish'
    WHEN [l].[Discriminator] = N'AtlanticCod' THEN N'Fish'
END AS [Group]
FROM [LivingBeeing] AS [l]
WHERE [l].[Discriminator] IN (N'AtlanticCod', N'WhiteShark')
```

  * Select Abstract Member With Condition On It Over Tph Hierarchy With Generic Classes After Restricting To Subtype (line 212)
     * T-Sql executed is

```SQL
SELECT CASE
    WHEN [l].[Discriminator] = N'WhiteShark' THEN N'Carcharodon carcharias'
    WHEN [l].[Discriminator] = N'AtlanticCod' THEN N'Gadus morhua'
END AS [Species], CASE
    WHEN [l].[Discriminator] = N'WhiteShark' THEN N'Fish'
    WHEN [l].[Discriminator] = N'AtlanticCod' THEN N'Fish'
END AS [Group]
FROM [LivingBeeing] AS [l]
WHERE [l].[Discriminator] IN (N'AtlanticCod', N'WhiteShark') AND CASE
    WHEN [l].[Discriminator] = N'WhiteShark' THEN N'Carcharodon carcharias'
    WHEN [l].[Discriminator] = N'AtlanticCod' THEN N'Gadus morhua'
END IS NOT NULL AND CASE
    WHEN [l].[Discriminator] = N'WhiteShark' THEN N'Fish'
    WHEN [l].[Discriminator] = N'AtlanticCod' THEN N'Fish'
END IS NOT NULL
```

  * Select Multiple Levels Of Abstract Members Over Tph Hierarchy (line 234)
     * T-Sql executed is

```SQL
SELECT CASE
    WHEN [l].[Discriminator] = N'HoneyBee' THEN N'Apis mellifera'
    WHEN [l].[Discriminator] = N'Dog' THEN N'Canis lupus'
END, CASE
    WHEN [l].[Discriminator] = N'HoneyBee' THEN CAST(0 AS bit)
    WHEN [l].[Discriminator] = N'Dog' THEN CAST(1 AS bit)
    ELSE CAST(0 AS bit)
END
FROM [LivingBeeing] AS [l]
WHERE [l].[Discriminator] IN (N'Dog', N'HoneyBee')
```

  * Select Select Many (line 256)
     * T-Sql executed is

```SQL
SELECT [s0].[EfGrandChildId], [s0].[EfChildId], [s0].[GrandChildBool], [s0].[GrandChildDouble], [s0].[GrandChildInt], [s0].[GrandChildString]
FROM [EfParents] AS [e]
LEFT JOIN (
    SELECT [s].[EfGrandChildId], [s].[EfChildId], [s].[GrandChildBool], [s].[GrandChildDouble], [s].[GrandChildInt], [s].[GrandChildString], [s].[EfParentId]
    FROM (
        SELECT [e1].[EfGrandChildId], [e1].[EfChildId], [e1].[GrandChildBool], [e1].[GrandChildDouble], [e1].[GrandChildInt], [e1].[GrandChildString], [e0].[EfParentId], ROW_NUMBER() OVER(PARTITION BY [e0].[EfParentId] ORDER BY [e1].[EfGrandChildId]) AS [row]
        FROM [EfChildren] AS [e0]
        INNER JOIN [EfGrandChildren] AS [e1] ON [e0].[EfChildId] = [e1].[EfChildId]
    ) AS [s]
    WHERE [s].[row] <= 1
) AS [s0] ON [e].[EfParentId] = [s0].[EfParentId]
```


#### [Select Async](../TestGroup05BasicFeatures/Test02SelectAsync.cs):
- Supported
  * Async (line 43)
     * T-Sql executed is

```SQL
SELECT [e].[EfParentId], [e].[EndDate], [e].[ParentBool], [e].[ParentDouble], [e].[ParentInt], [e].[ParentNullableDecimal1], [e].[ParentNullableDecimal2], [e].[ParentNullableInt], [e].[ParentString], [e].[ParentTimeSpan], [e].[StartDate]
FROM [EfParents] AS [e]
```

  * Bool Equals Constant Async (line 83)
     * T-Sql executed is

```SQL
SELECT [e].[ParentBool]
FROM [EfParents] AS [e]
```

  * Decompile Upfront Bool Equals Constant Async (line 104)
     * T-Sql executed is

```SQL
SELECT [e].[ParentBool]
FROM [EfParents] AS [e]
```

  * Bool Equals Static Variable To Array Async (line 127)
     * T-Sql executed is

```SQL
SELECT ~([e].[ParentBool] ^ @staticBool)
FROM [EfParents] AS [e]
```

  * Int Equals Constant (line 148)
     * T-Sql executed is

```SQL
SELECT ~CAST([e].[ParentInt] ^ 123 AS bit)
FROM [EfParents] AS [e]
```


#### [Equals And Not Equals](../TestGroup05BasicFeatures/Test03EqualsAndNotEquals.cs):
- Supported
  * Int Equals Constant (line 36)
     * T-Sql executed is

```SQL
SELECT ~CAST([e].[ParentInt] ^ 123 AS bit)
FROM [EfParents] AS [e]
```

  * Int Equals Static Variable (line 58)
     * T-Sql executed is

```SQL
SELECT ~CAST([e].[ParentInt] ^ @staticInt AS bit)
FROM [EfParents] AS [e]
```

  * Int Equals String Length (line 79)
     * T-Sql executed is

```SQL
SELECT CASE
    WHEN [e].[ParentInt] = CAST(LEN([e].[ParentString]) AS int) AND [e].[ParentString] IS NOT NULL THEN CAST(1 AS bit)
    ELSE CAST(0 AS bit)
END
FROM [EfParents] AS [e]
```

  * Int Not Equals String Length (line 100)
     * T-Sql executed is

```SQL
SELECT CASE
    WHEN [e].[ParentInt] <> CAST(LEN([e].[ParentString]) AS int) OR [e].[ParentString] IS NULL THEN CAST(1 AS bit)
    ELSE CAST(0 AS bit)
END
FROM [EfParents] AS [e]
```


#### [Nullable](../TestGroup05BasicFeatures/Test04Nullable.cs):
- Supported
  * Property Is Null (line 39)
     * T-Sql executed is

```SQL
SELECT CASE
    WHEN [e].[ParentNullableInt] IS NULL THEN CAST(1 AS bit)
    ELSE CAST(0 AS bit)
END
FROM [EfParents] AS [e]
```

  * Bool Equals Static Variable (line 62)
     * T-Sql executed is

```SQL
SELECT CASE
    WHEN [e].[ParentNullableInt] IS NULL THEN CAST(1 AS bit)
    ELSE CAST(0 AS bit)
END
FROM [EfParents] AS [e]
```

  * Int Equals Constant (line 83)
     * T-Sql executed is

```SQL
SELECT ~CAST(COALESCE([e].[ParentNullableInt], 0) ^ 123 AS bit)
FROM [EfParents] AS [e]
```

  * Nullable Init (line 104)
     * T-Sql executed is

```SQL
SELECT NULL
FROM [EfParents] AS [e]
```

  * Nullable Add (line 125)
     * T-Sql executed is

```SQL
SELECT [e].[ParentNullableDecimal1] + [e].[ParentNullableDecimal2]
FROM [EfParents] AS [e]
```


#### [Where](../TestGroup05BasicFeatures/Test05Where.cs):
- Supported
  * Where Bool Equals Constant (line 37)
     * T-Sql executed is

```SQL
SELECT [e].[EfParentId]
FROM [EfParents] AS [e]
WHERE [e].[ParentBool] = CAST(1 AS bit)
```

  * Where Bool Equals Static Variable (line 60)
     * T-Sql executed is

```SQL
SELECT [e].[EfParentId]
FROM [EfParents] AS [e]
WHERE [e].[ParentBool] = @staticBool
```

  * Where Int Equals Constant (line 81)
     * T-Sql executed is

```SQL
SELECT [e].[EfParentId]
FROM [EfParents] AS [e]
WHERE [e].[ParentInt] = 123
```

  * Where Filters On Abstract Members Over Tph Hierarchy (line 102)
     * T-Sql executed is

```SQL
SELECT [l].[Id]
FROM [LivingBeeing] AS [l]
WHERE CASE
    WHEN [l].[Discriminator] = N'Person' THEN N'Human'
    WHEN [l].[Discriminator] = N'WhiteShark' THEN N'Carcharodon carcharias'
    WHEN [l].[Discriminator] = N'AtlanticCod' THEN N'Gadus morhua'
    WHEN [l].[Discriminator] = N'HoneyBee' THEN N'Apis mellifera'
    WHEN [l].[Discriminator] = N'Dog' THEN N'Canis lupus'
END = N'Human'
```


#### [Single](../TestGroup05BasicFeatures/Test10Single.cs):
- Supported
  * Single Int Equals Unique Value (line 42)
     * T-Sql executed is

```SQL
SELECT TOP(2) [e].[EfParentId], ~CAST([e].[ParentInt] ^ 987 AS bit) AS [IntEqualsUniqueValue]
FROM [EfParents] AS [e]
WHERE [e].[ParentInt] = 987
```


#### [Single Async](../TestGroup05BasicFeatures/Test11SingleAsync.cs):
- Supported
  * Single Int Equals Unique Value Async (line 48)
     * T-Sql executed is

```SQL
SELECT TOP(2) [e].[EfParentId], ~CAST([e].[ParentInt] ^ 987 AS bit) AS [IntEqualsUniqueValue]
FROM [EfParents] AS [e]
WHERE [e].[ParentInt] = 987
```



### Group: Order Take
#### [Order By](../TestGroup10OrderTake/Test01OrderBy.cs):
- Supported
  * Order By Children Count (line 37)
     * T-Sql executed is

```SQL
SELECT [e].[EfParentId]
FROM [EfParents] AS [e]
ORDER BY (
    SELECT COUNT(*)
    FROM [EfChildren] AS [e0]
    WHERE [e].[EfParentId] = [e0].[EfParentId])
```

  * Order By Children Count Then By String Length (line 59)
     * T-Sql executed is

```SQL
SELECT [e].[EfParentId]
FROM [EfParents] AS [e]
ORDER BY (
    SELECT COUNT(*)
    FROM [EfChildren] AS [e0]
    WHERE [e].[EfParentId] = [e0].[EfParentId]), CAST(LEN([e].[ParentString]) AS int)
```

  * Where Any Children Then Order By Children Count (line 81)
     * T-Sql executed is

```SQL
SELECT [e].[EfParentId]
FROM [EfParents] AS [e]
WHERE EXISTS (
    SELECT 1
    FROM [EfChildren] AS [e0]
    WHERE [e].[EfParentId] = [e0].[EfParentId])
ORDER BY (
    SELECT COUNT(*)
    FROM [EfChildren] AS [e1]
    WHERE [e].[EfParentId] = [e1].[EfParentId])
```


#### [Skip Take](../TestGroup10OrderTake/Test02SkipTake.cs):
- Supported
  * Order By Children Count Then Take (line 37)
     * T-Sql executed is

```SQL
SELECT TOP(@p) [e].[EfParentId]
FROM [EfParents] AS [e]
ORDER BY (
    SELECT COUNT(*)
    FROM [EfChildren] AS [e0]
    WHERE [e].[EfParentId] = [e0].[EfParentId])
```

  * Order By Children Count Then Skip And Take (line 59)
     * T-Sql executed is

```SQL
SELECT [e].[EfParentId]
FROM [EfParents] AS [e]
ORDER BY (
    SELECT COUNT(*)
    FROM [EfChildren] AS [e0]
    WHERE [e].[EfParentId] = [e0].[EfParentId])
OFFSET @p ROWS FETCH NEXT @p0 ROWS ONLY
```

  * Where Any Children Then Order By Then Skip Take (line 81)
     * T-Sql executed is

```SQL
SELECT [e].[EfParentId]
FROM [EfParents] AS [e]
WHERE EXISTS (
    SELECT 1
    FROM [EfChildren] AS [e0]
    WHERE [e].[EfParentId] = [e0].[EfParentId])
ORDER BY (
    SELECT COUNT(*)
    FROM [EfChildren] AS [e1]
    WHERE [e].[EfParentId] = [e1].[EfParentId])
OFFSET @p ROWS FETCH NEXT @p ROWS ONLY
```



### Group: Quantifier Operators
#### [Any](../TestGroup12QuantifierOperators/Test01Any.cs):
- Supported
  * Any Children (line 36)
     * T-Sql executed is

```SQL
SELECT CASE
    WHEN EXISTS (
        SELECT 1
        FROM [EfChildren] AS [e0]
        WHERE [e].[EfParentId] = [e0].[EfParentId]) THEN CAST(1 AS bit)
    ELSE CAST(0 AS bit)
END
FROM [EfParents] AS [e]
```

  * Any Children With Filter (line 57)
     * T-Sql executed is

```SQL
SELECT CASE
    WHEN EXISTS (
        SELECT 1
        FROM [EfChildren] AS [e0]
        WHERE [e].[EfParentId] = [e0].[EfParentId] AND [e0].[ChildInt] = 123) THEN CAST(1 AS bit)
    ELSE CAST(0 AS bit)
END
FROM [EfParents] AS [e]
```


#### [All](../TestGroup12QuantifierOperators/Test02All.cs):
- Supported
  * Singleton All Filter (line 36)
     * T-Sql executed is

```SQL
SELECT CASE
    WHEN NOT EXISTS (
        SELECT 1
        FROM [EfParents] AS [e]
        WHERE [e].[ParentInt] <> 123) THEN CAST(1 AS bit)
    ELSE CAST(0 AS bit)
END
```

  * All Filter On Children Int (line 57)
     * T-Sql executed is

```SQL
SELECT CASE
    WHEN NOT EXISTS (
        SELECT 1
        FROM [EfChildren] AS [e0]
        WHERE [e].[EfParentId] = [e0].[EfParentId] AND [e0].[ChildInt] <> 123) THEN CAST(1 AS bit)
    ELSE CAST(0 AS bit)
END
FROM [EfParents] AS [e]
```


#### [Contains](../TestGroup12QuantifierOperators/Test03Contains.cs):
- Supported
  * String Contains Constant String With Filter (line 37)
     * T-Sql executed is

```SQL
SELECT [e].[ParentString]
FROM [EfParents] AS [e]
WHERE [e].[ParentString] LIKE N'%2%'
```



### Group: Aggregation
#### [Count](../TestGroup15Aggregation/Test01Count.cs):
- Supported
  * Count Children (line 37)
     * T-Sql executed is

```SQL
SELECT (
    SELECT COUNT(*)
    FROM [EfChildren] AS [e0]
    WHERE [e].[EfParentId] = [e0].[EfParentId])
FROM [EfParents] AS [e]
```

  * Count Children With Filter (line 59)
     * T-Sql executed is

```SQL
SELECT (
    SELECT COUNT(*)
    FROM [EfChildren] AS [e0]
    WHERE [e].[EfParentId] = [e0].[EfParentId] AND [e0].[ChildInt] = 123)
FROM [EfParents] AS [e]
```

  * Count Children With Filter By Closure (line 81)
     * T-Sql executed is

```SQL
SELECT (
    SELECT COUNT(*)
    FROM [EfChildren] AS [e0]
    WHERE [e].[EfParentId] = [e0].[EfParentId] AND [e0].[ChildInt] = [e].[ParentInt])
FROM [EfParents] AS [e]
```

  * Count Children With Filter By External Closure (line 104)
     * T-Sql executed is

```SQL
SELECT (
    SELECT COUNT(*)
    FROM [EfChildren] AS [e0]
    WHERE [e].[EfParentId] = [e0].[EfParentId] AND [e0].[ChildInt] = @i)
FROM [EfParents] AS [e]
```

  * Count Children With Filter By External Closure2 (line 128)
     * T-Sql executed is

```SQL
SELECT (
    SELECT COUNT(*)
    FROM [EfChildren] AS [e0]
    WHERE [e].[EfParentId] = [e0].[EfParentId] AND [e0].[ChildInt] = @i AND [e0].[EfParentId] = @j)
FROM [EfParents] AS [e]
```

  * Singleton Count Children With Filter (line 150)
     * T-Sql executed is

```SQL
SELECT COUNT(*)
FROM [EfParents] AS [e]
WHERE (
    SELECT COUNT(*)
    FROM [EfChildren] AS [e0]
    WHERE [e].[EfParentId] = [e0].[EfParentId]) = 2
```


#### [Sum](../TestGroup15Aggregation/Test02Sum.cs):
- Supported
  * Sum Count In Children Where Children Can Be None (line 61)
     * T-Sql executed is

```SQL
SELECT COALESCE((
    SELECT COALESCE(SUM([e0].[ChildInt]), 0)
    FROM [EfChildren] AS [e0]
    WHERE [e].[EfParentId] = [e0].[EfParentId]), 0)
FROM [EfParents] AS [e]
```


#### [Count Async](../TestGroup15Aggregation/Test03CountAsync.cs):
- Supported
  * Count Children Async (line 44)
     * T-Sql executed is

```SQL
SELECT (
    SELECT COUNT(*)
    FROM [EfChildren] AS [e0]
    WHERE [e].[EfParentId] = [e0].[EfParentId])
FROM [EfParents] AS [e]
```

  * Count Children With Filter Async (line 66)
     * T-Sql executed is

```SQL
SELECT (
    SELECT COUNT(*)
    FROM [EfChildren] AS [e0]
    WHERE [e].[EfParentId] = [e0].[EfParentId] AND [e0].[ChildInt] = 123)
FROM [EfParents] AS [e]
```

  * Count Children With Filter By Closure Async (line 88)
     * T-Sql executed is

```SQL
SELECT (
    SELECT COUNT(*)
    FROM [EfChildren] AS [e0]
    WHERE [e].[EfParentId] = [e0].[EfParentId] AND [e0].[ChildInt] = [e].[ParentInt])
FROM [EfParents] AS [e]
```

  * Count Children With Filter By External Closure Async (line 111)
     * T-Sql executed is

```SQL
SELECT (
    SELECT COUNT(*)
    FROM [EfChildren] AS [e0]
    WHERE [e].[EfParentId] = [e0].[EfParentId] AND [e0].[ChildInt] = @i)
FROM [EfParents] AS [e]
```

  * Count Children With Filter By External Closure2 Async (line 135)
     * T-Sql executed is

```SQL
SELECT (
    SELECT COUNT(*)
    FROM [EfChildren] AS [e0]
    WHERE [e].[EfParentId] = [e0].[EfParentId] AND [e0].[ChildInt] = @i AND [e0].[EfParentId] = @j)
FROM [EfParents] AS [e]
```

  * Singleton Count Children With Filter Async (line 157)
     * T-Sql executed is

```SQL
SELECT COUNT(*)
FROM [EfParents] AS [e]
WHERE (
    SELECT COUNT(*)
    FROM [EfChildren] AS [e0]
    WHERE [e].[EfParentId] = [e0].[EfParentId]) = 2
```



### Group: Types
#### [Strings](../TestGroup50Types/Test01Strings.cs):
- Supported
  * Concatenate Person Not Handle Null (line 36)
     * T-Sql executed is

```SQL
SELECT [e].[FirstName] + N' ' + COALESCE([e].[MiddleName], N'') + N' ' + [e].[LastName]
FROM [EfPersons] AS [e]
```

  * Concatenate Person Handle Null (line 57)
     * T-Sql executed is

```SQL
SELECT [e].[FirstName] + CASE
    WHEN [e].[MiddleName] IS NULL THEN N''
    ELSE N' '
END + COALESCE([e].[MiddleName], N'') + N' ' + [e].[LastName]
FROM [EfPersons] AS [e]
```

  * Concatenate Person Handle Name Order (line 80)
     * T-Sql executed is

```SQL
SELECT CASE
    WHEN [e].[NameOrder] = CAST(1 AS bit) THEN [e].[LastName] + N', ' + [e].[FirstName] + CASE
        WHEN [e].[MiddleName] IS NULL THEN N''
        ELSE N' '
    END
    ELSE [e].[FirstName] + CASE
        WHEN [e].[MiddleName] IS NULL THEN N''
        ELSE N' '
    END + COALESCE([e].[MiddleName], N'') + N' ' + [e].[LastName]
END
FROM [EfPersons] AS [e]
```

  * Select Generic Method Person Handle (line 101)
     * T-Sql executed is

```SQL
SELECT [e].[FirstName] + CASE
    WHEN [e].[MiddleName] IS NULL THEN N''
    ELSE N' '
END + COALESCE([e].[MiddleName], N'') + N' ' + [e].[LastName]
FROM [EfPersons] AS [e]
```

  * Filter Generic Method Person Handle (line 119)
     * T-Sql executed is

```SQL
SELECT [e].[EfPersonId], [e].[FirstName], [e].[LastName], [e].[MiddleName], [e].[NameOrder]
FROM [EfPersons] AS [e]
```


#### [DateTime](../TestGroup50Types/Test05DateTime.cs):
- Supported
  * DateTime Where Compare With Static Variable (line 39)
     * T-Sql executed is

```SQL
SELECT [e].[StartDate]
FROM [EfParents] AS [e]
WHERE [e].[StartDate] > '2000-01-01T00:00:00.0000000'
```



### Group: Additional Features
#### [Nested Expressions](../TestGroup90AdditionalFeatures/Test01NestedExpressions.cs):
- Supported
  * Subquery As Context Extension Method (line 72)
     * T-Sql executed is

```SQL
SELECT [e].[EfParentId] AS [ParentId], COALESCE((
    SELECT TOP(1) [e0].[EfChildId]
    FROM [EfChildren] AS [e0]
    WHERE [e0].[EfParentId] = [e].[EfParentId]), 0) AS [FirstChildId]
FROM [EfParents] AS [e]
```




The End
