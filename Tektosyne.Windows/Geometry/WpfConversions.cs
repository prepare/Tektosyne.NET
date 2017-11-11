using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace Tektosyne.Geometry {

    /// <summary>
    /// Provides conversions between geometric primitives in <b>Tektosyne.Geometry</b> and
    /// <b>System.Windows</b>.</summary>

    public static class WpfConversions {
        #region ToWpfPoint

        /// <summary>
        /// Converts the specified <see cref="PointD"/> to a WPF <see cref="Point"/>.</summary>
        /// <param name="point">
        /// The <see cref="PointD"/> instance to convert.</param>
        /// <returns>
        /// A new WPF <see cref="Point"/> instance whose coordinates equal those of the specified
        /// <paramref name="point"/>.</returns>

        public static Point ToWpfPoint(this PointD point) {
            return new Point(point.X, point.Y);
        }

        #endregion
        #region ToWpfPoints(PointD[])

        /// <overloads>
        /// Converts the specified <see cref="PointD"/> collection to a WPF <see
        /// cref="PointCollection"/>.</overloads>
        /// <summary>
        /// Converts the specified <see cref="Array"/> of <see cref="PointD"/> instances to a WPF
        /// <see cref="PointCollection"/>.</summary>
        /// <param name="points">
        /// An <see cref="Array"/> containing the <see cref="PointD"/> instances to convert.</param>
        /// <returns>
        /// A new <see cref="PointCollection"/> containing WPF <see cref="Point"/> instances whose
        /// coordinates equal those of the specified <paramref name="points"/> element at the same
        /// index position.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="points"/> is a null reference.</exception>

        public static PointCollection ToWpfPoints(this PointD[] points) {
            if (points == null)
                ThrowHelper.ThrowArgumentNullException("points");

            var collection = new PointCollection(points.Length);
            foreach (var point in points)
                collection.Add(new Point(point.X, point.Y));

            return collection;
        }

        #endregion
        #region ToWpfPoints(ICollection<PointD>)

        /// <summary>
        /// Converts the specified <see cref="ICollection{T}"/> of <see cref="PointD"/> instances to
        /// a WPF <see cref="PointCollection"/>.</summary>
        /// <param name="points">
        /// An <see cref="ICollection{T}"/> containing the <see cref="PointD"/> instances to
        /// convert.</param>
        /// <returns>
        /// A new <see cref="PointCollection"/> containing WPF <see cref="Point"/> instances
        /// whose coordinates equal those of the specified <paramref name="points"/> element at the
        /// same index position.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="points"/> is a null reference.</exception>

        public static PointCollection ToWpfPoints(this ICollection<PointD> points) {
            if (points == null)
                ThrowHelper.ThrowArgumentNullException("points");

            var collection = new PointCollection(points.Count);
            foreach (var point in points)
                collection.Add(new Point(point.X, point.Y));

            return collection;
        }

        #endregion
        #region ToWpfVector

        /// <summary>
        /// Converts the specified <see cref="PointD"/> to a WPF <see cref="Vector"/>.</summary>
        /// <param name="vector">
        /// The <see cref="PointD"/> instance to convert.</param>
        /// <returns>
        /// A new WPF <see cref="Vector"/> instance whose coordinates equal those of the specified
        /// <paramref name="vector"/>.</returns>

        public static Vector ToWpfVector(this PointD vector) {
            return new Vector(vector.X, vector.Y);
        }

        #endregion
        #region ToWpfSize

        /// <summary>
        /// Converts the specified <see cref="SizeD"/> to a WPF <see cref="Size"/>.</summary>
        /// <param name="size">
        /// The <see cref="SizeD"/> instance to convert.</param>
        /// <returns>
        /// A new WPF <see cref="Size"/> instance whose dimensions equal those of the specified
        /// <paramref name="size"/>.</returns>

        public static Size ToWpfSize(this SizeD size) {
            return new Size(size.Width, size.Height);
        }

        #endregion
        #region ToWpfRect

        /// <summary>
        /// Converts the specified <see cref="RectD"/> to a WPF <see cref="Rect"/>.</summary>
        /// <param name="rect">
        /// The <see cref="RectD"/> instance to convert.</param>
        /// <returns>
        /// A new WPF <see cref="Rect"/> instance whose coordinates and dimensions equal those of
        /// the specified <paramref name="rect"/>.</returns>

        public static Rect ToWpfRect(this RectD rect) {
            return new Rect(rect.X, rect.Y, rect.Width, rect.Height);
        }

        #endregion
        #region ToInt32Rect

        /// <summary>
        /// Converts the specified <see cref="RectI"/> to a WPF <see cref="Int32Rect"/>.</summary>
        /// <param name="rect">
        /// The <see cref="RectI"/> instance to convert.</param>
        /// <returns>
        /// A new WPF <see cref="Int32Rect"/> instance whose coordinates and dimensions equal those
        /// of the specified <paramref name="rect"/>.</returns>

        public static Int32Rect ToInt32Rect(this RectI rect) {
            return new Int32Rect(rect.X, rect.Y, rect.Width, rect.Height);
        }

        #endregion
        #region ToPointD(Point)

        /// <overloads>
        /// Converts the specified WPF coordinates to a <see cref="PointD"/>.</overloads>
        /// <summary>
        /// Converts the specified WPF <see cref="Point"/> to a <see cref="PointD"/>.</summary>
        /// <param name="point">
        /// The WPF <see cref="Point"/> instance to convert.</param>
        /// <returns>
        /// A new <see cref="PointD"/> instance whose coordinates equal those of the specified
        /// <paramref name="point"/>.</returns>

        public static PointD ToPointD(this Point point) {
            return new PointD(point.X, point.Y);
        }

        #endregion
        #region ToPointD(Vector)

        /// <summary>
        /// Converts the specified WPF <see cref="Vector"/> to a <see cref="PointD"/>.</summary>
        /// <param name="vector">
        /// The WPF <see cref="Vector"/> instance to convert.</param>
        /// <returns>
        /// A new <see cref="PointD"/> instance whose coordinates equal those of the specified
        /// <paramref name="vector"/>.</returns>

        public static PointD ToPointD(this Vector vector) {
            return new PointD(vector.X, vector.Y);
        }

        #endregion
        #region ToSizeD

        /// <summary>
        /// Converts the specified WPF <see cref="Size"/> to a <see cref="SizeD"/>.</summary>
        /// <param name="size">
        /// The WPF <see cref="Size"/> instance to convert.</param>
        /// <returns>
        /// A new <see cref="SizeD"/> instance whose dimensions equal those of the specified
        /// <paramref name="size"/>.</returns>

        public static SizeD ToSizeD(this Size size) {
            return new SizeD(size.Width, size.Height);
        }

        #endregion
        #region ToRectD

        /// <summary>
        /// Converts the specified WPF <see cref="Rect"/> to a <see cref="RectD"/>.</summary>
        /// <param name="rect">
        /// The WPF <see cref="Rect"/> instance to convert.</param>
        /// <returns>
        /// A new <see cref="RectD"/> instance whose coordinates and dimensions equal those of the
        /// specified <paramref name="rect"/>.</returns>

        public static RectD ToRectD(this Rect rect) {
            return new RectD(rect.X, rect.Y, rect.Width, rect.Height);
        }

        #endregion
        #region ToRectI

        /// <summary>
        /// Converts the specified WPF <see cref="Int32Rect"/> to a <see cref="RectI"/>.</summary>
        /// <param name="rect">
        /// The WPF <see cref="Int32Rect"/> instance to convert.</param>
        /// <returns>
        /// A new <see cref="RectI"/> instance whose coordinates and dimensions equal those of the
        /// specified <paramref name="rect"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="rect"/> contains a <see cref="Int32Rect.Width"/> or <see
        /// cref="Int32Rect.Height"/> that is less than zero.</exception>

        public static RectI ToRectI(this Int32Rect rect) {
            return new RectI(rect.X, rect.Y, rect.Width, rect.Height);
        }

        #endregion
    }
}
