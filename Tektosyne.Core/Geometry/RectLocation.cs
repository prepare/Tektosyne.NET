using System;

namespace Tektosyne.Geometry {

    /// <summary>
    /// Specifies the location of a point relative to a rectangle.</summary>
    /// <remarks>
    /// Every valid <b>RectLocation</b> value is a bitwise combination of an <b>…X</b> value and a
    /// <b>…Y</b> value, indicating the location of each point coordinate relative to the 
    /// corresponding nearest edge of the rectangle.</remarks>

    [Flags]
    public enum RectLocation {

        /// <summary>
        /// Specifies that the point’s location is unknown or does not exist.</summary>

        None = 0,

        /// <summary>
        /// Specifies that the point’s x-coordinate is located before the rectangle’s smallest
        /// x-coordinate.</summary>

        BeforeX = 1,

        /// <summary>
        /// Specifies that the point’s x-coordinate equals the rectangle’s smallest x-coordinate.
        /// </summary>

        StartX = 2,

        /// <summary>
        /// Specifies that the point’s x-coordinate is located between the rectangle’s smallest and
        /// greatest x-coordinate, exclusively.</summary>

        InsideX = 4,

        /// <summary>
        /// Specifies that the point’s x-coordinate equals the rectangle’s greatest x-coordinate.
        /// </summary>

        EndX = 8,

        /// <summary>
        /// Specifies that the point’s x-coordinate is located after the rectangle’s greatest
        /// x-coordinate.</summary>

        AfterX = 16,

        /// <summary>
        /// Specifies that the point’s y-coordinate is located before the rectangle’s smallest
        /// y-coordinate.</summary>

        BeforeY = 32,

        /// <summary>
        /// Specifies that the point’s y-coordinate equals the rectangle’s smallest y-coordinate.
        /// </summary>

        StartY = 64,

        /// <summary>
        /// Specifies that the point’s y-coordinate is located between the rectangle’s smallest and
        /// greatest y-coordinate, exclusively.</summary>

        InsideY = 128,

        /// <summary>
        /// Specifies that the point’s y-coordinate equals the rectangle’s greatest y-coordinate.
        /// </summary>

        EndY = 256,

        /// <summary>
        /// Specifies that the point’s y-coordinate is located after the rectangle’s greatest
        /// y-coordinate.</summary>

        AfterY = 512
    }
}
