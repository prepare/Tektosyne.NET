using System;
using System.Collections.Generic;
using Tektosyne.Geometry;

namespace Tektosyne.Graph {

    /// <summary>
    /// Provides a navigable graph whose nodes map to regions in two-dimensional space.</summary>
    /// <typeparam name="T">
    /// The type of all nodes in the graph. If <typeparamref name="T"/> is a reference type, nodes
    /// cannot be null references.</typeparam>
    /// <remarks><para>
    /// <b>IGraph2D</b> provides a graph suitable for pathfinding and other generic graph
    /// algorithms. All nodes map to locations and regions in two-dimensional space. Connected nodes
    /// can be navigated by mobile objects, typically represented by an <see cref="IGraphAgent{T}"/>
    /// instance.
    /// </para><para>
    /// <b>IGraph2D</b> makes no assumptions about the underlying topology. Representable topologies
    /// include grids of regular polygons as described by the <see cref="PolygonGrid"/> class, but
    /// also irregular graphs as described by the <see cref="Subdivision"/> class.
    /// </para><note type="implementnotes">
    /// <typeparamref name="T"/> must provide meaningful <see cref="Object.Equals"/> and <see
    /// cref="Object.GetHashCode"/> methods because some graph algorithms store nodes in hashtables.
    /// </note></remarks>

    public interface IGraph2D<T> {
        #region Connectivity

        /// <summary>
        /// Gets the maximum number of direct neighbors for any <see cref="IGraph2D{T}"/> node.
        /// </summary>
        /// <value>
        /// A positive <see cref="Int32"/> value indicating the maximum number of direct neighbors
        /// for any given <see cref="IGraph2D{T}"/> node.</value>
        /// <remarks>
        /// The direct neighbors of any given <see cref="IGraph2D{T}"/> node are those that are
        /// directly connected with the node, without any intermediate nodes.</remarks>

        int Connectivity { get; }

        #endregion
        #region NodeCount

        /// <summary>
        /// Gets the total number of <see cref="Nodes"/> in the <see cref="IGraph2D{T}"/>.</summary>
        /// <value>
        /// The total number of <see cref="Nodes"/> in the <see cref="IGraph2D{T}"/>.</value>
        /// <remarks>
        /// <b>NodeCount</b> never returns a negative value.</remarks>

        int NodeCount { get; }

        #endregion
        #region Nodes

        /// <summary>
        /// Gets an <see cref="IEnumerable{T}"/> collection that contains all nodes in the <see
        /// cref="IGraph2D{T}"/>.</summary>
        /// <value>
        /// An <see cref="IEnumerable{T}"/> collection that contains all nodes in the <see
        /// cref="IGraph2D{T}"/>.</value>
        /// <remarks>
        /// <b>Nodes</b> returns a total of <see cref="NodeCount"/> elements. The enumeration order
        /// depends on the concrete <see cref="IGraph2D{T}"/> instance.</remarks>

        IEnumerable<T> Nodes { get; }

        #endregion
        #region Contains

        /// <summary>
        /// Determines whether the <see cref="IGraph2D{T}"/> contains the specified node.</summary>
        /// <param name="node">
        /// The <see cref="IGraph2D{T}"/> node to examine.</param>
        /// <returns>
        /// <c>true</c> if the <see cref="IGraph2D{T}"/> contains the specified <paramref
        /// name="node"/>; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="node"/> is not a valid <see cref="IGraph2D{T}"/> node.</exception>
        /// <remarks>
        /// <b>Contains</b> should throw an <see cref="ArgumentException"/> only if the specified
        /// <paramref name="node"/> is invalid for any possible <see cref="IGraph2D{T}"/> instance,
        /// e.g. a null reference.</remarks>

        bool Contains(T node);

        #endregion
        #region GetDistance

        /// <summary>
        /// Returns the distance between the two specified <see cref="IGraph2D{T}"/> nodes.
        /// </summary>
        /// <param name="source">
        /// The source node in the <see cref="IGraph2D{T}"/>.</param>
        /// <param name="target">
        /// The target node in the <see cref="IGraph2D{T}"/>.</param>
        /// <returns>
        /// The non-negative distance between <paramref name="source"/> and <paramref
        /// name="target"/>, using world coordinates, movement steps, or another arbitrary measure.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="source"/> or <paramref name="target"/> is not a valid <see
        /// cref="IGraph2D{T}"/> node.</exception>
        /// <remarks><para>
        /// <b>GetDistance</b> must conform to the following invariants:
        /// </para><list type="bullet"><item>
        /// The distance between identical valid nodes is always zero.
        /// </item><item>
        /// The distance between different valid nodes is always positive.
        /// </item><item>
        /// The sum of the distances between all successive nodes within a sequence is never less
        /// than the distance between any two nodes from the same sequence.
        /// </item><item>
        /// The distance between two valid nodes is always equal to or less than the result of <see
        /// cref="IGraphAgent{T}.GetStepCost"/> for the same two nodes.
        /// </item><item>
        /// The distance between two valid nodes remains unchanged if the arguments are reversed.
        /// </item><item>
        /// The distance is undefined if one or both nodes are invalid. In this case only, the
        /// result may be negative.
        /// </item></list><para>
        /// <b>GetDistance</b> should throw an <see cref="ArgumentException"/> only if the specified
        /// <paramref name="source"/> or <paramref name="target"/> is invalid for any possible <see
        /// cref="IGraph2D{T}"/> instance, e.g. a null reference.</para></remarks>

        double GetDistance(T source, T target);

        #endregion
        #region GetNearestNode

        /// <summary>
        /// Gets the <see cref="IGraph2D{T}"/> node that is nearest to the specified location, in
        /// world coordinates.</summary>
        /// <param name="location">
        /// The <see cref="PointD"/> location, in world coordinates, whose nearest <see
        /// cref="IGraph2D{T}"/> node to find.</param>
        /// <returns>
        /// The <see cref="IGraph2D{T}"/> node whose <see cref="GetWorldLocation"/> result is
        /// nearest to the specified <paramref name="location"/>.</returns>
        /// <remarks><para>
        /// <b>GetNearestNode</b> always returns a valid <see cref="IGraph2D{T}"/> node, even if the
        /// distance between its <see cref="GetWorldLocation"/> result and the specified <paramref
        /// name="location"/> is very large.
        /// </para><para>
        /// The node returned by <b>GetNearestNode</b> is not necessarily the one whose <see
        /// cref="GetWorldRegion"/> result contains the specified <paramref name="location"/>.
        /// </para></remarks>

        T GetNearestNode(PointD location);

        #endregion
        #region GetNeighbors

        /// <summary>
        /// Returns all direct neighbors of the specified <see cref="IGraph2D{T}"/> node.</summary>
        /// <param name="node">
        /// The <see cref="IGraph2D{T}"/> node whose direct neighbors to return.</param>
        /// <returns>
        /// An <see cref="IList{T}"/> containing all valid <see cref="IGraph2D{T}"/> nodes that are
        /// directly connected with the specified <paramref name="node"/>. The number of elements is
        /// at most <see cref="Connectivity"/>.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="node"/> is not a valid <see cref="IGraph2D{T}"/> node.</exception>
        /// <remarks><para>
        /// <b>GetNeighbors</b> never returns a null reference, but it returns an empty <see
        /// cref="IList{T}"/> if the specified <paramref name="node"/> or all its neighbors are not
        /// part of the <see cref="IGraph2D{T}"/>.
        /// </para><para>
        /// <b>GetNeighbors</b> returns the complete set of target nodes for which <see
        /// cref="IGraphAgent{T}.CanMakeStep"/> could possibly succeed, assuming the specified
        /// <paramref name="node"/> is the source node.
        /// </para><para>
        /// <b>GetNeighbors</b> should throw an <see cref="ArgumentException"/> only if the
        /// specified <paramref name="node"/> is invalid for any possible <see cref="IGraph2D{T}"/>
        /// instance, e.g. a null reference.</para></remarks>

        IList<T> GetNeighbors(T node);

        #endregion
        #region GetWorldLocation

        /// <summary>
        /// Gets the location of the specified <see cref="IGraph2D{T}"/> node, in world coordinates.
        /// </summary>
        /// <param name="node">
        /// The <see cref="IGraph2D{T}"/> node whose location to return.</param>
        /// <returns>
        /// The <see cref="PointD"/> location of the specified <paramref name="node"/>, in world
        /// coordinates.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="node"/> is not a valid <see cref="IGraph2D{T}"/> node.</exception>
        /// <remarks><para>
        /// <b>GetWorldLocation</b> returns coordinates within an arbitrary "world" coordinate
        /// system whose interpretation depends on the <see cref="IGraph2D{T}"/> instance. The
        /// returned coordinates are the location of the specified <paramref name="node"/> within
        /// the region determined by <see cref="GetWorldRegion"/>.
        /// </para><para>
        /// The result of <see cref="GetDistance"/> for any two nodes is usually the Euclidean
        /// distance between their <b>GetWorldLocation</b> results, but this is not a requirement.
        /// </para></remarks>

        PointD GetWorldLocation(T node);

        #endregion
        #region GetWorldRegion

        /// <summary>
        /// Gets the region covered by the specified <see cref="IGraph2D{T}"/> node, in world
        /// coordinates.</summary>
        /// <param name="node">
        /// The <see cref="IGraph2D{T}"/> node whose region to return.</param>
        /// <returns><para>
        /// An <see cref="Array"/> containing the <see cref="PointD"/> vertices of the polygonal
        /// region covered by the specified <paramref name="node"/>, in world coordinates.
        /// </para><para>-or-</para><para>
        /// A null reference if <paramref name="node"/> does not define a polygonal region.
        /// </para></returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="node"/> is not a valid <see cref="IGraph2D{T}"/> node.</exception>
        /// <remarks><para>
        /// <b>GetWorldRegion</b> returns coordinates within an arbitrary "world" coordinate system
        /// whose interpretation depends on the <see cref="IGraph2D{T}"/> instance.
        /// </para><para>
        /// The returned polygon is implicitly assumed to be closed, with an edge connecting its
        /// last and first vertex. The polygon should be simple and enclose a positive area that
        /// contains the location of the specified <paramref name="node"/>, as determined by <see
        /// cref="GetWorldLocation"/>.</para></remarks>

        PointD[] GetWorldRegion(T node);

        #endregion
    }
}
