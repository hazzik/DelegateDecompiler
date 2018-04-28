Detail With Sql of supported commands
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
     * T-Sql executed is

```SQL
SELECT 
    [Extent1].[ParentBool] AS [ParentBool]
    FROM [dbo].[EfParents] AS [Extent1]
```

  * Bool Equals Static Variable (line 53)
     * T-Sql executed is

```SQL
SELECT 
    CASE WHEN ([Extent1].[ParentBool] = @p__linq__0) THEN cast(1 as bit) WHEN ([Extent1].[ParentBool] <> @p__linq__0) THEN cast(0 as bit) END AS [C1]
    FROM [dbo].[EfParents] AS [Extent1]
```

  * Int Equals Constant (line 70)
     * T-Sql executed is

```SQL
SELECT 
    CASE WHEN (123 = [Extent1].[ParentInt]) THEN cast(1 as bit) WHEN (123 <> [Extent1].[ParentInt]) THEN cast(0 as bit) END AS [C1]
    FROM [dbo].[EfParents] AS [Extent1]
```

  * Select Property Without Computed Attribute (line 87)
     * T-Sql executed is

```SQL
SELECT 
    [Extent1].[FirstName] + N' ' + CASE WHEN ([Extent1].[MiddleName] IS NULL) THEN N'' ELSE [Extent1].[MiddleName] END + N' ' + [Extent1].[LastName] AS [C1]
    FROM [dbo].[EfPersons] AS [Extent1]
```

  * Select Method Without Computed Attribute (line 104)
     * T-Sql executed is

```SQL
SELECT 
    [Extent1].[FirstName] + N' ' + CASE WHEN ([Extent1].[MiddleName] IS NULL) THEN N'' ELSE [Extent1].[MiddleName] END + N' ' + [Extent1].[LastName] AS [C1]
    FROM [dbo].[EfPersons] AS [Extent1]
```

  * Select Abstract Member Over Tph Hierarchy (line 121)
     * T-Sql executed is

```SQL
SELECT 
    CASE WHEN ([Extent1].[Discriminator] = N'Person') THEN N'Human' WHEN ([Extent1].[Discriminator] = N'HoneyBee') THEN N'Apis mellifera' WHEN ([Extent1].[Discriminator] = N'Dog') THEN N'Canis lupus' END AS [C1]
    FROM [dbo].[LivingBeeings] AS [Extent1]
    WHERE [Extent1].[Discriminator] IN (N'Dog',N'HoneyBee',N'Person')
```

  * Select Abstract Member Over Tph Hierarchy After Restricting To Subtype (line 138)
     * T-Sql executed is

```SQL
SELECT 
    CASE WHEN ([Extent1].[Discriminator] = N'HoneyBee') THEN N'Apis mellifera' WHEN ([Extent1].[Discriminator] = N'Dog') THEN N'Canis lupus' END AS [C1]
    FROM [dbo].[LivingBeeings] AS [Extent1]
    WHERE ([Extent1].[Discriminator] IN (N'Dog',N'HoneyBee',N'Person')) AND ([Extent1].[Discriminator] IN (N'Dog',N'HoneyBee'))
```

  * Select Multiple Levels Of Abstract Members Over Tph Hierarchy (line 155)
     * T-Sql executed is

```SQL
SELECT 
    CASE WHEN (CASE WHEN ([Extent1].[Discriminator] = N'HoneyBee') THEN N'Apis mellifera' WHEN ([Extent1].[Discriminator] = N'Dog') THEN N'Canis lupus' END IS NULL) THEN N'' WHEN ([Extent1].[Discriminator] = N'HoneyBee') THEN N'Apis mellifera' WHEN ([Extent1].[Discriminator] = N'Dog') THEN N'Canis lupus' END + N' : ' + CASE WHEN ((CASE WHEN ([Extent1].[Discriminator] = N'HoneyBee') THEN cast(0 as bit) WHEN ([Extent1].[Discriminator] = N'Dog') THEN cast(1 as bit) ELSE cast(0 as bit) END) = 1) THEN N'True' WHEN ((CASE WHEN ([Extent1].[Discriminator] = N'HoneyBee') THEN cast(0 as bit) WHEN ([Extent1].[Discriminator] = N'Dog') THEN cast(1 as bit) ELSE cast(0 as bit) END) = 0) THEN N'False' ELSE N'' END AS [C1]
    FROM [dbo].[LivingBeeings] AS [Extent1]
    WHERE ([Extent1].[Discriminator] IN (N'Dog',N'HoneyBee',N'Person')) AND ([Extent1].[Discriminator] IN (N'Dog',N'HoneyBee'))
```


#### [Select Async](../TestGroup05BasicFeatures/Test02SelectAsync.cs):
- Supported
  * Bool Equals Constant Async (line 34)
     * T-Sql executed is

```SQL
SELECT 
    [Extent1].[ParentBool] AS [ParentBool]
    FROM [dbo].[EfParents] AS [Extent1]
```

  * Bool Equals Static Variable To Array Async (line 53)
     * T-Sql executed is

```SQL
SELECT 
    CASE WHEN ([Extent1].[ParentBool] = @p__linq__0) THEN cast(1 as bit) WHEN ([Extent1].[ParentBool] <> @p__linq__0) THEN cast(0 as bit) END AS [C1]
    FROM [dbo].[EfParents] AS [Extent1]
```

  * Int Equals Constant (line 70)
     * T-Sql executed is

```SQL
SELECT 
    CASE WHEN (123 = [Extent1].[ParentInt]) THEN cast(1 as bit) WHEN (123 <> [Extent1].[ParentInt]) THEN cast(0 as bit) END AS [C1]
    FROM [dbo].[EfParents] AS [Extent1]
```


#### [Equals And Not Equals](../TestGroup05BasicFeatures/Test03EqualsAndNotEquals.cs):
- Supported
  * Int Equals Constant (line 32)
     * T-Sql executed is

```SQL
SELECT 
    CASE WHEN (123 = [Extent1].[ParentInt]) THEN cast(1 as bit) WHEN (123 <> [Extent1].[ParentInt]) THEN cast(0 as bit) END AS [C1]
    FROM [dbo].[EfParents] AS [Extent1]
```

  * Int Equals Static Variable (line 50)
     * T-Sql executed is

```SQL
SELECT 
    CASE WHEN ([Extent1].[ParentInt] = @p__linq__0) THEN cast(1 as bit) WHEN ([Extent1].[ParentInt] <> @p__linq__0) THEN cast(0 as bit) END AS [C1]
    FROM [dbo].[EfParents] AS [Extent1]
```

  * Int Equals String Length (line 67)
     * T-Sql executed is

```SQL
SELECT 
    CASE WHEN ([Extent1].[ParentInt] = ( CAST(LEN([Extent1].[ParentString]) AS int))) THEN cast(1 as bit) WHEN ( NOT (([Extent1].[ParentInt] = ( CAST(LEN([Extent1].[ParentString]) AS int))) AND (0 = (CASE WHEN ([Extent1].[ParentString] IS NULL) THEN cast(1 as bit) ELSE cast(0 as bit) END)))) THEN cast(0 as bit) END AS [C1]
    FROM [dbo].[EfParents] AS [Extent1]
```

  * Int Not Equals String Length (line 84)
     * T-Sql executed is

```SQL
SELECT 
    CASE WHEN ( NOT (([Extent1].[ParentInt] = ( CAST(LEN([Extent1].[ParentString]) AS int))) AND (0 = (CASE WHEN ([Extent1].[ParentString] IS NULL) THEN cast(1 as bit) ELSE cast(0 as bit) END)))) THEN cast(1 as bit) WHEN ([Extent1].[ParentInt] = ( CAST(LEN([Extent1].[ParentString]) AS int))) THEN cast(0 as bit) END AS [C1]
    FROM [dbo].[EfParents] AS [Extent1]
```


#### [Nullable](../TestGroup05BasicFeatures/Test04Nullable.cs):
- Supported
  * Property Is Null (line 35)
     * T-Sql executed is

```SQL
SELECT 
    CASE WHEN ([Extent1].[ParentNullableInt] IS NULL) THEN cast(1 as bit) ELSE cast(0 as bit) END AS [C1]
    FROM [dbo].[EfParents] AS [Extent1]
```

  * Bool Equals Static Variable (line 54)
     * T-Sql executed is

```SQL
SELECT 
    CASE WHEN (([Extent1].[ParentNullableInt] = @p__linq__0) OR (([Extent1].[ParentNullableInt] IS NULL) AND (@p__linq__0 IS NULL))) THEN cast(1 as bit) WHEN ( NOT (([Extent1].[ParentNullableInt] = @p__linq__0) AND ((CASE WHEN ([Extent1].[ParentNullableInt] IS NULL) THEN cast(1 as bit) ELSE cast(0 as bit) END) = (CASE WHEN (@p__linq__0 IS NULL) THEN cast(1 as bit) ELSE cast(0 as bit) END)))) THEN cast(0 as bit) END AS [C1]
    FROM [dbo].[EfParents] AS [Extent1]
```

  * Int Equals Constant (line 71)
     * T-Sql executed is

```SQL
SELECT 
    CASE WHEN (123 = [Extent1].[ParentNullableInt]) THEN cast(1 as bit) WHEN ( NOT ((123 = [Extent1].[ParentNullableInt]) AND ([Extent1].[ParentNullableInt] IS NOT NULL))) THEN cast(0 as bit) END AS [C1]
    FROM [dbo].[EfParents] AS [Extent1]
```

  * Nullable Init (line 88)
     * T-Sql executed is

```SQL
SELECT 
    CAST(NULL AS int) AS [C1]
    FROM [dbo].[EfParents] AS [Extent1]
```

  * Nullable Add (line 105)
     * T-Sql executed is

```SQL
SELECT 
    [Extent1].[ParentNullableDecimal1] + [Extent1].[ParentNullableDecimal2] AS [C1]
    FROM [dbo].[EfParents] AS [Extent1]
```


#### [Where](../TestGroup05BasicFeatures/Test05Where.cs):
- Supported
  * Where Bool Equals Constant (line 33)
     * T-Sql executed is

```SQL
SELECT 
    [Extent1].[EfParentId] AS [EfParentId]
    FROM [dbo].[EfParents] AS [Extent1]
    WHERE [Extent1].[ParentBool] = 1
```

  * Where Bool Equals Static Variable (line 52)
     * T-Sql executed is

```SQL
SELECT 
    [Extent1].[EfParentId] AS [EfParentId]
    FROM [dbo].[EfParents] AS [Extent1]
    WHERE [Extent1].[ParentBool] = @p__linq__0
```

  * Where Int Equals Constant (line 69)
     * T-Sql executed is

```SQL
SELECT 
    [Extent1].[EfParentId] AS [EfParentId]
    FROM [dbo].[EfParents] AS [Extent1]
    WHERE 123 = [Extent1].[ParentInt]
```

  * Where Filters On Abstract Members Over Tph Hierarchy (line 86)
     * T-Sql executed is

```SQL
SELECT 
    [Extent1].[Id] AS [Id]
    FROM [dbo].[LivingBeeings] AS [Extent1]
    WHERE ([Extent1].[Discriminator] IN (N'Dog',N'HoneyBee',N'Person')) AND (N'Human' = (CASE WHEN ([Extent1].[Discriminator] = N'Person') THEN N'Human' WHEN ([Extent1].[Discriminator] = N'HoneyBee') THEN N'Apis mellifera' WHEN ([Extent1].[Discriminator] = N'Dog') THEN N'Canis lupus' END))
```

  * Where Filters On Multiple Levels Of Abstract Members Over Tph Hierarchy (line 103)
     * T-Sql executed is

```SQL
SELECT 
    [Extent1].[Id] AS [Id]
    FROM [dbo].[LivingBeeings] AS [Extent1]
    WHERE ([Extent1].[Discriminator] IN (N'Dog',N'HoneyBee',N'Person')) AND ([Extent1].[Discriminator] IN (N'Dog',N'HoneyBee')) AND (N'Apis mellifera : False' = (CASE WHEN (CASE WHEN ([Extent1].[Discriminator] = N'HoneyBee') THEN N'Apis mellifera' WHEN ([Extent1].[Discriminator] = N'Dog') THEN N'Canis lupus' END IS NULL) THEN N'' WHEN ([Extent1].[Discriminator] = N'HoneyBee') THEN N'Apis mellifera' WHEN ([Extent1].[Discriminator] = N'Dog') THEN N'Canis lupus' END + N' : ' + CASE WHEN ((CASE WHEN ([Extent1].[Discriminator] = N'HoneyBee') THEN cast(0 as bit) WHEN ([Extent1].[Discriminator] = N'Dog') THEN cast(1 as bit) ELSE cast(0 as bit) END) = 1) THEN N'True' WHEN ((CASE WHEN ([Extent1].[Discriminator] = N'HoneyBee') THEN cast(0 as bit) WHEN ([Extent1].[Discriminator] = N'Dog') THEN cast(1 as bit) ELSE cast(0 as bit) END) = 0) THEN N'False' ELSE N'' END))
```


#### [Single](../TestGroup05BasicFeatures/Test10Single.cs):
- Supported
  * Single Int Equals Unique Value (line 40)
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
  * Single Int Equals Unique Value Async (line 41)
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
  * Order By Children Count (line 33)
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

  * Order By Children Count Then By String Length (line 51)
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

  * Where Any Children Then Order By Children Count (line 69)
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
  * Order By Children Count Then Take (line 33)
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

  * Order By Children Count Then Skip And Take (line 51)
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
    OFFSET 1 ROWS FETCH NEXT 2 ROWS ONLY 
```

  * Where Any Children Then Order By Then Skip Take (line 69)
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
    OFFSET 1 ROWS FETCH NEXT 1 ROWS ONLY 
```



### Group: Quantifier Operators
#### [Any](../TestGroup12QuantifierOperators/Test01Any.cs):
- Supported
  * Any Children (line 32)
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

  * Any Children With Filter (line 49)
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
  * Singleton All Filter (line 32)
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

  * All Filter On Children Int (line 49)
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
  * String Contains Constant String With Filter (line 33)
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
  * Count Children (line 33)
     * T-Sql executed is

```SQL
SELECT 
    (SELECT 
        COUNT(1) AS [A1]
        FROM [dbo].[EfChilds] AS [Extent2]
        WHERE [Extent1].[EfParentId] = [Extent2].[EfParentId]) AS [C1]
    FROM [dbo].[EfParents] AS [Extent1]
```

  * Count Children With Filter (line 51)
     * T-Sql executed is

```SQL
SELECT 
    (SELECT 
        COUNT(1) AS [A1]
        FROM [dbo].[EfChilds] AS [Extent2]
        WHERE ([Extent1].[EfParentId] = [Extent2].[EfParentId]) AND (123 = [Extent2].[ChildInt])) AS [C1]
    FROM [dbo].[EfParents] AS [Extent1]
```

  * Count Children With Filter By Closure (line 69)
     * T-Sql executed is

```SQL
SELECT 
    (SELECT 
        COUNT(1) AS [A1]
        FROM [dbo].[EfChilds] AS [Extent2]
        WHERE ([Extent1].[EfParentId] = [Extent2].[EfParentId]) AND ([Extent2].[ChildInt] = [Extent1].[ParentInt])) AS [C1]
    FROM [dbo].[EfParents] AS [Extent1]
```

  * Count Children With Filter By External Closure (line 88)
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

  * Count Children With Filter By External Closure2 (line 108)
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

  * Singleton Count Children With Filter (line 126)
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
  * Singleton Sum Children (line 33)
     * T-Sql executed is

```SQL
SELECT 
    [GroupBy2].[A1] AS [C1]
    FROM ( SELECT 
        SUM([Extent1].[A1]) AS [A1]
        FROM ( SELECT 
            (SELECT 
                COUNT(1) AS [A1]
                FROM [dbo].[EfChilds] AS [Extent2]
                WHERE [Extent1].[EfParentId] = [Extent2].[EfParentId]) AS [A1]
            FROM [dbo].[EfParents] AS [Extent1]
        )  AS [Extent1]
    )  AS [GroupBy2]
```

  * Sum Count In Children Where Children Can Be None (line 51)
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



### Group: Types
#### [Strings](../TestGroup50Types/Test01Strings.cs):
- Supported
  * Concatenate Person Not Handle Null (line 32)
     * T-Sql executed is

```SQL
SELECT 
    [Extent1].[FirstName] + N' ' + CASE WHEN ([Extent1].[MiddleName] IS NULL) THEN N'' ELSE [Extent1].[MiddleName] END + N' ' + [Extent1].[LastName] AS [C1]
    FROM [dbo].[EfPersons] AS [Extent1]
```

  * Concatenate Person Handle Null (line 49)
     * T-Sql executed is

```SQL
SELECT 
    [Extent1].[FirstName] + CASE WHEN (CASE WHEN ([Extent1].[MiddleName] IS NULL) THEN N'' ELSE N' ' END IS NULL) THEN N'' WHEN ([Extent1].[MiddleName] IS NULL) THEN N'' ELSE N' ' END + CASE WHEN ([Extent1].[MiddleName] IS NULL) THEN N'' ELSE [Extent1].[MiddleName] END + N' ' + [Extent1].[LastName] AS [C1]
    FROM [dbo].[EfPersons] AS [Extent1]
```

  * Concatenate Person Handle Name Order (line 68)
     * T-Sql executed is

```SQL
SELECT 
    CASE WHEN ([Extent1].[NameOrder] = 1) THEN [Extent1].[LastName] + N', ' + [Extent1].[FirstName] + CASE WHEN (CASE WHEN ([Extent1].[MiddleName] IS NULL) THEN N'' ELSE N' ' END IS NULL) THEN N'' WHEN ([Extent1].[MiddleName] IS NULL) THEN N'' ELSE N' ' END ELSE [Extent1].[FirstName] + CASE WHEN (CASE WHEN ([Extent1].[MiddleName] IS NULL) THEN N'' ELSE N' ' END IS NULL) THEN N'' WHEN ([Extent1].[MiddleName] IS NULL) THEN N'' ELSE N' ' END + CASE WHEN ([Extent1].[MiddleName] IS NULL) THEN N'' ELSE [Extent1].[MiddleName] END + N' ' + [Extent1].[LastName] END AS [C1]
    FROM [dbo].[EfPersons] AS [Extent1]
```

  * Generic Method Person Handle (line 85)
     * T-Sql executed is

```SQL
SELECT 
    [Extent1].[FirstName] + CASE WHEN (CASE WHEN ([Extent1].[MiddleName] IS NULL) THEN N'' ELSE N' ' END IS NULL) THEN N'' WHEN ([Extent1].[MiddleName] IS NULL) THEN N'' ELSE N' ' END + CASE WHEN ([Extent1].[MiddleName] IS NULL) THEN N'' ELSE [Extent1].[MiddleName] END + N' ' + [Extent1].[LastName] AS [C1]
    FROM [dbo].[EfPersons] AS [Extent1]
```


#### [DateTime](../TestGroup50Types/Test05DateTime.cs):
- Supported
  * DateTime Where Compare With Static Variable (line 35)
     * T-Sql executed is

```SQL
SELECT 
    [Extent1].[StartDate] AS [StartDate]
    FROM [dbo].[EfParents] AS [Extent1]
    WHERE [Extent1].[StartDate] > @p__linq__0
```




The End
