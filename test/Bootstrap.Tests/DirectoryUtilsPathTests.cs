using System.IO;
using Bootstrap.Components.Storage;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bootstrap.Tests
{
    /// <summary>
    /// Pure path-comparison tests — no filesystem, so they run on every platform.
    /// </summary>
    [TestClass]
    public class DirectoryUtilsPathTests
    {
        private static string Root => Path.GetFullPath(Path.Combine(Path.GetTempPath(), "lm-path-tests"));

        private static string Under(params string[] segments) => Path.Combine(Root, Path.Combine(segments));

        [TestMethod]
        public void SamePathIsSameOrUnder()
        {
            Assert.IsTrue(DirectoryUtils.IsSameOrUnder(Under("Book"), Under("Book")));
        }

        [TestMethod]
        public void TrailingSeparatorIsIgnored()
        {
            Assert.IsTrue(DirectoryUtils.IsSameOrUnder(
                Under("Book") + Path.DirectorySeparatorChar, Under("Book")));
        }

        [TestMethod]
        public void NestedPathIsUnder()
        {
            Assert.IsTrue(DirectoryUtils.IsSameOrUnder(Under("Book"), Under("Book", "Chapter", "1")));
        }

        /// <summary>
        /// The regression this method exists for: a sibling whose name starts with the parent's is
        /// not nested, so copying "Book" into "Book2" must be allowed.
        /// </summary>
        [TestMethod]
        public void SiblingSharingANamePrefixIsNotUnder()
        {
            Assert.IsFalse(DirectoryUtils.IsSameOrUnder(Under("Book"), Under("Book2")));
            Assert.IsFalse(DirectoryUtils.IsSameOrUnder(Under("Book"), Under("Book2", "Book")));
        }

        [TestMethod]
        public void UnrelatedPathIsNotUnder()
        {
            Assert.IsFalse(DirectoryUtils.IsSameOrUnder(Under("Book"), Under("Other")));
        }

        [TestMethod]
        public void ParentIsNotUnderItsChild()
        {
            Assert.IsFalse(DirectoryUtils.IsSameOrUnder(Under("Book", "Chapter"), Under("Book")));
        }

        [TestMethod]
        public void RelativeSegmentsAreResolved()
        {
            Assert.IsTrue(DirectoryUtils.IsSameOrUnder(
                Under("Book"), Under("Book", "Chapter", "..", "Chapter", "1")));
            Assert.IsFalse(DirectoryUtils.IsSameOrUnder(
                Under("Book"), Under("Book", "..", "Book2")));
        }
    }
}
