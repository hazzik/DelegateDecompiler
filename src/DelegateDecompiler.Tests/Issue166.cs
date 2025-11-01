using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace DelegateDecompiler.Tests
{
    [TestFixture]
    public class Issue166 : DecompilerTestsBase
    {
        [Test]
        public void ShouldSupportInstanceClosures()
        {
            var comments = new Comment[0];
            var expected = comments.AsQueryable().Select(c => new CommentDto
            {
                HasUserPurchasedTheCourse = c.User.Purchases.Any(p => p.Items.Any(i => i.CourseId == c.CourseId)),
            });
            var actual = comments.AsQueryable().Select(c => ToCommentDto(c)).Decompile();
            AssertAreEqual(actual.Expression, expected.Expression);
        }

        [Decompile]
        static CommentDto ToCommentDto(Comment comment)
        {
            return new CommentDto
            {
                HasUserPurchasedTheCourse = comment.User.Purchases.Any(p => p.Items.Any(i => i.CourseId == comment.CourseId)),
            };
        }

        class Comment
        {
            public User User { get; set; }

            public int CourseId { get; set; }
        }

        class User
        {
            public IEnumerable<Purchase> Purchases { get; set; }
        }

        class PurchaseItem
        {
            public int CourseId { get; set; }
        }

        class Purchase
        {
            public IEnumerable<PurchaseItem> Items { get; set; }
        }

        class CommentDto
        {
            public bool HasUserPurchasedTheCourse { get; set; }
        }
    }
}
