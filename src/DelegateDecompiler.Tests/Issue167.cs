using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace DelegateDecompiler.Tests
{
    [TestFixture]
    public class Issue167 : DecompilerTestsBase
    {
        [Test]
        public void ShouldFlattenTransparentExpressions()
        {
            var comments = new Comment[0];
            var expected = comments.AsQueryable().Select(c => new CommentDto
            {
                IsUserPurchaser = c.Purchases.Any(p => p.UserId == c.UserId)
            });
            var actual = comments.AsQueryable().Select(c => ToCommentDto(c)).Decompile();

            AssertAreEqual(expected.Expression, actual.Expression);
        }

        [Decompile]
        static CommentDto ToCommentDto(Comment comment)
        {
            return new CommentDto
            {
                IsUserPurchaser = comment.Purchases.Any(p => p.UserId == comment.UserId),
            };
        }

        public class Comment
        {
            public int UserId { get; set; }

            public IEnumerable<Purchase> Purchases { get; }
        }

        public class Purchase
        {
            public int UserId { get; set; }
        }

        public class CommentDto
        {
            public object IsUserPurchaser { get; set; }
        }
    }
}
