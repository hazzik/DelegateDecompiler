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
        public class Post {
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
                get
                {
                    return (this.HasBar || this.HasBaz) && this.Posts.Any(x => x.IsActive);
                }
            }

            [Decompile]
            public bool HasFoo2
            {
                get
                {
                    return (this.HasBar && this.Posts.Any(x => x.IsActive)) || (this.HasBaz && this.Posts.Any(x => x.IsActive)) ;
                }
            }

            [Decompile]
            public bool HasFoo3
            {
                get
                {
                    return this.Posts.Any(x => x.IsActive) && (this.HasBar || this.HasBaz);
                }
            }
        }


        [Test, Ignore("Difference in debugView is expected")]
        public void Test()
        {
            Expression<Func<Blog, bool>> expected = b => b.HasBar ? b.Posts.Any(x => x.IsActive) : (b.HasBaz && b.Posts.Any(x => x.IsActive));
            Func<Blog, bool> compiled = b => b.HasFoo;

            Test(expected, compiled);
        }

        [Test]
        public void Test2()
        {
            Expression<Func<Blog, bool>> expected = b => b.Posts.Any(x => x.IsActive) && (b.HasBar ? true : b.HasBaz);
            Func<Blog, bool> compiled = b => b.HasFoo3;

            Test(expected, compiled);
        }
    }
}