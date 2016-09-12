Imports System
Imports System.Linq.Expressions
Imports NUnit.Framework


<TestFixture>
Public Class DecimalTests
    Inherits DecompilerTestsBase

    <Test>
    Public Sub Ceq()
        Dim expected As Expression(Of Func(Of Decimal, Decimal, Boolean)) = Function(x As Decimal, y As Decimal) x = y
        Dim compiled As Func(Of Decimal, Decimal, Boolean) = Function(x As Decimal, y As Decimal) x = y
        Test(expected, compiled)
    End Sub

    <Test>
    Public Sub Cgt()
        Dim expected As Expression(Of Func(Of Decimal, Decimal, Boolean)) = Function(x As Decimal, y As Decimal) x > y
        Dim compiled As Func(Of Decimal, Decimal, Boolean) = Function(x As Decimal, y As Decimal) x > y
        Test(expected, compiled)
    End Sub

    <Test>
    Public Sub Bgt()
        Dim expected As Expression(Of Func(Of Decimal, Decimal, Decimal)) = Function(x As Decimal, y As Decimal) If((x > y), x, y)
        Dim compiled As Func(Of Decimal, Decimal, Decimal) = Function(x As Decimal, y As Decimal) If((x > y), x, y)
        Test(expected, compiled)
    End Sub

    <Test>
    Public Sub Cge()
        Dim expected As Expression(Of Func(Of Decimal, Decimal, Boolean)) = Function(x As Decimal, y As Decimal) x >= y
        Dim compiled As Func(Of Decimal, Decimal, Boolean) = Function(x As Decimal, y As Decimal) x >= y
        Test(expected, compiled)
    End Sub

    <Test>
    Public Sub Bge()
        Dim expected As Expression(Of Func(Of Decimal, Decimal, Decimal)) = Function(x As Decimal, y As Decimal) If((x >= y), x, y)
        Dim compiled As Func(Of Decimal, Decimal, Decimal) = Function(x As Decimal, y As Decimal) If((x >= y), x, y)
        Test(expected, compiled)
    End Sub

    <Test>
    Public Sub Clt()
        Dim expected As Expression(Of Func(Of Decimal, Decimal, Boolean)) = Function(x As Decimal, y As Decimal) x < y
        Dim compiled As Func(Of Decimal, Decimal, Boolean) = Function(x As Decimal, y As Decimal) x < y
        Test(expected, compiled)
    End Sub

    <Test>
    Public Sub Blt()
        Dim expected As Expression(Of Func(Of Decimal, Decimal, Decimal)) = Function(x As Decimal, y As Decimal) If((x < y), x, y)
        Dim compiled As Func(Of Decimal, Decimal, Decimal) = Function(x As Decimal, y As Decimal) If((x < y), x, y)
        Test(expected, compiled)
    End Sub

    <Test>
    Public Sub Cle()
        Dim expected As Expression(Of Func(Of Decimal, Decimal, Boolean)) = Function(x As Decimal, y As Decimal) x <= y
        Dim compiled As Func(Of Decimal, Decimal, Boolean) = Function(x As Decimal, y As Decimal) x <= y
        Test(expected, compiled)
    End Sub

    <Test>
    Public Sub Ble()
        Dim expected As Expression(Of Func(Of Decimal, Decimal, Decimal)) = Function(x As Decimal, y As Decimal) If((x <= y), x, y)
        Dim compiled As Func(Of Decimal, Decimal, Decimal) = Function(x As Decimal, y As Decimal) If((x <= y), x, y)
        Test(expected, compiled)
    End Sub

    <Test>
    Public Sub Bne()
        Dim expected As Expression(Of Func(Of Decimal, Decimal, Decimal)) = Function(x As Decimal, y As Decimal) If((x <> y), x, y)
        Dim compiled As Func(Of Decimal, Decimal, Decimal) = Function(x As Decimal, y As Decimal) If((x <> y), x, y)
        Test(expected, compiled)
    End Sub

    <Test>
    Public Sub Beq()
        Dim expected As Expression(Of Func(Of Decimal, Decimal, Decimal)) = Function(x As Decimal, y As Decimal) If((x = y), x, y)
        Dim compiled As Func(Of Decimal, Decimal, Decimal) = Function(x As Decimal, y As Decimal) If((x = y), x, y)
        Test(expected, compiled)
    End Sub

    <Test>
    Public Sub Mul()
        Dim expected As Expression(Of Func(Of Decimal, Decimal, Decimal)) = Function(x As Decimal, y As Decimal) x*y
        Dim compiled As Func(Of Decimal, Decimal, Decimal) = Function(x As Decimal, y As Decimal) x*y
        Test(expected, compiled)
    End Sub

    <Test>
    Public Sub MulByInt()
        Dim expected As Expression(Of Func(Of Decimal, Integer, Decimal)) = Function(x As Decimal, y As Integer) x*y
        Dim compiled As Func(Of Decimal, Integer, Decimal) = Function(x As Decimal, y As Integer) x*y
        Test(expected, compiled)
    End Sub

    <Test>
    Public Sub Div()
        Dim expected As Expression(Of Func(Of Decimal, Decimal, Decimal)) = Function(x As Decimal, y As Decimal) x/y
        Dim compiled As Func(Of Decimal, Decimal, Decimal) = Function(x As Decimal, y As Decimal) x/y
        Test(expected, compiled)
    End Sub

    <Test>
    Public Sub DecimalCast()
        Dim expected As Expression(Of Func(Of Decimal)) = Function() 1.21D
        Dim expected2 As Expression(Of Func(Of Decimal)) = Function() New Decimal(121, 0, 0, False, 2)
        Dim compiled As Func(Of Decimal) = Function() 1.21D
        Test(expected, expected2, compiled)
    End Sub

    <Test>
    Public Sub DecimalConstructor()
        Dim expected As Expression(Of Func(Of Decimal)) = Function() 1.21D
        Dim expected2 As Expression(Of Func(Of Decimal)) = Function() New Decimal(121, 0, 0, False, 2)
        Dim compiled As Func(Of Decimal) = Function() New Decimal(121, 0, 0, False, 2)
        Test(expected, expected2, compiled)
    End Sub
End Class

