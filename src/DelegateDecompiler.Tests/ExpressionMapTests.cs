using System.Linq;
using NUnit.Framework;

namespace DelegateDecompiler.Tests
{
    public class ExpressionMapTests : ConfigurationTestsBase
    {
        [Test]
        public void ExpressionMapShouldRegisterPropertyAsDecompileable()
        {
            Assert.Null(InstanceGetter());
            var configuration = new DefaultConfiguration();
            configuration.AddExpressionMap<Employee, string>(x => x.FullNameWithExpressionMap, x => x.FirstName + " " + x.LastName);
            Configuration.Configure(configuration);
            Assert.IsTrue(configuration.ShouldDecompile(typeof(Employee).GetProperty(nameof(Employee.FullNameWithExpressionMap))));
        }

        [Test]
        public void NoExpressionMapShouldNotRegisterPropertyAsDecompileable()
        {
            Assert.Null(InstanceGetter());
            var configuration = new DefaultConfiguration();
            Configuration.Configure(configuration);
            Assert.IsFalse(configuration.ShouldDecompile(typeof(Employee).GetProperty(nameof(Employee.FullNameWithExpressionMap))));
        }

        [Test]
        public void ExpressionMapShouldDecompile()
        {
            Assert.Null(InstanceGetter());
            var configuration = new DefaultConfiguration();
            configuration.AddExpressionMap<Employee, string>(x => x.FullNameWithExpressionMap, x => x.FirstName + " " + x.LastName);
            Configuration.Configure(configuration);


            var employees = new[] { new Employee { FirstName = "Test", LastName = "User" } };

            var expected = (from employee in employees.AsQueryable()
                            where (employee.FirstName + " " + employee.LastName) == "Test User"
                            select employee);

            var actual = (from employee in employees.AsQueryable()
                          where employee.FullNameWithExpressionMap == "Test User"
                          select employee).Decompile();

            Assert.AreEqual(expected.Expression.ToString(), actual.Expression.ToString());
        }

        [Test]
        public void ExpressionMapMethodShouldDecompile()
        {
            Assert.Null(InstanceGetter());
            var configuration = new DefaultConfiguration();
            configuration.AddExpressionMap<Employee, string>(x => x.GetFullNameWithExpressionMap(), x => x.FirstName + " " + x.LastName);
            Configuration.Configure(configuration);


            var employees = new[] { new Employee { FirstName = "Test", LastName = "User" } };

            var expected = (from employee in employees.AsQueryable()
                            where (employee.FirstName + " " + employee.LastName) == "Test User"
                            select employee);

            var actual = (from employee in employees.AsQueryable()
                          where employee.GetFullNameWithExpressionMap() == "Test User"
                          select employee).Decompile();

            Assert.AreEqual(expected.Expression.ToString(), actual.Expression.ToString());
        }
    }
}
