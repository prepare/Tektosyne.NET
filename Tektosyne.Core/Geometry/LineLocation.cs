using System;

namespace Tektosyne.Geometry {

    /// <summary>
    /// Specifies the location of a point relative to a directed line segment.</summary>
    /// <remarks>
    /// All <b>LineLocation</b> values except <see cref="LineLocation.None"/> are powers of two,
    /// even though the enumeration does not have the <see cref="FlagsAttribute"/>. This allows
    /// using a bit mask for efficient testing of multiple alternative values.</remarks>

    public enum LineLocation {

        /// <summary>
        /// Specifies that the point’s location is unknown or does not exist.</summary>

        None = 0,

        /// <summary>
        /// Specifies that the point is collinear with the line segment and located before its start
        /// point on its infinite extension.</summary>

        Before = 1,

        /// <summary>
        /// Specifies that the point coincides with the start point.</summary>

        Start = 2,

        /// <summary>
        /// Specifies that the point is collinear with the line segment and located between its
        /// start and end point, exclusively.</summary>

        Between = 4,

        /// <summary>
        /// Specifies that the point coincides with the end point.</summary>

        End = 8,

        /// <summary>
        /// Specifies that the point is collinear with the line segment and located after its end
        /// point on its infinite extension.</summary>

        After = 16,

        /// <summary>
        /// Specifies that the point is not collinear with the line segment and located to the left
        /// of its infinite extension, viewed from start point to end point.</summary>

        Left = 32,

        /// <summary>
        /// Specifies that the point is not collinear with the line segment and located to the right
        /// of its infinite extension, viewed from start point to end point.</summary>

        Right = 64
    }
}
