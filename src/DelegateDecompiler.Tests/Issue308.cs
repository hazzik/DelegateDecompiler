using System;
using NUnit.Framework;

namespace DelegateDecompiler.Tests;

[TestFixture]
public class Issue308 : DecompilerTestsBase
{
    [Test]
    public void TestIsValidAt1()
    {
        static bool Actual<TEntity>(TEntity entity, DateTime dateTime) where TEntity : ITimeValidity =>
            entity.IsValidAt(dateTime);

        Test(Actual<Entity>, (entity, dateTime) => entity.IsValidAt(dateTime));
    }

    [Test]
    public void TestIsValidAt2()
    {
        static bool Actual<TEntity>(TEntity entity, IDateTimeProvider dateTimeProvider)
            where TEntity : ITimeValidity => entity.IsValidAt(dateTimeProvider.Now);
     
        Test(Actual<Entity>, (entity, dateTimeProvider) => entity.IsValidAt(dateTimeProvider.Now));
    }

    [Test]
    public void TestIsValidAt3()
    {
        static bool Actual<TEntity>(TEntity entity, DateTime dateTime) where TEntity : ITimeValidity =>
            entity.IsValidAt(DateTime.Now);

        Test(Actual<Entity>, (entity, dateTime) => entity.IsValidAt(DateTime.Now));
    }

    [Test]
    public void TestIsValidAt4()
    {
        static bool Actual<TEntity>(TEntity entity) where TEntity : ITimeValidity =>
            entity.IsValidAt(DateTime.Now);

        Test(Actual<Entity>, entity => entity.IsValidAt(DateTime.Now));
    }

    interface IDateTimeProvider
    {
        public DateTime Now { get; }
    }

    interface ITimeValidity
    {
        public bool IsValidAt(DateTime time);
    }

    class Entity : ITimeValidity
    {
        public bool IsValidAt(DateTime time) => default;
    }
}
