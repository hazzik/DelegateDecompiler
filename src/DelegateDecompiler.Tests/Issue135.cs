using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NUnit.Framework;

namespace DelegateDecompiler.Tests
{
    [TestFixture]
    public class Issue135 : DecompilerTestsBase
    {
        public class Post
        {
            public bool IsActive { get; set; }
        }

        public class Blog
        {
            public bool HasBar { get; }
            public bool HasBaz { get; }

            public IEnumerable<Post> Posts { get; }


            [Decompile]
            public bool HasFoo
            {
                get { return (HasBar || HasBaz) && Posts.Any(x => x.IsActive); }
            }

            [Decompile]
            public bool HasFoo2
            {
                get { return (HasBar && Posts.Any(x => x.IsActive)) || (HasBaz && Posts.Any(x => x.IsActive)); }
            }

            [Decompile]
            public bool HasFoo3
            {
                get { return Posts.Any(x => x.IsActive) && (HasBar || HasBaz); }
            }
        }

        [Test]
        public void Test1()
        {
            Expression<Func<Blog, bool>> expected1 = b =>
                (b.HasBar || b.HasBaz) && b.Posts.Any(x => x.IsActive);

            Expression<Func<Blog, bool>> expected2 = b =>
                b.HasBar ? b.Posts.Any(x => x.IsActive) : b.HasBaz && b.Posts.Any(x => x.IsActive);

            Func<Blog, bool> actual = b =>
                (b.HasBar || b.HasBaz) && b.Posts.Any(x => x.IsActive);

            Test(expected1, expected2, actual, false);
        }

        [Test]
        public void Test2()
        {
            Expression<Func<Blog, bool>> expected1 = b =>
                b.HasBar && b.Posts.Any(x => x.IsActive) || b.HasBaz && b.Posts.Any(x => x.IsActive);

            Expression<Func<Blog, bool>> expected2 = b =>
                b.HasBar
                    ? b.Posts.Any(x => x.IsActive) || b.HasBaz && b.Posts.Any(x => x.IsActive)
                    : b.HasBaz && b.Posts.Any(x => x.IsActive);

            Func<Blog, bool> actual = b =>
                b.HasBar && b.Posts.Any(x => x.IsActive) || b.HasBaz && b.Posts.Any(x => x.IsActive);

            Test(expected1, expected2, actual, false);
        }

        [Test]
        public void Test3()
        {
            Expression<Func<Blog, bool>> expected = b =>
                b.Posts.Any(x => x.IsActive) && (b.HasBar || b.HasBaz);

            Func<Blog, bool> actual = b =>
                b.Posts.Any(x => x.IsActive) && (b.HasBar || b.HasBaz);

            Test(expected, actual);
        }

        [Test, Ignore("Not fixed yet.")]
        public void TestQueryable1()
        {
            var blogs = new[] { new Blog() }.AsQueryable();

            var expected = (
                from b in blogs
                where (b.HasBar || b.HasBaz) && b.Posts.Any(x => x.IsActive)
                select b);

            var actual = (
                from b in blogs
                where b.HasFoo
                select b).Decompile();

            AssertAreEqual(expected.Expression, actual.Expression, compareDebugView: false);
        }

        [Test, Ignore("Not fixed yet.")]
        public void TestQueryable2()
        {
            var blogs = new[] { new Blog() }.AsQueryable();

            var expected = (
                from b in blogs
                where (b.HasBar && b.Posts.Any(x => x.IsActive)) || (b.HasBaz && b.Posts.Any(x => x.IsActive))
                select b);

            var actual = (
                from b in blogs
                where b.HasFoo2
                select b).Decompile();

            AssertAreEqual(expected.Expression, actual.Expression, compareDebugView: false);
        }

        [Test]
        public void TestQueryable3()
        {
            var blogs = new[] { new Blog() }.AsQueryable();

            var expected = (
                from b in blogs
                where b.Posts.Any(x => x.IsActive) && (b.HasBar || b.HasBaz)
                select b);

            var actual = (
                from b in blogs
                where b.HasFoo3
                select b).Decompile();

            AssertAreEqual(expected.Expression, actual.Expression, compareDebugView: false);
        }
    }
}