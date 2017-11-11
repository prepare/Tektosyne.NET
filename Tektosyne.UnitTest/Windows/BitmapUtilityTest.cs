using System;
using System.Windows.Media;
using NUnit.Framework;
using Tektosyne.Windows;

namespace Tektosyne.UnitTest.Windows {

    [TestFixture]
    public class BitmapUtilityTest {

        [Test]
        public void BlendPbgra32() {
            Assert.AreEqual(0x12345678, BitmapUtility.BlendPbgra32(0x00ABCDEF, 0x12345678));
            Assert.AreEqual(0xFFABCDEF, BitmapUtility.BlendPbgra32(0xFFABCDEF, 0x12345678));
            Assert.AreEqual(0xAF485058, BitmapUtility.BlendPbgra32(0x7F203040, 0x60504030));
        }

        [Test]
        public void ColorFromPbgra32() {
            Assert.AreEqual(Color.FromArgb(0xFF, 0x20, 0x30, 0x40),
                BitmapUtility.ColorFromPbgra32(0xFF203040));

            Assert.AreEqual(Color.FromArgb(0x7F, 0x1E, 0x2E, 0x3E),
                BitmapUtility.ColorFromPbgra32(0x7F0F171F));

            Assert.AreEqual(Color.FromArgb(0x00, 0x00, 0x00, 0x00),
                BitmapUtility.ColorFromPbgra32(0x00000000));
        }

        [Test]
        public void ColorToOpaquePbgra32() {
            Assert.AreEqual(0xFF203040, BitmapUtility.ColorToOpaquePbgra32(
                Color.FromArgb(0xFF, 0x20, 0x30, 0x40)));

            Assert.AreEqual(0xFF203040, BitmapUtility.ColorToOpaquePbgra32(
                Color.FromArgb(0x7F, 0x20, 0x30, 0x40)));

            Assert.AreEqual(0xFF203040, BitmapUtility.ColorToOpaquePbgra32(
                Color.FromArgb(0x00, 0x20, 0x30, 0x40)));
        }

        [Test]
        public void ColorToPbgra32() {
            Assert.AreEqual(0xFF203040, BitmapUtility.ColorToPbgra32(
                Color.FromArgb(0xFF, 0x20, 0x30, 0x40)));

            Assert.AreEqual(0x7F0F171F, BitmapUtility.ColorToPbgra32(
                Color.FromArgb(0x7F, 0x20, 0x30, 0x40)));

            Assert.AreEqual(0x00000000, BitmapUtility.ColorToPbgra32(
                Color.FromArgb(0x00, 0x20, 0x30, 0x40)));
        }

        [Test]
        public void OpaquePbgra32() {
            Assert.AreEqual(0xFF203040, BitmapUtility.OpaquePbgra32(0xFF203040));
            Assert.AreEqual(0xFF1E2E3E, BitmapUtility.OpaquePbgra32(0x7F0F171F));
            Assert.AreEqual(0x00000000, BitmapUtility.OpaquePbgra32(0x00000000));
        }

        [Test]
        public void ShiftPbgra32() {
            Assert.AreEqual(0x00ABCDEF, BitmapUtility.ShiftPbgra32(0x00ABCDEF, 2, -4, 0));
            Assert.AreEqual(0xFFADC9EF, BitmapUtility.ShiftPbgra32(0xFFABCDEF, 2, -4, 0));
            Assert.AreEqual(0x80212E40, BitmapUtility.ShiftPbgra32(0x80203040, 2, -4, 0));
        }
    }
}
