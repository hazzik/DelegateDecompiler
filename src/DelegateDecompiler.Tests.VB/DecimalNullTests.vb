Imports System
Imports System.Linq.Expressions
Imports NUnit.Framework


<TestFixture>
Public Class DecimalNullTests
    Inherits DecompilerTestsBase


    <Test>
    Public Sub ExpressionWithNullable()
        Dim expected As Expression(Of Func(Of Decimal, Decimal?)) = Function(x As Decimal) x
        Dim compiled As Func(Of Decimal, Decimal?) = Function(x As Decimal) x
        Test(compiled, expected)
    End Sub

    <Test>
    Public Sub ExpressionWithNullableEqual()
        Dim expected As Expression(Of Func(Of Decimal?, Decimal?, Boolean?)) = Function(x As Decimal?, y As Decimal?) x = y
        Dim compiled As Func(Of Decimal?, Decimal?, Boolean?) = Function(x As Decimal?, y As Decimal?) x = y
        Test(compiled, expected)
    End Sub

    <Test>
    Public Sub ExpressionWithNullableNotEqual()
        Dim expected As Expression(Of Func(Of Decimal?, Decimal?, Boolean?)) = Function(x As Decimal?, y As Decimal?) x <> y
        Dim compiled As Func(Of Decimal?, Decimal?, Boolean?) = Function(x As Decimal?, y As Decimal?) x <> y
        Test(compiled, expected)
    End Sub

    <Test>
    Public Sub ExpressionWithNullableGreaterThan()
        Dim expected As Expression(Of Func(Of Decimal?, Decimal?, Boolean?)) = Function(x As Decimal?, y As Decimal?) x > y
        Dim compiled As Func(Of Decimal?, Decimal?, Boolean?) = Function(x As Decimal?, y As Decimal?) x > y
        Test(compiled, expected)
    End Sub

    <Test>
    Public Sub ExpressionWithNullableGreaterThanOrEqual()
        Dim expected As Expression(Of Func(Of Decimal?, Decimal?, Boolean?)) = Function(x As Decimal?, y As Decimal?) x >= y
        Dim compiled As Func(Of Decimal?, Decimal?, Boolean?) = Function(x As Decimal?, y As Decimal?) x >= y
        Test(compiled, expected)
    End Sub

    <Test>
    Public Sub ExpressionWithNullableLessThan()
        Dim expected As Expression(Of Func(Of Decimal?, Decimal?, Boolean?)) = Function(x As Decimal?, y As Decimal?) x < y
        Dim compiled As Func(Of Decimal?, Decimal?, Boolean?) = Function(x As Decimal?, y As Decimal?) x < y
        Test(compiled, expected)
    End Sub

    <Test>
    Public Sub ExpressionWithNullableLessThanOrEqual()
        Dim expected As Expression(Of Func(Of Decimal?, Decimal?, Boolean?)) = Function(x As Decimal?, y As Decimal?) x <= y
        Dim compiled As Func(Of Decimal?, Decimal?, Boolean?) = Function(x As Decimal?, y As Decimal?) x <= y
        Test(compiled, expected)
    End Sub

    <Test>
    Public Sub ExpressionWithNullableMul()
        Dim expected As Expression(Of Func(Of Decimal?, Decimal?, Decimal?)) = Function(x As Decimal?, y As Decimal?) x * y
        Dim compiled As Func(Of Decimal?, Decimal?, Decimal?) = Function(x As Decimal?, y As Decimal?) x * y
        Test(compiled, expected)
    End Sub

    <Test>
    Public Sub ExpressionWithNullablePlus()
        Dim expected As Expression(Of Func(Of Decimal?, Decimal?, Decimal?)) = Function(x As Decimal?, y As Decimal?) x + y
        Dim compiled As Func(Of Decimal?, Decimal?, Decimal?) = Function(x As Decimal?, y As Decimal?) x + y
        Test(compiled, expected)
    End Sub

    <Test>
    Public Sub ExpressionWithNullableMinus()
        Dim expected As Expression(Of Func(Of Decimal?, Decimal?, Decimal?)) = Function(x As Decimal?, y As Decimal?) x - y
        Dim compiled As Func(Of Decimal?, Decimal?, Decimal?) = Function(x As Decimal?, y As Decimal?) x - y
        Test(compiled, expected)
    End Sub

    <Test>
    Public Sub ExpressionWithNullableDiv()
        Dim expected As Expression(Of Func(Of Decimal?, Decimal?, Decimal?)) = Function(x As Decimal?, y As Decimal?) x / y
        Dim compiled As Func(Of Decimal?, Decimal?, Decimal?) = Function(x As Decimal?, y As Decimal?) x / y
        Test(compiled, expected)
    End Sub

    <Test>
    Public Sub ExpressionWithNullableEqual2()
        Dim expected As Expression(Of Func(Of Decimal?, Decimal, Boolean?)) = Function(x As Decimal?, y As Decimal) x = y
        Dim compiled As Func(Of Decimal?, Decimal, Boolean?) = Function(x As Decimal?, y As Decimal) x = y
        Test(compiled, expected)
    End Sub

    <Test>
    Public Sub ExpressionWithNullableNotEqual2()
        Dim expected As Expression(Of Func(Of Decimal?, Decimal, Boolean?)) = Function(x As Decimal?, y As Decimal) x <> y
        Dim compiled As Func(Of Decimal?, Decimal, Boolean?) = Function(x As Decimal?, y As Decimal) x <> y
        Test(compiled, expected)
    End Sub

    <Test>
    Public Sub ExpressionWithNullableGreaterThan2()
        Dim expected As Expression(Of Func(Of Decimal?, Decimal, Boolean?)) = Function(x As Decimal?, y As Decimal) x > y
        Dim compiled As Func(Of Decimal?, Decimal, Boolean?) = Function(x As Decimal?, y As Decimal) x > y
        Test(compiled, expected)
    End Sub

    <Test>
    Public Sub ExpressionWithNullableGreaterThanOrEqual2()
        Dim expected As Expression(Of Func(Of Decimal?, Decimal, Boolean?)) = Function(x As Decimal?, y As Decimal) x >= y
        Dim compiled As Func(Of Decimal?, Decimal, Boolean?) = Function(x As Decimal?, y As Decimal) x >= y
        Test(compiled, expected)
    End Sub

    <Test>
    Public Sub ExpressionWithNullableLessThan2()
        Dim expected As Expression(Of Func(Of Decimal?, Decimal, Boolean?)) = Function(x As Decimal?, y As Decimal) x < y
        Dim compiled As Func(Of Decimal?, Decimal, Boolean?) = Function(x As Decimal?, y As Decimal) x < y
        Test(compiled, expected)
    End Sub

    <Test>
    Public Sub ExpressionWithNullableLessThanOrEqual2()
        Dim expected As Expression(Of Func(Of Decimal?, Decimal, Boolean?)) = Function(x As Decimal?, y As Decimal) x <= y
        Dim compiled As Func(Of Decimal?, Decimal, Boolean?) = Function(x As Decimal?, y As Decimal) x <= y
        Test(compiled, expected)
    End Sub

    <Test>
    Public Sub ExpressionWithNullableMul2()
        Dim expected As Expression(Of Func(Of Decimal?, Decimal, Decimal?)) = Function(x As Decimal?, y As Decimal) x * y
        Dim compiled As Func(Of Decimal?, Decimal, Decimal?) = Function(x As Decimal?, y As Decimal) x * y
        Test(compiled, expected)
    End Sub

    <Test>
    Public Sub ExpressionWithNullablePlus2()
        Dim expected As Expression(Of Func(Of Decimal?, Decimal, Decimal?)) = Function(x As Decimal?, y As Decimal) x + y
        Dim compiled As Func(Of Decimal?, Decimal, Decimal?) = Function(x As Decimal?, y As Decimal) x + y
        Test(compiled, expected)
    End Sub

    <Test>
    Public Sub ExpressionWithNullableMinus2()
        Dim expected As Expression(Of Func(Of Decimal?, Decimal, Decimal?)) = Function(x As Decimal?, y As Decimal) x - y
        Dim compiled As Func(Of Decimal?, Decimal, Decimal?) = Function(x As Decimal?, y As Decimal) x - y
        Test(compiled, expected)
    End Sub

    <Test>
    Public Sub ExpressionWithNullableDiv2()
        Dim expected As Expression(Of Func(Of Decimal?, Decimal, Decimal?)) = Function(x As Decimal?, y As Decimal) x / y
        Dim compiled As Func(Of Decimal?, Decimal, Decimal?) = Function(x As Decimal?, y As Decimal) x / y
        Test(compiled, expected)
    End Sub

    <Test>
    Public Sub ExpressionWithNullableEqual3()
        Dim expected As Expression(Of Func(Of Decimal, Decimal?, Boolean?)) = Function(x As Decimal, y As Decimal?) x = y
        Dim compiled As Func(Of Decimal, Decimal?, Boolean?) = Function(x As Decimal, y As Decimal?) x = y
        Test(compiled, expected)
    End Sub

    <Test>
    Public Sub ExpressionWithNullableNotEqual3()
        Dim expected As Expression(Of Func(Of Decimal, Decimal?, Boolean?)) = Function(x As Decimal, y As Decimal?) x <> y
        Dim compiled As Func(Of Decimal, Decimal?, Boolean?) = Function(x As Decimal, y As Decimal?) x <> y
        Test(compiled, expected)
    End Sub

    <Test>
    Public Sub ExpressionWithNullableGreaterThan3()
        Dim expected As Expression(Of Func(Of Decimal, Decimal?, Boolean?)) = Function(x As Decimal, y As Decimal?) x > y
        Dim compiled As Func(Of Decimal, Decimal?, Boolean?) = Function(x As Decimal, y As Decimal?) x > y
        Test(compiled, expected)
    End Sub

    <Test>
    Public Sub ExpressionWithNullableGreaterThanOrEqual3()
        Dim expected As Expression(Of Func(Of Decimal, Decimal?, Boolean?)) = Function(x As Decimal, y As Decimal?) x >= y
        Dim compiled As Func(Of Decimal, Decimal?, Boolean?) = Function(x As Decimal, y As Decimal?) x >= y
        Test(compiled, expected)
    End Sub

    <Test>
    Public Sub ExpressionWithNullableLessThan3()
        Dim expected As Expression(Of Func(Of Decimal, Decimal?, Boolean?)) = Function(x As Decimal, y As Decimal?) x < y
        Dim compiled As Func(Of Decimal, Decimal?, Boolean?) = Function(x As Decimal, y As Decimal?) x < y
        Test(compiled, expected)
    End Sub

    <Test>
    Public Sub ExpressionWithNullableLessThanOrEqual3()
        Dim expected As Expression(Of Func(Of Decimal, Decimal?, Boolean?)) = Function(x As Decimal, y As Decimal?) x <= y
        Dim compiled As Func(Of Decimal, Decimal?, Boolean?) = Function(x As Decimal, y As Decimal?) x <= y
        Test(compiled, expected)
    End Sub

    <Test>
    Public Sub ExpressionWithNullableMul3()
        Dim expected As Expression(Of Func(Of Decimal, Decimal?, Decimal?)) = Function(x As Decimal, y As Decimal?) x * y
        Dim compiled As Func(Of Decimal, Decimal?, Decimal?) = Function(x As Decimal, y As Decimal?) x * y
        Test(compiled, expected)
    End Sub

    <Test>
    Public Sub ExpressionWithNullablePlus3()
        Dim expected As Expression(Of Func(Of Decimal, Decimal?, Decimal?)) = Function(x As Decimal, y As Decimal?) x + y
        Dim compiled As Func(Of Decimal, Decimal?, Decimal?) = Function(x As Decimal, y As Decimal?) x + y
        Test(compiled, expected)
    End Sub

    <Test>
    Public Sub ExpressionWithNullableMinus3()
        Dim expected As Expression(Of Func(Of Decimal, Decimal?, Decimal?)) = Function(x As Decimal, y As Decimal?) x - y
        Dim compiled As Func(Of Decimal, Decimal?, Decimal?) = Function(x As Decimal, y As Decimal?) x - y
        Test(compiled, expected)
    End Sub

    <Test>
    Public Sub ExpressionWithNullableDiv3()
        Dim expected As Expression(Of Func(Of Decimal, Decimal?, Decimal?)) = Function(x As Decimal, y As Decimal?) x / y
        Dim compiled As Func(Of Decimal, Decimal?, Decimal?) = Function(x As Decimal, y As Decimal?) x / y
        Test(compiled, expected)
    End Sub

    <Test>
    Public Sub ExpressionWithNullableSum3()
        Dim expected As Expression(Of Func(Of Decimal?, Decimal?, Decimal?, Decimal?)) = Function(x As Decimal?, y As Decimal?, z As Decimal?) x + y + z
        Dim compiled As Func(Of Decimal?, Decimal?, Decimal?, Decimal?) = Function(x As Decimal?, y As Decimal?, z As Decimal?) x + y + z
        Test(compiled, expected)
    End Sub
End Class