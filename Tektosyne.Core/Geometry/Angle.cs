using System;

namespace Tektosyne.Geometry {

    /// <summary>
    /// Provides constants and methods to manipulate angles in radians and degrees.</summary>

    public static class Angle {
        #region DegreesToRadians

        /// <summary>
        /// The factor that converts an angle from degrees to radians.</summary>
        /// <remarks>
        /// <b>DegreesToRadians</b> holds the value <see cref="Math.PI"/> / 180.</remarks>

        public const double DegreesToRadians = Math.PI / 180;

        #endregion
        #region RadiansToDegrees

        /// <summary>
        /// The factor that converts an angle from radians to degrees.</summary>
        /// <remarks>
        /// <b>RadiansToDegrees</b> holds the value 180 / <see cref="Math.PI"/>.</remarks>

        public const double RadiansToDegrees = 180 / Math.PI;

        #endregion
        #region DegreesToCompass

        /// <summary>
        /// Converts the specified angle, in degrees, to the nearest <see cref="Compass"/>
        /// direction.</summary>
        /// <param name="degrees">
        /// The angle to convert, in degrees. This value is taken <see cref="Fortran.Modulo"/> 360
        /// degrees, and may therefore be outside the interval [0, 360).</param>
        /// <returns>
        /// The <see cref="Compass"/> direction nearest to the specified <paramref name="degrees"/>.
        /// </returns>
        /// <remarks>
        /// The specified <paramref name="degrees"/> are measured in the same way as <see
        /// cref="Compass"/> directions, i.e. starting at zero for <see cref="Compass.North"/> and
        /// increasing clockwise.</remarks>

        public static Compass DegreesToCompass(double degrees) {
            degrees = Fortran.Modulo(degrees + 22.5, 360);
            int point = (int) (degrees / 45);
            return (Compass) (point * 45);
        }

        #endregion
        #region DistanceDegrees

        /// <summary>
        /// Finds the shortest distance between two specified normalized angles, in degrees.
        /// </summary>
        /// <param name="start">
        /// The angle from which the distance is measured, in normalized degrees.</param>
        /// <param name="end">
        /// The angle to which the distance is measured, in normalized degrees.</param>
        /// <returns>
        /// The shortest distance from <paramref name="start"/> to <paramref name="end"/>, in signed
        /// degrees.</returns>
        /// <remarks><para>
        /// <b>DistanceDegrees</b> requires that the specified <paramref name="start"/> and
        /// <paramref name="end"/> have been normalized to the half-open interval [0, 360), e.g.
        /// using <see cref="NormalizeDegrees"/>.
        /// </para><para>
        /// If so, <b>DistanceDegrees</b> returns the value within the half-open interval (-180,
        /// +180] that solves the following equation:
        /// </para><para>
        /// <paramref name="end"/> = <see cref="NormalizeDegrees"/>(<paramref name="start"/> + 
        /// <b>DistanceDegrees</b>(<paramref name="start"/>, <paramref name="end"/>))
        /// </para></remarks>

        public static double DistanceDegrees(double start, double end) {
            double dist = end - start;

            if (dist > 180)
                dist -= 360;
            else if (dist <= -180)
                dist += 360;

            return dist;
        }

        #endregion
        #region DistanceRadians

        /// <summary>
        /// Finds the shortest distance between two specified normalized angles, in radians.
        /// </summary>
        /// <param name="start">
        /// The angle from which the distance is measured, in normalized radians.</param>
        /// <param name="end">
        /// The angle to which the distance is measured, in normalized radians.</param>
        /// <returns>
        /// The shortest distance from <paramref name="start"/> to <paramref name="end"/>, in signed
        /// radians.</returns>
        /// <remarks><para>
        /// <b>DistanceRadians</b> requires that the specified <paramref name="start"/> and
        /// <paramref name="end"/> have been normalized to the half-open interval [0, 2 <see
        /// cref="Math.PI"/>), e.g. using <see cref="NormalizeRadians"/>.
        /// </para><para>
        /// If so, <b>DistanceRadians</b> returns the value within the half-open interval (-<see
        /// cref="Math.PI"/>, +<see cref="Math.PI"/>] that solves the following equation:
        /// </para><para>
        /// <paramref name="end"/> = <see cref="NormalizeRadians"/>(<paramref name="start"/> + 
        /// <b>DistanceRadians</b>(<paramref name="start"/>, <paramref name="end"/>))
        /// </para></remarks>

        public static double DistanceRadians(double start, double end) {
            double dist = end - start;

            if (dist > Math.PI)
                dist -= 2 * Math.PI;
            else if (dist <= -Math.PI)
                dist += 2 * Math.PI;

            return dist;
        }

        #endregion
        #region NormalizeDegrees

        /// <summary>
        /// Normalizes the specified angle, in degrees, to the interval [0, 360).</summary>
        /// <param name="degrees">
        /// The angle to normalize, in degrees.</param>
        /// <returns>
        /// The specified <paramref name="degrees"/> normalized to the half-open interval [0, 360).
        /// </returns>

        public static double NormalizeDegrees(double degrees) {
            degrees %= 360;
            if (degrees < 0) degrees += 360;
            return degrees;
        }

        #endregion
        #region NormalizeRoundedDegrees

        /// <summary>
        /// Normalizes the specified angle, in degrees, to the interval [0, 360) after rounding to
        /// the nearest <see cref="Int32"/> number.</summary>
        /// <param name="degrees">
        /// The angle to normalize, in degrees.</param>
        /// <returns>
        /// The specified <paramref name="degrees"/> rounded to the nearest <see cref="Int32"/>
        /// number and normalized to the half-open interval [0, 360).</returns>
        /// <remarks>
        /// <b>NormalizeRoundedDegrees</b> uses <see cref="Fortran.NInt"/> to round the specified
        /// <paramref name="degrees"/> before normalization. The result is guaranteed to be an <see
        /// cref="Int32"/> value between 0 and 359.</remarks>

        public static int NormalizeRoundedDegrees(double degrees) {
            int angle = Fortran.NInt(degrees);
            angle %= 360;
            if (angle < 0) angle += 360;
            return angle;
        }

        #endregion
        #region NormalizeRadians

        /// <summary>
        /// Normalizes the specified angle, in radians, to the interval [0, 2 <see
        /// cref="Math.PI"/>).</summary>
        /// <param name="radians">
        /// The angle to normalize, in radians.</param>
        /// <returns>
        /// The specified <paramref name="radians"/> normalized to the half-open interval [0, 2 <see
        /// cref="Math.PI"/>).</returns>

        public static double NormalizeRadians(double radians) {
            radians %= 2 * Math.PI;
            if (radians < 0) radians += 2 * Math.PI;
            return radians;
        }

        #endregion
    }
}
