using System;
using System.Drawing;

using GdiPointF = System.Drawing.PointF;
using GdiSizeF = System.Drawing.SizeF;

namespace Tektosyne.Geometry {

    /// <summary>
    /// Provides conversions between geometric primitives in <b>Tektosyne.Geometry</b> and
    /// <b>System.Drawing</b>.</summary>

    public static class GdiConversions {
        #region ToGdiPoint

        /// <summary>
        /// Converts the specified <see cref="PointI"/> to a GDI <see cref="Point"/>.</summary>
        /// <param name="point">
        /// The <see cref="PointI"/> instance to convert.</param>
        /// <returns>
        /// A new GDI <see cref="Point"/> instance whose coordinates equal those of the specified
        /// <paramref name="point"/>.</returns>

        public static Point ToGdiPoint(this PointI point) {
            return new Point(point.X, point.Y);
        }

        #endregion
        #region ToGdiSize

        /// <summary>
        /// Converts the specified <see cref="SizeI"/> to a GDI <see cref="Size"/>.</summary>
        /// <param name="size">
        /// The <see cref="SizeI"/> instance to convert.</param>
        /// <returns>
        /// A new GDI <see cref="Size"/> instance whose dimensions equal those of the specified
        /// <paramref name="size"/>.</returns>

        public static Size ToGdiSize(this SizeI size) {
            return new Size(size.Width, size.Height);
        }

        #endregion
        #region ToGdiRect

        /// <summary>
        /// Converts the specified <see cref="RectI"/> to a GDI <see cref="Rectangle"/>.</summary>
        /// <param name="rect">
        /// The <see cref="RectI"/> instance to convert.</param>
        /// <returns>
        /// A new GDI <see cref="Rectangle"/> instance whose coordinates and dimensions equal those
        /// of the specified <paramref name="rect"/>.</returns>

        public static Rectangle ToGdiRect(this RectI rect) {
            return new Rectangle(rect.X, rect.Y, rect.Width, rect.Height);
        }

        #endregion
        #region ToPointI

        /// <summary>
        /// Converts the specified GDI <see cref="Point"/> to a <see cref="PointI"/>.</summary>
        /// <param name="point">
        /// The GDI <see cref="Point"/> instance to convert.</param>
        /// <returns>
        /// A new <see cref="PointI"/> instance whose coordinates equal those of the specified
        /// <paramref name="point"/>.</returns>

        public static PointI ToPointI(this Point point) {
            return new PointI(point.X, point.Y);
        }

        #endregion
        #region ToSizeI

        /// <summary>
        /// Converts the specified GDI <see cref="Size"/> to a <see cref="SizeI"/>.</summary>
        /// <param name="size">
        /// The GDI <see cref="Size"/> instance to convert.</param>
        /// <returns>
        /// A new <see cref="SizeI"/> instance whose dimensions equal those of the specified
        /// <paramref name="size"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="size"/> contains a <see cref="Size.Width"/> or <see
        /// cref="Size.Height"/> that is less than zero.</exception>

        public static SizeI ToSizeI(this Size size) {
            return new SizeI(size.Width, size.Height);
        }

        #endregion
        #region ToRectI

        /// <summary>
        /// Converts the specified GDI <see cref="Rectangle"/> to a <see cref="RectI"/>.</summary>
        /// <param name="rect">
        /// The GDI <see cref="Rectangle"/> instance to convert.</param>
        /// <returns>
        /// A new <see cref="RectI"/> instance whose coordinates and dimensions equal those of the
        /// specified <paramref name="rect"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="rect"/> contains a <see cref="Rectangle.Width"/> or <see
        /// cref="Rectangle.Height"/> that is less than zero.</exception>

        public static RectI ToRectI(this Rectangle rect) {
            return new RectI(rect.X, rect.Y, rect.Width, rect.Height);
        }

        #endregion
        #region ToGdiPointF

        /// <summary>
        /// Converts the specified <see cref="PointF"/> to a GDI <see cref="GdiPointF"/>.</summary>
        /// <param name="point">
        /// The <see cref="PointF"/> instance to convert.</param>
        /// <returns>
        /// A new GDI <see cref="GdiPointF"/> instance whose coordinates equal those of the
        /// specified <paramref name="point"/>.</returns>

        public static GdiPointF ToGdiPointF(this PointF point) {
            return new GdiPointF(point.X, point.Y);
        }

        #endregion
        #region ToGdiSizeF

        /// <summary>
        /// Converts the specified <see cref="SizeF"/> to a GDI <see cref="GdiSizeF"/>.</summary>
        /// <param name="size">
        /// The <see cref="SizeF"/> instance to convert.</param>
        /// <returns>
        /// A new GDI <see cref="GdiSizeF"/> instance whose dimensions equal those of the specified
        /// <paramref name="size"/>.</returns>

        public static GdiSizeF ToGdiSizeF(this SizeF size) {
            return new GdiSizeF(size.Width, size.Height);
        }

        #endregion
        #region ToGdiRectF

        /// <summary>
        /// Converts the specified <see cref="RectF"/> to a GDI <see cref="RectangleF"/>.</summary>
        /// <param name="rect">
        /// The <see cref="RectF"/> instance to convert.</param>
        /// <returns>
        /// A new GDI <see cref="RectangleF"/> instance whose coordinates and dimensions equal those
        /// of the specified <paramref name="rect"/>.</returns>

        public static RectangleF ToGdiRectF(this RectF rect) {
            return new RectangleF(rect.X, rect.Y, rect.Width, rect.Height);
        }

        #endregion
        #region ToPointF

        /// <summary>
        /// Converts the specified GDI <see cref="GdiPointF"/> to a <see cref="PointF"/>.</summary>
        /// <param name="point">
        /// The GDI <see cref="GdiPointF"/> instance to convert.</param>
        /// <returns>
        /// A new <see cref="PointF"/> instance whose coordinates equal those of the specified
        /// <paramref name="point"/>.</returns>

        public static PointF ToPointF(this GdiPointF point) {
            return new PointF(point.X, point.Y);
        }

        #endregion
        #region ToSizeF

        /// <summary>
        /// Converts the specified GDI <see cref="GdiSizeF"/> to a <see cref="SizeF"/>.</summary>
        /// <param name="size">
        /// The GDI <see cref="GdiSizeF"/> instance to convert.</param>
        /// <returns>
        /// A new <see cref="SizeF"/> instance whose dimensions equal those of the specified
        /// <paramref name="size"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="size"/> contains a <see cref="GdiSizeF.Width"/> or <see
        /// cref="GdiSizeF.Height"/> that is less than zero.</exception>

        public static SizeF ToSizeF(this GdiSizeF size) {
            return new SizeF(size.Width, size.Height);
        }

        #endregion
        #region ToRectF

        /// <summary>
        /// Converts the specified GDI <see cref="RectangleF"/> to a <see cref="RectF"/>.</summary>
        /// <param name="rect">
        /// The GDI <see cref="RectangleF"/> instance to convert.</param>
        /// <returns>
        /// A new <see cref="RectF"/> instance whose coordinates and dimensions equal those of the
        /// specified <paramref name="rect"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="rect"/> contains a <see cref="RectangleF.Width"/> or <see
        /// cref="RectangleF.Height"/> that is less than zero.</exception>

        public static RectF ToRectF(this RectangleF rect) {
            return new RectF(rect.X, rect.Y, rect.Width, rect.Height);
        }

        #endregion
    }
}
