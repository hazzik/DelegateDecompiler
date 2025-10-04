Imports System.Linq.Expressions
Imports NUnit.Framework

<TestFixture>
Public Class NullTests
    Inherits DecompilerTestsBase

    <Test>
    Public Sub NotNull()
        Dim expected As Expression(Of Func(Of Object, Boolean)) = Function(x As Object) x IsNot Nothing
        Dim compiled As Func(Of Object, Boolean) = Function(x As Object) x IsNot Nothing
        Test(compiled, expected)
    End Sub

End Class