using System;
using System.IO;
using NUnit.Framework;
using Tektosyne.IO;

namespace Tektosyne.UnitTest.IO {

    [TestFixture]
    public class RootedPathTest {

        [Test, ExpectedException(typeof(ArgumentException))]
        public void ConstructorFailed() {
            RootedPath path = new RootedPath("Windows", "win.ini");
        }

        [Test]
        public void DefaultPath() {
            RootedPath path = new RootedPath(@"C:\Windows");
            Assert.AreEqual(@"C:\Windows\", path.RootFolder);
            Assert.AreEqual(@"C:\Windows\", path.AbsolutePath);

            Assert.AreEqual("", path.RelativePath);
            Assert.IsTrue(path.IsEmpty);
            Assert.AreEqual(@"C:\Windows", path.DirectoryName);
            Assert.AreEqual("", path.FileName);
        }

        [Test]
        public void ImplicitPath() {
            RootedPath path = new RootedPath(@"C:\Windows\", "win.ini");
            Assert.AreEqual(@"C:\Windows\", path.RootFolder);
            Assert.AreEqual(@"C:\Windows\win.ini", path.AbsolutePath);

            Assert.AreEqual("win.ini", path.RelativePath);
            Assert.IsFalse(path.IsEmpty);
            Assert.AreEqual(@"C:\Windows", path.DirectoryName);
            Assert.AreEqual("win.ini", path.FileName);
        }

        [Test]
        public void ExplicitPath() {
            RootedPath path = new RootedPath(@"C:\Windows", @"C:\Windows\win.ini");
            Assert.AreEqual(@"C:\Windows\", path.RootFolder);
            Assert.AreEqual(@"C:\Windows\win.ini", path.AbsolutePath);

            Assert.AreEqual("win.ini", path.RelativePath);
            Assert.IsFalse(path.IsEmpty);
            Assert.AreEqual(@"C:\Windows", path.DirectoryName);
            Assert.AreEqual("win.ini", path.FileName);
        }

        [Test]
        public void DifferentPath() {
            RootedPath path = new RootedPath(@"C:\Windows", @"C:\Temp\win.ini");
            Assert.AreEqual(@"C:\Windows\", path.RootFolder);
            Assert.AreEqual(@"C:\Temp\win.ini", path.AbsolutePath);

            Assert.AreEqual(@"C:\Temp\win.ini", path.RelativePath);
            Assert.IsFalse(path.IsEmpty);
            Assert.AreEqual(@"C:\Temp", path.DirectoryName);
            Assert.AreEqual("win.ini", path.FileName);
        }

        [Test]
        public void EquivalentPath() {
            RootedPath path = new RootedPath(@"C:\Windows\", @"C:/Windows");
            Assert.AreEqual(@"C:\Windows\", path.AbsolutePath);

            Assert.AreEqual("", path.RelativePath);
            Assert.IsTrue(path.IsEmpty);
            Assert.AreEqual(@"C:\Windows", path.DirectoryName);
            Assert.AreEqual("", path.FileName);
        }

        [Test]
        public void Clone() {
            RootedPath path = new RootedPath(@"C:\Windows", "win.ini");
            RootedPath clone = (RootedPath) path.Clone();

            Assert.AreNotSame(path, clone);
            Assert.AreEqual(path, clone);
            Assert.AreEqual(@"C:\Windows\win.ini", clone.AbsolutePath);
        }

        [Test]
        public void Equals() {
            Assert.IsTrue(RootedPath.Equals(null, null));

            RootedPath path = new RootedPath(@"C:\Windows", "win.ini");
            Assert.AreEqual(new RootedPath(@"C:\Windows"), new RootedPath(@"C:\Windows"));
            Assert.AreEqual(path, new RootedPath(@"C:\Windows", "win.ini"));

            Assert.AreNotEqual(path, null);
            Assert.AreNotEqual(path, new RootedPath(@"C:\Temp", "win.ini"));
            Assert.AreNotEqual(path, new RootedPath(@"C:\Windows", "system.ini"));
            Assert.AreNotEqual(path, new RootedPath(@"C:\", @"Windows\win.ini"));
        }
    }
}
