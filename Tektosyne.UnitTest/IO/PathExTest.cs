using System;
using System.IO;
using NUnit.Framework;
using Tektosyne.IO;

namespace Tektosyne.UnitTest.IO {

    [TestFixture]
    public class PathExTest {

        [Test]
        public void AddDirectorySeparator() {
            Assert.AreEqual(@"\", PathEx.AddDirectorySeparator(null));

            Assert.AreEqual(@"C:\Test\", PathEx.AddDirectorySeparator(@"C:\Test"));
            Assert.AreEqual(@"C:\Test\", PathEx.AddDirectorySeparator(@"C:\Test\"));
            Assert.AreEqual(@"C:\Test/", PathEx.AddDirectorySeparator(@"C:\Test/"));
        }

        [Test]
        public void Equals() {
            Assert.IsTrue(PathEx.Equals(null, null));
            Assert.IsFalse(PathEx.Equals(null, "SomePath"));
            Assert.IsFalse(PathEx.Equals("SomePath", null));

            Assert.IsTrue(PathEx.Equals(@"C:\Test", @"C:\Test"));
            Assert.IsTrue(PathEx.Equals(@"C:\Test", @"C:/Test"));
            Assert.IsTrue(PathEx.Equals(@"C:/Test", @"C:\Test"));
            Assert.IsTrue(PathEx.Equals(@"C:/Test", @"C:/Test"));

            Assert.IsTrue(PathEx.Equals(@"C:\Test", @"C:\Test\"));
            Assert.IsTrue(PathEx.Equals(@"C:\Test", @"C:\Test//"));
            Assert.IsFalse(PathEx.Equals(@"C:\Test", @"C:\Testing"));
        }

        [Test]
        public void GetTempFileName() {
            string temp = Path.GetTempPath();
            string path = PathEx.GetTempFileName(".txt");

            Assert.AreEqual(".txt", Path.GetExtension(path));
            Assert.AreEqual(path, Path.Combine(temp, Path.GetFileName(path)));
            Assert.IsFalse(File.Exists(path));
        }

        [Test]
        public void NormalizeDirectorySeparator() {
            Assert.AreEqual("", PathEx.NormalizeDirectorySeparator(null));

            Assert.AreEqual(@"C:\Test", PathEx.NormalizeDirectorySeparator(@"C:\Test"));
            Assert.AreEqual(@"C:\Test", PathEx.NormalizeDirectorySeparator(@"C:/Test"));
            Assert.AreEqual(@"C:\Test\", PathEx.NormalizeDirectorySeparator(@"C:\Test/"));
        }

        [Test]
        public void RemoveDirectorySeparator() {
            Assert.AreEqual("", PathEx.RemoveDirectorySeparator(null));

            Assert.AreEqual(@"C:\Test", PathEx.RemoveDirectorySeparator(@"C:\Test"));
            Assert.AreEqual(@"C:\Test", PathEx.RemoveDirectorySeparator(@"C:\Test\"));
            Assert.AreEqual(@"C:\Test", PathEx.RemoveDirectorySeparator(@"C:\Test/"));
            Assert.AreEqual(@"C:\Test", PathEx.RemoveDirectorySeparator(@"C:\Test\/\"));
        }

        [Test]
        public void Shorten() {
            Assert.AreEqual("", PathEx.Shorten(null, null));

            Assert.AreEqual(@"Test", PathEx.Shorten(@"C:\Test", @"C:\"));
            Assert.AreEqual(@"B\C\Test", PathEx.Shorten(@"C:\A\B\C\Test", @"C:\A"));
            Assert.AreEqual(@"C:\AB\C\Test", PathEx.Shorten(@"C:\AB\C\Test", @"C:\A"));
            Assert.AreEqual(@"C:\Test", PathEx.Shorten(@"C:\Test", @"C:\Program Files"));
        }
    }
}
