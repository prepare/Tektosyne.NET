using System;
using System.Windows.Media;
using NUnit.Framework;
using Tektosyne.Windows;

using GdiColor = System.Drawing.Color;

namespace Tektosyne.UnitTest.Windows {

    [TestFixture]
    public class WindowsUtilityTest {

        [Test]
        public void ToGdiColor() {
            Assert.AreEqual(GdiColor.FromArgb(0xFF, 0x20, 0x30, 0x40),
                Color.FromArgb(0xFF, 0x20, 0x30, 0x40).ToGdiColor());

            Assert.AreEqual(GdiColor.FromArgb(0x7F, 0x1E, 0x2E, 0x3E),
                Color.FromArgb(0x7F, 0x1E, 0x2E, 0x3E).ToGdiColor());

            Assert.AreEqual(GdiColor.FromArgb(0, 0, 0, 0),
                Color.FromArgb(0x00, 0x00, 0x00, 0x00).ToGdiColor());
        }

        [Test]
        public void ToWpfColor() {
            Assert.AreEqual(Color.FromArgb(0xFF, 0x20, 0x30, 0x40),
                GdiColor.FromArgb(0xFF, 0x20, 0x30, 0x40).ToWpfColor());

            Assert.AreEqual(Color.FromArgb(0x7F, 0x1E, 0x2E, 0x3E),
                GdiColor.FromArgb(0x7F, 0x1E, 0x2E, 0x3E).ToWpfColor());

            Assert.AreEqual(Color.FromArgb(0, 0, 0, 0),
                GdiColor.FromArgb(0x00, 0x00, 0x00, 0x00).ToWpfColor());
        }
    }
}
