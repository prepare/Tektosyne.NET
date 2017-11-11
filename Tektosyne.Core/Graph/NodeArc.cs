using System;
using System.Diagnostics;

namespace Tektosyne.Graph {

    /// <summary>
    /// Provides the tangential arc and distance of an <see cref="IGraph2D{T}"/> node from a source
    /// node.</summary>
    /// <remarks>
    /// <b>NodeArc</b> encapsulates various data required by the <see cref="Visibility{T}"/>
    /// line-of-sight algorithm. The <see cref="IGraph2D{T}"/> node that defines the <see
    /// cref="NodeArc"/> and the source node that defines the viewpoint are stored separately.
    /// </remarks>

    public class NodeArc {
        #region NodeArc(...)

        /// <summary>
        /// Initializes a new instance of the <see cref="NodeArc"/> class with the specified
        /// tangential arc and distance.</summary>
        /// <param name="start">
        /// The starting angle of the tangential arc from the source node, measured clockwise in
        /// radians from the x-axis, assuming that y-coordinates increase upward.</param>
        /// <param name="sweep">
        /// The positive sweep angle of the tangential arc from the source node, measured clockwise
        /// in radians, assuming that y-coordinates increase upward.</param>
        /// <param name="distance">
        /// The positive distance from the source node to the graph node that defines the tangential
        /// arc, in world coordinates.</param>

        internal NodeArc(double start, double sweep, double distance) {
            Debug.Assert(0 < sweep && sweep <= 2 * Math.PI);
            Debug.Assert(distance > 0);

            Start = start;
            Sweep = sweep;
            Distance = distance;
        }

        #endregion
        #region Internal Fields

        /// <summary>
        /// The visible fraction of the <see cref="NodeArc"/>.</summary>

        internal double _visibleFraction = 1;

        #endregion
        #region Distance

        /// <summary>
        /// The positive distance from the source node to the <see cref="NodeArc"/>, in world
        /// coordinates.</summary>
        /// <remarks>
        /// <b>Distance</b> is measured from the <see cref="IGraph2D{T}.GetWorldLocation"/> result
        /// for the source node to the nearest vertex of the <see
        /// cref="IGraph2D{T}.GetWorldRegion"/> result for the graph node that defines the
        /// tangential arc.</remarks>

        public readonly double Distance;

        #endregion
        #region Start

        /// <summary>
        /// The starting angle of the <see cref="NodeArc"/>, measured clockwise in radians from the
        /// x-axis, assuming that y-coordinates increase upward.</summary>

        public readonly double Start;

        #endregion
        #region Sweep

        /// <summary>
        /// The positive sweep angle of the <see cref="NodeArc"/>, measured clockwise in radians, 
        /// assuming that y-coordinates increase upward.</summary>

        public readonly double Sweep;

        #endregion
        #region VisibleFraction

        /// <summary>
        /// Gets the visible fraction of the <see cref="NodeArc"/>.</summary>
        /// <value>
        /// The fraction of the <see cref="Sweep"/> angle that remains unobscured, from zero to one.
        /// The default is one, indicating that the entire <see cref="NodeArc"/> is visible.</value>
        /// <remarks>
        /// <b>VisibleFraction</b> may be inaccurate if it is smaller than the current <see
        /// cref="Visibility{T}.Threshold"/>, as the <see cref="Visibility{T}"/> algorithm stops
        /// updating a <see cref="NodeArc"/> as soon as it is considered obscured.</remarks>

        public double VisibleFraction {
            [DebuggerStepThrough]
            get { return _visibleFraction; }
        }

        #endregion
        #region IsObscured

        /// <summary>
        /// Determines whether the current <see cref="NodeArc"/> completely obscures the specified
        /// instance, or vice versa.</summary>
        /// <param name="arc">
        /// The <see cref="NodeArc"/> to examine.</param>
        /// <returns><para>
        /// An <see cref="Int32"/> value indicating the relative obscuration of this instance and
        /// <paramref name="arc"/>, as follows:
        /// </para><list type="table"><listheader>
        /// <term>Return Value</term><description>Condition</description>
        /// </listheader><item>
        /// <term>Less than zero</term><description>
        /// <paramref name="arc"/> completely obscures this instance.</description>
        /// </item><item>
        /// <term>Zero</term><description>
        /// Neither instance completely obscures the other.</description>
        /// </item><item>
        /// <term>Greater than zero</term><description>
        /// This instance completely obscures <paramref name="arc"/>.</description>
        /// </item></list></returns>
        /// <remarks>
        /// <b>IsObscured</b> takes the <see cref="Distance"/> of both instances into account to
        /// determine which instance obscures the other, if any.</remarks>

        public int IsObscured(NodeArc arc) {

            // start of specified arc relative to current arc
            double relativeStart = arc.Start - Start;
            if (relativeStart <= -Math.PI)
                relativeStart += 2 * Math.PI;
            else if (relativeStart > Math.PI)
                relativeStart -= 2 * Math.PI;

            Debug.Assert(relativeStart > -Math.PI && relativeStart <= Math.PI);
            double relativeSweep = relativeStart + arc.Sweep;

            // specified arc completely obscures current arc
            if (relativeStart <= 0 && relativeSweep >= Sweep && arc.Distance <= Distance)
                return -1;

            // current arc completely obscures specified arc
            if (relativeStart >= 0 && relativeSweep <= Sweep && arc.Distance > Distance)
                return +1;

            return 0;
        }

        #endregion
        #region Obscure

        /// <summary>
        /// Obscures the specified tangential arc with the <see cref="NodeArc"/>.</summary>
        /// <param name="start"><para>
        /// The starting angle of the tangential arc to obscure, in radians.
        /// </para><para>
        /// On return, possibly increased to the starting angle of the arc that remains visible.
        /// </para></param>
        /// <param name="sweep"><para>
        /// The positive sweep angle of the tangential arc to obscure, in radians.
        /// </para><para>
        /// On return, possibly decreased to the remaining sweep angle of the arc that remains
        /// visible, or to zero if the arc is completely obscured.</para></param>
        /// <remarks>
        /// <b>Obscure</b> assumes that the <see cref="NodeArc"/> is closer to the common source
        /// node than the instance from which the specified <paramref name="start"/> and <paramref
        /// name="sweep"/> were initialized. Clients must ensure this condition holds.</remarks>

        internal void Obscure(ref double start, ref double sweep) {
            Debug.Assert(sweep > 0);

            // start of current arc, relative to specified start
            double relativeStart = Start - start;
            if (relativeStart <= -Math.PI)
                relativeStart += 2 * Math.PI;
            else if (relativeStart > Math.PI)
                relativeStart -= 2 * Math.PI;

            // sweep angle of current arc, relative to specified start
            double relativeSweep = relativeStart + Sweep;

            // check for completely distinct arcs
            if (relativeSweep <= 0 || sweep <= relativeStart)
                return;

            // arc is completely obscured
            if (relativeStart <= 0 && relativeSweep >= sweep) {
                sweep = 0;
                return;
            }

            // start of arc is obscured
            if (relativeStart <= 0) {
                Debug.Assert(relativeSweep < sweep);
                start += relativeSweep;
                sweep -= relativeSweep;
                return;
            }

            // end of arc is obscured
            if (relativeSweep >= sweep) {
                Debug.Assert(relativeStart > 0);
                sweep = relativeStart;
                return;
            }

            // middle of arc is obscured, keep greater visible part
            Debug.Assert(relativeStart > 0 && relativeSweep < sweep);
            if (relativeStart >= sweep - relativeSweep) {
                sweep = relativeStart;
            } else {
                start += relativeSweep;
                sweep -= relativeSweep;
            }
        }

        #endregion
    }
}
