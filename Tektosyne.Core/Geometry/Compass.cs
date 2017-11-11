using System;

namespace Tektosyne.Geometry {

    /// <summary>
    /// Specifies the eight major compass directions.</summary>
    /// <remarks><para>
    /// <b>Compass</b> specifies the four cardinal directions of the compass, and the four ordinal
    /// directions halfway between the cardinal points.
    /// </para><para>
    /// Each <b>Compass</b> direction is assigned an <see cref="Int32"/> value that equals its 
    /// compass angle, starting with zero degrees for <see cref="Compass.North"/> and continuing
    /// clockwise in 45 degree increments.</para></remarks>

    public enum Compass {

        /// <summary>Specifies zero degrees.</summary>
        North = 0,

        /// <summary>Specifies 45 degrees.</summary>
        NorthEast = 45,

        /// <summary>Specifies 90 degrees.</summary>
        East = 90,

        /// <summary>Specifies 135 degrees.</summary>
        SouthEast = 135,

        /// <summary>Specifies 180 degrees.</summary>
        South = 180,

        /// <summary>Specifies 225 degrees.</summary>
        SouthWest = 225,

        /// <summary>Specifies 270 degrees.</summary>
        West = 270,

        /// <summary>Specifies 315 degrees.</summary>
        NorthWest = 315
    }
}
