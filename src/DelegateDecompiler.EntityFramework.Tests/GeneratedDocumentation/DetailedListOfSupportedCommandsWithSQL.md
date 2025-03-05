Detail With Sql of supported commands
============
## Documentation produced for DelegateDecompiler, version 0.34.2.0 on Wednesday, 05 March 2025 13:06

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
  * Bool Equals Constant (line 38)
     * T-Sql executed is

```SQL
SELECT 
    [Extent1].[ParentBool] AS [ParentBool]
    FROM [dbo].[EfParents] AS [Extent1]
```

  * Bool Equals Static Variable (line 61)
     * T-Sql executed is

```SQL
SELECT 
    CASE WHEN ([Extent1].[ParentBool] = @p__linq__0) THEN cast(1 as bit) WHEN ([Extent1].[ParentBool] <> @p__linq__0) THEN cast(0 as bit) END AS [C1]
    FROM [dbo].[EfParents] AS [Extent1]
```

  * Int Equals Constant (line 82)
     * T-Sql executed is

```SQL
SELECT 
    CASE WHEN (123 = [Extent1].[ParentInt]) THEN cast(1 as bit) WHEN (123 <> [Extent1].[ParentInt]) THEN cast(0 as bit) END AS [C1]
    FROM [dbo].[EfParents] AS [Extent1]
```

  * Select Property Without Computed Attribute (line 103)
     * T-Sql executed is

```SQL
SELECT 
    [Extent1].[FirstName] + N' ' + CASE WHEN ([Extent1].[MiddleName] IS NULL) THEN N'' ELSE [Extent1].[MiddleName] END + N' ' + [Extent1].[LastName] AS [C1]
    FROM [dbo].[EfPersons] AS [Extent1]
```

  * Select Method Without Computed Attribute (line 124)
     * T-Sql executed is

```SQL
SELECT 
    [Extent1].[FirstName] + N' ' + CASE WHEN ([Extent1].[MiddleName] IS NULL) THEN N'' ELSE [Extent1].[MiddleName] END + N' ' + [Extent1].[LastName] AS [C1]
    FROM [dbo].[EfPersons] AS [Extent1]
```

  * Select Abstract Member Over Tph Hierarchy (line 145)
     * T-Sql executed is

```SQL
SELECT 
    CASE WHEN ([Extent1].[Discriminator] = N'Person') THEN N'Human' WHEN ([Extent1].[Discriminator] = N'HoneyBee') THEN N'Apis mellifera' WHEN ([Extent1].[Discriminator] = N'Dog') THEN N'Canis lupus' END AS [C1]
    FROM [dbo].[LivingBeeings] AS [Extent1]
    WHERE [Extent1].[Discriminator] IN (N'Dog',N'HoneyBee',N'Person')
```

  * Select Abstract Member Over Tph Hierarchy After Restricting To Subtype (line 166)
     * T-Sql executed is

```SQL
SELECT 
    CASE WHEN ([Extent1].[Discriminator] = N'HoneyBee') THEN N'Apis mellifera' WHEN ([Extent1].[Discriminator] = N'Dog') THEN N'Canis lupus' END AS [C1]
    FROM [dbo].[LivingBeeings] AS [Extent1]
    WHERE ([Extent1].[Discriminator] IN (N'Dog',N'HoneyBee',N'Person')) AND ([Extent1].[Discriminator] IN (N'Dog',N'HoneyBee'))
```

  * Select Multiple Levels Of Abstract Members Over Tph Hierarchy (line 234)
     * T-Sql executed is

```SQL
SELECT 
    CASE WHEN (CASE WHEN ([Extent1].[Discriminator] = N'HoneyBee') THEN N'Apis mellifera' WHEN ([Extent1].[Discriminator] = N'Dog') THEN N'Canis lupus' END IS NULL) THEN N'' WHEN ([Extent1].[Discriminator] = N'HoneyBee') THEN N'Apis mellifera' WHEN ([Extent1].[Discriminator] = N'Dog') THEN N'Canis lupus' END + N' : ' + CASE WHEN ((CASE WHEN ([Extent1].[Discriminator] = N'HoneyBee') THEN cast(0 as bit) WHEN ([Extent1].[Discriminator] = N'Dog') THEN cast(1 as bit) WHEN ([Extent1].[Discriminator] <> N'Dog') THEN cast(0 as bit) END) = 1) THEN N'True' WHEN ((CASE WHEN ([Extent1].[Discriminator] = N'HoneyBee') THEN cast(0 as bit) WHEN ([Extent1].[Discriminator] = N'Dog') THEN cast(1 as bit) WHEN ([Extent1].[Discriminator] <> N'Dog') THEN cast(0 as bit) END) = 0) THEN N'False' ELSE N'' END AS [C1]
    FROM [dbo].[LivingBeeings] AS [Extent1]
    WHERE ([Extent1].[Discriminator] IN (N'Dog',N'HoneyBee',N'Person')) AND ([Extent1].[Discriminator] IN (N'Dog',N'HoneyBee'))
```

  * Select Select Many (line 256)
     * T-Sql executed is

```SQL
SELECT 
    [Limit1].[EfGrandChildId] AS [EfGrandChildId], 
    [Limit1].[GrandChildBool] AS [GrandChildBool], 
    [Limit1].[GrandChildInt] AS [GrandChildInt], 
    [Limit1].[GrandChildDouble] AS [GrandChildDouble], 
    [Limit1].[GrandChildString] AS [GrandChildString], 
    [Limit1].[EfChildId] AS [EfChildId]
    FROM  [dbo].[EfParents] AS [Extent1]
    OUTER APPLY  (SELECT TOP (1) [Project1].[EfGrandChildId] AS [EfGrandChildId], [Project1].[GrandChildBool] AS [GrandChildBool], [Project1].[GrandChildInt] AS [GrandChildInt], [Project1].[GrandChildDouble] AS [GrandChildDouble], [Project1].[GrandChildString] AS [GrandChildString], [Project1].[EfChildId] AS [EfChildId]
        FROM ( SELECT 
            [Extent3].[EfGrandChildId] AS [EfGrandChildId], 
            [Extent3].[GrandChildBool] AS [GrandChildBool], 
            [Extent3].[GrandChildInt] AS [GrandChildInt], 
            [Extent3].[GrandChildDouble] AS [GrandChildDouble], 
            [Extent3].[GrandChildString] AS [GrandChildString], 
            [Extent3].[EfChildId] AS [EfChildId]
            FROM  [dbo].[EfChilds] AS [Extent2]
            INNER JOIN [dbo].[EfGrandChilds] AS [Extent3] ON [Extent2].[EfChildId] = [Extent3].[EfChildId]
            WHERE [Extent1].[EfParentId] = [Extent2].[EfParentId]
        )  AS [Project1]
        ORDER BY [Project1].[EfGrandChildId] ASC ) AS [Limit1]
```


#### [Select Async](../TestGroup05BasicFeatures/Test02SelectAsync.cs):
- Supported
  * Async (line 43)
     * T-Sql executed is

```SQL
SELECT 
    [Extent1].[EfParentId] AS [EfParentId], 
    [Extent1].[ParentBool] AS [ParentBool], 
    [Extent1].[ParentInt] AS [ParentInt], 
    [Extent1].[ParentNullableInt] AS [ParentNullableInt], 
    [Extent1].[ParentNullableDecimal1] AS [ParentNullableDecimal1], 
    [Extent1].[ParentNullableDecimal2] AS [ParentNullableDecimal2], 
    [Extent1].[ParentDouble] AS [ParentDouble], 
    [Extent1].[ParentString] AS [ParentString], 
    [Extent1].[StartDate] AS [StartDate], 
    [Extent1].[EndDate] AS [EndDate], 
    [Extent1].[ParentTimeSpan] AS [ParentTimeSpan]
    FROM [dbo].[EfParents] AS [Extent1]
```

  * Async Non Generic (line 61)
     * T-Sql executed is

```SQL
SELECT 
    [Extent1].[EfParentId] AS [EfParentId], 
    [Extent1].[ParentBool] AS [ParentBool], 
    [Extent1].[ParentInt] AS [ParentInt], 
    [Extent1].[ParentNullableInt] AS [ParentNullableInt], 
    [Extent1].[ParentNullableDecimal1] AS [ParentNullableDecimal1], 
    [Extent1].[ParentNullableDecimal2] AS [ParentNullableDecimal2], 
    [Extent1].[ParentDouble] AS [ParentDouble], 
    [Extent1].[ParentString] AS [ParentString], 
    [Extent1].[StartDate] AS [StartDate], 
    [Extent1].[EndDate] AS [EndDate], 
    [Extent1].[ParentTimeSpan] AS [ParentTimeSpan]
    FROM [dbo].[EfParents] AS [Extent1]
```

  * Bool Equals Constant Async (line 83)
     * T-Sql executed is

```SQL
SELECT 
    [Extent1].[ParentBool] AS [ParentBool]
    FROM [dbo].[EfParents] AS [Extent1]
```

  * Decompile Upfront Bool Equals Constant Async (line 104)
     * T-Sql executed is

```SQL
SELECT 
    [Extent1].[ParentBool] AS [ParentBool]
    FROM [dbo].[EfParents] AS [Extent1]
```

  * Bool Equals Static Variable To Array Async (line 127)
     * T-Sql executed is

```SQL
SELECT 
    CASE WHEN ([Extent1].[ParentBool] = @p__linq__0) THEN cast(1 as bit) WHEN ([Extent1].[ParentBool] <> @p__linq__0) THEN cast(0 as bit) END AS [C1]
    FROM [dbo].[EfParents] AS [Extent1]
```

  * Int Equals Constant (line 148)
     * T-Sql executed is

```SQL
SELECT 
    CASE WHEN (123 = [Extent1].[ParentInt]) THEN cast(1 as bit) WHEN (123 <> [Extent1].[ParentInt]) THEN cast(0 as bit) END AS [C1]
    FROM [dbo].[EfParents] AS [Extent1]
```


#### [Equals And Not Equals](../TestGroup05BasicFeatures/Test03EqualsAndNotEquals.cs):
- Supported
  * Int Equals Constant (line 36)
     * T-Sql executed is

```SQL
SELECT 
    CASE WHEN (123 = [Extent1].[ParentInt]) THEN cast(1 as bit) WHEN (123 <> [Extent1].[ParentInt]) THEN cast(0 as bit) END AS [C1]
    FROM [dbo].[EfParents] AS [Extent1]
```

  * Int Equals Static Variable (line 58)
     * T-Sql executed is

```SQL
SELECT 
    CASE WHEN ([Extent1].[ParentInt] = @p__linq__0) THEN cast(1 as bit) WHEN ([Extent1].[ParentInt] <> @p__linq__0) THEN cast(0 as bit) END AS [C1]
    FROM [dbo].[EfParents] AS [Extent1]
```

  * Int Equals String Length (line 79)
     * T-Sql executed is

```SQL
SELECT 
    CASE WHEN ([Extent1].[ParentInt] = ( CAST(LEN([Extent1].[ParentString]) AS int))) THEN cast(1 as bit) WHEN ( NOT (([Extent1].[ParentInt] = ( CAST(LEN([Extent1].[ParentString]) AS int))) AND (0 = (CASE WHEN ([Extent1].[ParentString] IS NULL) THEN cast(1 as bit) ELSE cast(0 as bit) END)))) THEN cast(0 as bit) END AS [C1]
    FROM [dbo].[EfParents] AS [Extent1]
```

  * Int Not Equals String Length (line 100)
     * T-Sql executed is

```SQL
SELECT 
    CASE WHEN ( NOT (([Extent1].[ParentInt] = ( CAST(LEN([Extent1].[ParentString]) AS int))) AND (0 = (CASE WHEN ([Extent1].[ParentString] IS NULL) THEN cast(1 as bit) ELSE cast(0 as bit) END)))) THEN cast(1 as bit) WHEN ([Extent1].[ParentInt] = ( CAST(LEN([Extent1].[ParentString]) AS int))) THEN cast(0 as bit) END AS [C1]
    FROM [dbo].[EfParents] AS [Extent1]
```


#### [Nullable](../TestGroup05BasicFeatures/Test04Nullable.cs):
- Supported
  * Property Is Null (line 39)
     * T-Sql executed is

```SQL
SELECT 
    CASE WHEN ([Extent1].[ParentNullableInt] IS NULL) THEN cast(1 as bit) ELSE cast(0 as bit) END AS [C1]
    FROM [dbo].[EfParents] AS [Extent1]
```

  * Bool Equals Static Variable (line 62)
     * T-Sql executed is

```SQL
SELECT 
    CASE WHEN (([Extent1].[ParentNullableInt] = @p__linq__0) OR (([Extent1].[ParentNullableInt] IS NULL) AND (@p__linq__0 IS NULL))) THEN cast(1 as bit) WHEN ( NOT (([Extent1].[ParentNullableInt] = @p__linq__0) AND ((CASE WHEN ([Extent1].[ParentNullableInt] IS NULL) THEN cast(1 as bit) ELSE cast(0 as bit) END) = (CASE WHEN (@p__linq__0 IS NULL) THEN cast(1 as bit) ELSE cast(0 as bit) END)))) THEN cast(0 as bit) END AS [C1]
    FROM [dbo].[EfParents] AS [Extent1]
```

  * Int Equals Constant (line 83)
     * T-Sql executed is

```SQL
SELECT 
    CASE WHEN (123 = (CASE WHEN ([Extent1].[ParentNullableInt] IS NULL) THEN 0 ELSE [Extent1].[ParentNullableInt] END)) THEN cast(1 as bit) WHEN ( NOT ((123 = (CASE WHEN ([Extent1].[ParentNullableInt] IS NULL) THEN 0 ELSE [Extent1].[ParentNullableInt] END)) AND (CASE WHEN ([Extent1].[ParentNullableInt] IS NULL) THEN 0 ELSE [Extent1].[ParentNullableInt] END IS NOT NULL))) THEN cast(0 as bit) END AS [C1]
    FROM [dbo].[EfParents] AS [Extent1]
```

  * Nullable Init (line 104)
     * T-Sql executed is

```SQL
SELECT 
    CAST(NULL AS int) AS [C1]
    FROM [dbo].[EfParents] AS [Extent1]
```

  * Nullable Add (line 125)
     * T-Sql executed is

```SQL
SELECT 
    [Extent1].[ParentNullableDecimal1] + [Extent1].[ParentNullableDecimal2] AS [C1]
    FROM [dbo].[EfParents] AS [Extent1]
```


#### [Where](../TestGroup05BasicFeatures/Test05Where.cs):
- Supported
  * Where Bool Equals Constant (line 37)
     * T-Sql executed is

```SQL
SELECT 
    [Extent1].[EfParentId] AS [EfParentId]
    FROM [dbo].[EfParents] AS [Extent1]
    WHERE [Extent1].[ParentBool] = 1
```

  * Where Bool Equals Static Variable (line 60)
     * T-Sql executed is

```SQL
SELECT 
    [Extent1].[EfParentId] AS [EfParentId]
    FROM [dbo].[EfParents] AS [Extent1]
    WHERE [Extent1].[ParentBool] = @p__linq__0
```

  * Where Int Equals Constant (line 81)
     * T-Sql executed is

```SQL
SELECT 
    [Extent1].[EfParentId] AS [EfParentId]
    FROM [dbo].[EfParents] AS [Extent1]
    WHERE 123 = [Extent1].[ParentInt]
```

  * Where Filters On Abstract Members Over Tph Hierarchy (line 102)
     * T-Sql executed is

```SQL
SELECT 
    [Extent1].[Id] AS [Id]
    FROM [dbo].[LivingBeeings] AS [Extent1]
    WHERE ([Extent1].[Discriminator] IN (N'Dog',N'HoneyBee',N'Person')) AND (N'Human' = (CASE WHEN ([Extent1].[Discriminator] = N'Person') THEN N'Human' WHEN ([Extent1].[Discriminator] = N'HoneyBee') THEN N'Apis mellifera' WHEN ([Extent1].[Discriminator] = N'Dog') THEN N'Canis lupus' END))
```

  * Where Filters On Multiple Levels Of Abstract Members Over Tph Hierarchy (line 124)
     * T-Sql executed is

```SQL
SELECT 
    [Extent1].[Id] AS [Id]
    FROM [dbo].[LivingBeeings] AS [Extent1]
    WHERE ([Extent1].[Discriminator] IN (N'Dog',N'HoneyBee',N'Person')) AND ([Extent1].[Discriminator] IN (N'Dog',N'HoneyBee')) AND (N'Apis mellifera : False' = (CASE WHEN (CASE WHEN ([Extent1].[Discriminator] = N'HoneyBee') THEN N'Apis mellifera' WHEN ([Extent1].[Discriminator] = N'Dog') THEN N'Canis lupus' END IS NULL) THEN N'' WHEN ([Extent1].[Discriminator] = N'HoneyBee') THEN N'Apis mellifera' WHEN ([Extent1].[Discriminator] = N'Dog') THEN N'Canis lupus' END + N' : ' + CASE WHEN ((CASE WHEN ([Extent1].[Discriminator] = N'HoneyBee') THEN cast(0 as bit) WHEN ([Extent1].[Discriminator] = N'Dog') THEN cast(1 as bit) WHEN ([Extent1].[Discriminator] <> N'Dog') THEN cast(0 as bit) END) = 1) THEN N'True' WHEN ((CASE WHEN ([Extent1].[Discriminator] = N'HoneyBee') THEN cast(0 as bit) WHEN ([Extent1].[Discriminator] = N'Dog') THEN cast(1 as bit) WHEN ([Extent1].[Discriminator] <> N'Dog') THEN cast(0 as bit) END) = 0) THEN N'False' ELSE N'' END))
```


#### [Single](../TestGroup05BasicFeatures/Test10Single.cs):
- Supported
  * Single Int Equals Unique Value (line 42)
     * T-Sql executed is

```SQL
SELECT 
    [Limit1].[EfParentId] AS [EfParentId], 
    [Limit1].[C1] AS [C1]
    FROM ( SELECT TOP (2) 
        [Extent1].[EfParentId] AS [EfParentId], 
        CASE WHEN (987 = [Extent1].[ParentInt]) THEN cast(1 as bit) WHEN (987 <> [Extent1].[ParentInt]) THEN cast(0 as bit) END AS [C1]
        FROM [dbo].[EfParents] AS [Extent1]
        WHERE (CASE WHEN (987 = [Extent1].[ParentInt]) THEN cast(1 as bit) WHEN (987 <> [Extent1].[ParentInt]) THEN cast(0 as bit) END) = 1
    )  AS [Limit1]
```


#### [Single Async](../TestGroup05BasicFeatures/Test11SingleAsync.cs):
- Supported
  * Single Int Equals Unique Value Async (line 48)
     * T-Sql executed is

```SQL
SELECT 
    [Limit1].[EfParentId] AS [EfParentId], 
    [Limit1].[C1] AS [C1]
    FROM ( SELECT TOP (2) 
        [Extent1].[EfParentId] AS [EfParentId], 
        CASE WHEN (987 = [Extent1].[ParentInt]) THEN cast(1 as bit) WHEN (987 <> [Extent1].[ParentInt]) THEN cast(0 as bit) END AS [C1]
        FROM [dbo].[EfParents] AS [Extent1]
        WHERE (CASE WHEN (987 = [Extent1].[ParentInt]) THEN cast(1 as bit) WHEN (987 <> [Extent1].[ParentInt]) THEN cast(0 as bit) END) = 1
    )  AS [Limit1]
```



### Group: Order Take
#### [Order By](../TestGroup10OrderTake/Test01OrderBy.cs):
- Supported
  * Order By Children Count (line 37)
     * T-Sql executed is

```SQL
SELECT 
    [Project1].[EfParentId] AS [EfParentId]
    FROM ( SELECT 
        [Extent1].[EfParentId] AS [EfParentId], 
        (SELECT 
            COUNT(1) AS [A1]
            FROM [dbo].[EfChilds] AS [Extent2]
            WHERE [Extent1].[EfParentId] = [Extent2].[EfParentId]) AS [C1]
        FROM [dbo].[EfParents] AS [Extent1]
    )  AS [Project1]
    ORDER BY [Project1].[C1] ASC
```

  * Order By Children Count Then By String Length (line 59)
     * T-Sql executed is

```SQL
SELECT 
    [Project2].[EfParentId] AS [EfParentId]
    FROM ( SELECT 
         CAST(LEN([Project1].[ParentString]) AS int) AS [C1], 
        [Project1].[EfParentId] AS [EfParentId], 
        [Project1].[C1] AS [C2]
        FROM ( SELECT 
            [Extent1].[EfParentId] AS [EfParentId], 
            [Extent1].[ParentString] AS [ParentString], 
            (SELECT 
                COUNT(1) AS [A1]
                FROM [dbo].[EfChilds] AS [Extent2]
                WHERE [Extent1].[EfParentId] = [Extent2].[EfParentId]) AS [C1]
            FROM [dbo].[EfParents] AS [Extent1]
        )  AS [Project1]
    )  AS [Project2]
    ORDER BY [Project2].[C2] ASC, [Project2].[C1] ASC
```

  * Where Any Children Then Order By Children Count (line 81)
     * T-Sql executed is

```SQL
SELECT 
    [Project2].[EfParentId] AS [EfParentId]
    FROM ( SELECT 
        [Extent1].[EfParentId] AS [EfParentId], 
        (SELECT 
            COUNT(1) AS [A1]
            FROM [dbo].[EfChilds] AS [Extent3]
            WHERE [Extent1].[EfParentId] = [Extent3].[EfParentId]) AS [C1]
        FROM [dbo].[EfParents] AS [Extent1]
        WHERE  EXISTS (SELECT 
            1 AS [C1]
            FROM [dbo].[EfChilds] AS [Extent2]
            WHERE [Extent1].[EfParentId] = [Extent2].[EfParentId]
        )
    )  AS [Project2]
    ORDER BY [Project2].[C1] ASC
```


#### [Skip Take](../TestGroup10OrderTake/Test02SkipTake.cs):
- Supported
  * Order By Children Count Then Take (line 37)
     * T-Sql executed is

```SQL
SELECT TOP (2) 
    [Project1].[EfParentId] AS [EfParentId]
    FROM ( SELECT 
        [Extent1].[EfParentId] AS [EfParentId], 
        (SELECT 
            COUNT(1) AS [A1]
            FROM [dbo].[EfChilds] AS [Extent2]
            WHERE [Extent1].[EfParentId] = [Extent2].[EfParentId]) AS [C1]
        FROM [dbo].[EfParents] AS [Extent1]
    )  AS [Project1]
    ORDER BY [Project1].[C1] ASC
```

  * Order By Children Count Then Skip And Take (line 59)
     * T-Sql executed is

```SQL
SELECT 
    [Project1].[EfParentId] AS [EfParentId]
    FROM ( SELECT 
        [Extent1].[EfParentId] AS [EfParentId], 
        (SELECT 
            COUNT(1) AS [A1]
            FROM [dbo].[EfChilds] AS [Extent2]
            WHERE [Extent1].[EfParentId] = [Extent2].[EfParentId]) AS [C1]
        FROM [dbo].[EfParents] AS [Extent1]
    )  AS [Project1]
    ORDER BY row_number() OVER (ORDER BY [Project1].[C1] ASC)
    OFFSET 1 ROWS FETCH NEXT 2 ROWS ONLY 
```

  * Where Any Children Then Order By Then Skip Take (line 81)
     * T-Sql executed is

```SQL
SELECT 
    [Project2].[EfParentId] AS [EfParentId]
    FROM ( SELECT 
        [Extent1].[EfParentId] AS [EfParentId], 
        (SELECT 
            COUNT(1) AS [A1]
            FROM [dbo].[EfChilds] AS [Extent3]
            WHERE [Extent1].[EfParentId] = [Extent3].[EfParentId]) AS [C1]
        FROM [dbo].[EfParents] AS [Extent1]
        WHERE  EXISTS (SELECT 
            1 AS [C1]
            FROM [dbo].[EfChilds] AS [Extent2]
            WHERE [Extent1].[EfParentId] = [Extent2].[EfParentId]
        )
    )  AS [Project2]
    ORDER BY row_number() OVER (ORDER BY [Project2].[C1] ASC)
    OFFSET 1 ROWS FETCH NEXT 1 ROWS ONLY 
```



### Group: Quantifier Operators
#### [Any](../TestGroup12QuantifierOperators/Test01Any.cs):
- Supported
  * Any Children (line 36)
     * T-Sql executed is

```SQL
SELECT 
    CASE WHEN ( EXISTS (SELECT 
        1 AS [C1]
        FROM [dbo].[EfChilds] AS [Extent2]
        WHERE [Extent1].[EfParentId] = [Extent2].[EfParentId]
    )) THEN cast(1 as bit) ELSE cast(0 as bit) END AS [C1]
    FROM [dbo].[EfParents] AS [Extent1]
```

  * Any Children With Filter (line 57)
     * T-Sql executed is

```SQL
SELECT 
    CASE WHEN ( EXISTS (SELECT 
        1 AS [C1]
        FROM [dbo].[EfChilds] AS [Extent2]
        WHERE ([Extent1].[EfParentId] = [Extent2].[EfParentId]) AND (123 = [Extent2].[ChildInt])
    )) THEN cast(1 as bit) ELSE cast(0 as bit) END AS [C1]
    FROM [dbo].[EfParents] AS [Extent1]
```


#### [All](../TestGroup12QuantifierOperators/Test02All.cs):
- Supported
  * Singleton All Filter (line 36)
     * T-Sql executed is

```SQL
SELECT 
    CASE WHEN ( NOT EXISTS (SELECT 
        1 AS [C1]
        FROM [dbo].[EfParents] AS [Extent1]
        WHERE (123 <> [Extent1].[ParentInt]) OR (CASE WHEN (123 = [Extent1].[ParentInt]) THEN cast(1 as bit) WHEN (123 <> [Extent1].[ParentInt]) THEN cast(0 as bit) END IS NULL)
    )) THEN cast(1 as bit) ELSE cast(0 as bit) END AS [C1]
    FROM  ( SELECT 1 AS X ) AS [SingleRowTable1]
```

  * All Filter On Children Int (line 57)
     * T-Sql executed is

```SQL
SELECT 
    CASE WHEN ( NOT EXISTS (SELECT 
        1 AS [C1]
        FROM [dbo].[EfChilds] AS [Extent2]
        WHERE ([Extent1].[EfParentId] = [Extent2].[EfParentId]) AND ((123 <> [Extent2].[ChildInt]) OR (CASE WHEN (123 = [Extent2].[ChildInt]) THEN cast(1 as bit) WHEN (123 <> [Extent2].[ChildInt]) THEN cast(0 as bit) END IS NULL))
    )) THEN cast(1 as bit) ELSE cast(0 as bit) END AS [C1]
    FROM [dbo].[EfParents] AS [Extent1]
```


#### [Contains](../TestGroup12QuantifierOperators/Test03Contains.cs):
- Supported
  * String Contains Constant String With Filter (line 37)
     * T-Sql executed is

```SQL
SELECT 
    [Extent1].[ParentString] AS [ParentString]
    FROM [dbo].[EfParents] AS [Extent1]
    WHERE [Extent1].[ParentString] LIKE N'%2%'
```



### Group: Aggregation
#### [Count](../TestGroup15Aggregation/Test01Count.cs):
- Supported
  * Count Children (line 37)
     * T-Sql executed is

```SQL
SELECT 
    (SELECT 
        COUNT(1) AS [A1]
        FROM [dbo].[EfChilds] AS [Extent2]
        WHERE [Extent1].[EfParentId] = [Extent2].[EfParentId]) AS [C1]
    FROM [dbo].[EfParents] AS [Extent1]
```

  * Count Children With Filter (line 59)
     * T-Sql executed is

```SQL
SELECT 
    (SELECT 
        COUNT(1) AS [A1]
        FROM [dbo].[EfChilds] AS [Extent2]
        WHERE ([Extent1].[EfParentId] = [Extent2].[EfParentId]) AND (123 = [Extent2].[ChildInt])) AS [C1]
    FROM [dbo].[EfParents] AS [Extent1]
```

  * Count Children With Filter By Closure (line 81)
     * T-Sql executed is

```SQL
SELECT 
    (SELECT 
        COUNT(1) AS [A1]
        FROM [dbo].[EfChilds] AS [Extent2]
        WHERE ([Extent1].[EfParentId] = [Extent2].[EfParentId]) AND ([Extent2].[ChildInt] = [Extent1].[ParentInt])) AS [C1]
    FROM [dbo].[EfParents] AS [Extent1]
```

  * Count Children With Filter By External Closure (line 104)
     * T-Sql executed is

```SQL
SELECT 
    [Project1].[C1] AS [C1]
    FROM ( SELECT 
        (SELECT 
            COUNT(1) AS [A1]
            FROM [dbo].[EfChilds] AS [Extent2]
            WHERE ([Extent1].[EfParentId] = [Extent2].[EfParentId]) AND ([Extent2].[ChildInt] = @p__linq__0)) AS [C1]
        FROM [dbo].[EfParents] AS [Extent1]
    )  AS [Project1]
```

  * Count Children With Filter By External Closure2 (line 128)
     * T-Sql executed is

```SQL
SELECT 
    [Project1].[C1] AS [C1]
    FROM ( SELECT 
        (SELECT 
            COUNT(1) AS [A1]
            FROM [dbo].[EfChilds] AS [Extent2]
            WHERE ([Extent1].[EfParentId] = [Extent2].[EfParentId]) AND ([Extent2].[ChildInt] = @p__linq__0) AND ([Extent2].[EfParentId] = @p__linq__1)) AS [C1]
        FROM [dbo].[EfParents] AS [Extent1]
    )  AS [Project1]
```

  * Singleton Count Children With Filter (line 150)
     * T-Sql executed is

```SQL
SELECT 
    [GroupBy2].[A1] AS [C1]
    FROM ( SELECT 
        COUNT(1) AS [A1]
        FROM ( SELECT 
            (SELECT 
                COUNT(1) AS [A1]
                FROM [dbo].[EfChilds] AS [Extent2]
                WHERE [Extent1].[EfParentId] = [Extent2].[EfParentId]) AS [C1]
            FROM [dbo].[EfParents] AS [Extent1]
        )  AS [Project1]
        WHERE 2 = [Project1].[C1]
    )  AS [GroupBy2]
```


#### [Sum](../TestGroup15Aggregation/Test02Sum.cs):
- Supported
  * Singleton Sum Children (line 38)
     * T-Sql executed is

```SQL
SELECT 
    [GroupBy2].[A1] AS [C1]
    FROM ( SELECT 
        SUM([Extent1].[A1_0]) AS [A1]
        FROM ( SELECT 
            (SELECT 
                COUNT(1) AS [A1]
                FROM [dbo].[EfChilds] AS [Extent2]
                WHERE [Extent1].[EfParentId] = [Extent2].[EfParentId]) AS [A1_0]
            FROM [dbo].[EfParents] AS [Extent1]
        )  AS [Extent1]
    )  AS [GroupBy2]
```

  * Sum Count In Children Where Children Can Be None (line 61)
     * T-Sql executed is

```SQL
SELECT 
    CASE WHEN ([Project2].[C1] IS NULL) THEN 0 ELSE [Project2].[C2] END AS [C1]
    FROM ( SELECT 
        [Project1].[C1] AS [C1], 
        (SELECT 
            SUM([Extent3].[ChildInt]) AS [A1]
            FROM [dbo].[EfChilds] AS [Extent3]
            WHERE [Project1].[EfParentId] = [Extent3].[EfParentId]) AS [C2]
        FROM ( SELECT 
            [Extent1].[EfParentId] AS [EfParentId], 
            (SELECT 
                SUM([Extent2].[ChildInt]) AS [A1]
                FROM [dbo].[EfChilds] AS [Extent2]
                WHERE [Extent1].[EfParentId] = [Extent2].[EfParentId]) AS [C1]
            FROM [dbo].[EfParents] AS [Extent1]
        )  AS [Project1]
    )  AS [Project2]
```


#### [Count Async](../TestGroup15Aggregation/Test03CountAsync.cs):
- Supported
  * Count Children Async (line 44)
     * T-Sql executed is

```SQL
SELECT 
    (SELECT 
        COUNT(1) AS [A1]
        FROM [dbo].[EfChilds] AS [Extent2]
        WHERE [Extent1].[EfParentId] = [Extent2].[EfParentId]) AS [C1]
    FROM [dbo].[EfParents] AS [Extent1]
```

  * Count Children With Filter Async (line 66)
     * T-Sql executed is

```SQL
SELECT 
    (SELECT 
        COUNT(1) AS [A1]
        FROM [dbo].[EfChilds] AS [Extent2]
        WHERE ([Extent1].[EfParentId] = [Extent2].[EfParentId]) AND (123 = [Extent2].[ChildInt])) AS [C1]
    FROM [dbo].[EfParents] AS [Extent1]
```

  * Count Children With Filter By Closure Async (line 88)
     * T-Sql executed is

```SQL
SELECT 
    (SELECT 
        COUNT(1) AS [A1]
        FROM [dbo].[EfChilds] AS [Extent2]
        WHERE ([Extent1].[EfParentId] = [Extent2].[EfParentId]) AND ([Extent2].[ChildInt] = [Extent1].[ParentInt])) AS [C1]
    FROM [dbo].[EfParents] AS [Extent1]
```

  * Count Children With Filter By External Closure Async (line 111)
     * T-Sql executed is

```SQL
SELECT 
    [Project1].[C1] AS [C1]
    FROM ( SELECT 
        (SELECT 
            COUNT(1) AS [A1]
            FROM [dbo].[EfChilds] AS [Extent2]
            WHERE ([Extent1].[EfParentId] = [Extent2].[EfParentId]) AND ([Extent2].[ChildInt] = @p__linq__0)) AS [C1]
        FROM [dbo].[EfParents] AS [Extent1]
    )  AS [Project1]
```

  * Count Children With Filter By External Closure2 Async (line 135)
     * T-Sql executed is

```SQL
SELECT 
    [Project1].[C1] AS [C1]
    FROM ( SELECT 
        (SELECT 
            COUNT(1) AS [A1]
            FROM [dbo].[EfChilds] AS [Extent2]
            WHERE ([Extent1].[EfParentId] = [Extent2].[EfParentId]) AND ([Extent2].[ChildInt] = @p__linq__0) AND ([Extent2].[EfParentId] = @p__linq__1)) AS [C1]
        FROM [dbo].[EfParents] AS [Extent1]
    )  AS [Project1]
```

  * Singleton Count Children With Filter Async (line 157)
     * T-Sql executed is

```SQL
SELECT 
    [GroupBy2].[A1] AS [C1]
    FROM ( SELECT 
        COUNT(1) AS [A1]
        FROM ( SELECT 
            (SELECT 
                COUNT(1) AS [A1]
                FROM [dbo].[EfChilds] AS [Extent2]
                WHERE [Extent1].[EfParentId] = [Extent2].[EfParentId]) AS [C1]
            FROM [dbo].[EfParents] AS [Extent1]
        )  AS [Project1]
        WHERE 2 = [Project1].[C1]
    )  AS [GroupBy2]
```



### Group: Types
#### [Strings](../TestGroup50Types/Test01Strings.cs):
- Supported
  * Concatenate Person Not Handle Null (line 36)
     * T-Sql executed is

```SQL
SELECT 
    [Extent1].[FirstName] + N' ' + CASE WHEN ([Extent1].[MiddleName] IS NULL) THEN N'' ELSE [Extent1].[MiddleName] END + N' ' + [Extent1].[LastName] AS [C1]
    FROM [dbo].[EfPersons] AS [Extent1]
```

  * Concatenate Person Handle Null (line 57)
     * T-Sql executed is

```SQL
SELECT 
    [Extent1].[FirstName] + CASE WHEN (CASE WHEN ([Extent1].[MiddleName] IS NULL) THEN N'' ELSE N' ' END IS NULL) THEN N'' WHEN ([Extent1].[MiddleName] IS NULL) THEN N'' ELSE N' ' END + CASE WHEN ([Extent1].[MiddleName] IS NULL) THEN N'' ELSE [Extent1].[MiddleName] END + N' ' + [Extent1].[LastName] AS [C1]
    FROM [dbo].[EfPersons] AS [Extent1]
```

  * Concatenate Person Handle Name Order (line 80)
     * T-Sql executed is

```SQL
SELECT 
    CASE WHEN ([Extent1].[NameOrder] = 1) THEN [Extent1].[LastName] + N', ' + [Extent1].[FirstName] + CASE WHEN (CASE WHEN ([Extent1].[MiddleName] IS NULL) THEN N'' ELSE N' ' END IS NULL) THEN N'' WHEN ([Extent1].[MiddleName] IS NULL) THEN N'' ELSE N' ' END ELSE [Extent1].[FirstName] + CASE WHEN (CASE WHEN ([Extent1].[MiddleName] IS NULL) THEN N'' ELSE N' ' END IS NULL) THEN N'' WHEN ([Extent1].[MiddleName] IS NULL) THEN N'' ELSE N' ' END + CASE WHEN ([Extent1].[MiddleName] IS NULL) THEN N'' ELSE [Extent1].[MiddleName] END + N' ' + [Extent1].[LastName] END AS [C1]
    FROM [dbo].[EfPersons] AS [Extent1]
```

  * Select Generic Method Person Handle (line 101)
     * T-Sql executed is

```SQL
SELECT 
    [Extent1].[FirstName] + CASE WHEN (CASE WHEN ([Extent1].[MiddleName] IS NULL) THEN N'' ELSE N' ' END IS NULL) THEN N'' WHEN ([Extent1].[MiddleName] IS NULL) THEN N'' ELSE N' ' END + CASE WHEN ([Extent1].[MiddleName] IS NULL) THEN N'' ELSE [Extent1].[MiddleName] END + N' ' + [Extent1].[LastName] AS [C1]
    FROM [dbo].[EfPersons] AS [Extent1]
```

  * Filter Generic Method Person Handle (line 119)
     * T-Sql executed is

```SQL
SELECT 
    [Extent1].[EfPersonId] AS [EfPersonId], 
    [Extent1].[FirstName] AS [FirstName], 
    [Extent1].[MiddleName] AS [MiddleName], 
    [Extent1].[LastName] AS [LastName], 
    [Extent1].[NameOrder] AS [NameOrder]
    FROM [dbo].[EfPersons] AS [Extent1]
    WHERE [Extent1].[FirstName] + CASE WHEN (CASE WHEN ([Extent1].[MiddleName] IS NULL) THEN N'' ELSE N' ' END IS NULL) THEN N'' WHEN ([Extent1].[MiddleName] IS NULL) THEN N'' ELSE N' ' END + CASE WHEN ([Extent1].[MiddleName] IS NULL) THEN N'' ELSE [Extent1].[MiddleName] END + N' ' + [Extent1].[LastName] IS NOT NULL
```


#### [DateTime](../TestGroup50Types/Test05DateTime.cs):
- Supported
  * DateTime Where Compare With Static Variable (line 39)
     * T-Sql executed is

```SQL
SELECT 
    [Extent1].[StartDate] AS [StartDate]
    FROM [dbo].[EfParents] AS [Extent1]
    WHERE [Extent1].[StartDate] > @p__linq__0
```



### Group: Additional Features
#### [Nested Expressions](../TestGroup90AdditionalFeatures/Test01NestedExpressions.cs):
- Supported
  * Subquery As Context Extension Method (line 72)
     * T-Sql executed is

```SQL
SELECT 
    [Project4].[EfParentId] AS [EfParentId], 
    CASE WHEN ([Project4].[C1] IS NULL) THEN 0 ELSE [Project4].[C2] END AS [C1]
    FROM ( SELECT 
        [Project2].[EfParentId] AS [EfParentId], 
        [Project2].[C1] AS [C1], 
        (SELECT TOP (1) 
            [Extent3].[EfChildId] AS [EfChildId]
            FROM [dbo].[EfChilds] AS [Extent3]
            WHERE [Extent3].[EfParentId] = [Project2].[EfParentId]) AS [C2]
        FROM ( SELECT 
            [Extent1].[EfParentId] AS [EfParentId], 
            (SELECT TOP (1) 
                [Extent2].[EfChildId] AS [EfChildId]
                FROM [dbo].[EfChilds] AS [Extent2]
                WHERE [Extent2].[EfParentId] = [Extent1].[EfParentId]) AS [C1]
            FROM [dbo].[EfParents] AS [Extent1]
        )  AS [Project2]
    )  AS [Project4]
```




The End
