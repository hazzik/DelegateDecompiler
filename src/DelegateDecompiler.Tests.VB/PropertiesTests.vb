Imports NUnit.Framework

<TestFixture>
Public Class PropertiesTests
    <Test>
    Public Sub InlineProperty()
        Dim employees = New TestClass() {New TestClass() With { .A = True }}
        Dim expected = employees.AsQueryable().Where(Function(e) e.A)
        Dim actual As IQueryable(Of TestClass) = employees.AsQueryable().Where(Function(e) e.B).Decompile()

        Assert.AreEqual(expected.Expression.ToString(), actual.Expression.ToString())
    End Sub

    class TestClass
        Property A As Boolean

        <Computed>
        ReadOnly Property B As Boolean
            Get
                Return A
            End Get
        End Property
    End Class
End Class