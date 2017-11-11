using System;

namespace Tektosyne.Graph {

    /// <summary>
    /// Represents an agent that can navigate an <see cref="IGraph2D{T}"/>.</summary>
    /// <typeparam name="T">
    /// The type of all nodes in the graph. If <typeparamref name="T"/> is a reference type, nodes
    /// cannot be null references.</typeparam>
    /// <remarks>
    /// <b>IGraphAgent</b> represents a mobile object that can navigate the node connections defined
    /// by an <see cref="IGraph2D{T}"/> instance. Both interfaces allow the use of generic search
    /// algorithms, such as <see cref="AStar{T}"/> and <see cref="Coverage{T}"/>, with
    /// application-specific graphs and agents.</remarks>

    public interface IGraphAgent<T> {
        #region RelaxedRange

        /// <summary>
        /// Indicates whether the <see cref="IGraphAgent{T}"/> can enter <see cref="IGraph2D{T}"/>
        /// nodes that exceed the maximum path cost for a movement.</summary>
        /// <value>
        /// <c>true</c> if the <see cref="IGraphAgent{T}"/> may end a movement on an <see
        /// cref="IGraph2D{T}"/> node that exceeds the maximum path cost for the movement;
        /// otherwise, <c>false</c>.</value>
        /// <remarks><para>
        /// The <see cref="AStar{T}"/> and <see cref="Coverage{T}"/> algorithms examine
        /// <b>RelaxedRange</b> when computing the results of the <see cref="AStar{T}.GetLastNode"/>
        /// and <see cref="Coverage{T}.FindReachable"/> methods, respectively.
        /// </para><para>
        /// If <b>RelaxedRange</b> is <c>false</c>, the maximum path cost for a movement is the
        /// absolute upper limit that determines all reachable nodes. The <see
        /// cref="IGraphAgent{T}"/> will not enter any node whose total path cost exceeds this
        /// limit, as determined by <see cref="GetStepCost"/>.
        /// </para><para>
        /// If <b>RelaxedRange</b> is <c>true</c>, the <b>IGraphAgent</b> can enter any node as the
        /// <em>final</em> step of a movement path, regardless of the actual <b>GetStepCost</b>
        /// result for that node, as long as the total path cost of all <em>previous</em> steps is
        /// less than the maximum path cost.</para></remarks>

        bool RelaxedRange { get; }

        #endregion
        #region CanMakeStep

        /// <summary>
        /// Determines whether the <see cref="IGraphAgent{T}"/> can move from one specified <see
        /// cref="IGraph2D{T}"/> node to another neighboring node.</summary>
        /// <param name="source">
        /// The <see cref="IGraph2D{T}"/> node where the move starts.</param>
        /// <param name="target">
        /// The <see cref="IGraph2D{T}"/> node where the move ends. This node must be a neighbor of
        /// <paramref name="source"/>.</param>
        /// <returns>
        /// <c>true</c> if the <see cref="IGraphAgent{T}"/> can move from <paramref name="source"/>
        /// to <paramref name="target"/>; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="source"/> and <paramref name="target"/> are not valid neighboring <see
        /// cref="IGraph2D{T}"/> nodes.</exception>
        /// <remarks><para>
        /// <b>CanMakeStep</b> should only consider whether the <see cref="IGraphAgent{T}"/> could
        /// move to the specified <paramref name="target"/> node <em>if</em> it were already placed
        /// on the specified <paramref name="source"/> node.
        /// </para><para>
        /// <b>CanMakeStep</b> should <em>not</em> consider whether the <b>IGraphAgent</b> could
        /// reach the <paramref name="source"/> or <paramref name="target"/> node from its actual
        /// present <see cref="IGraph2D{T}"/> node, if any.
        /// </para><para>
        /// <b>CanMakeStep</b> should succeed if the <b>PlanarAgent</b> could occupy <paramref
        /// name="target"/> either temporarily or permanently. Use <see cref="CanOccupy"/> to impose
        /// additional restrictions on stopping a movement at specific nodes.</para></remarks>

        bool CanMakeStep(T source, T target);

        #endregion
        #region CanOccupy

        /// <summary>
        /// Determines whether the <see cref="IGraphAgent{T}"/> can permanently occupy the specified
        /// <see cref="IGraph2D{T}"/> node.</summary>
        /// <param name="target">
        /// The <see cref="IGraph2D{T}"/> node to occupy.</param>
        /// <returns>
        /// <c>true</c> if the <see cref="IGraphAgent{T}"/> can permanently occupy <paramref
        /// name="target"/>; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="target"/> is not a valid <see cref="IGraph2D{T}"/> node.</exception>
        /// <remarks><para>
        /// <b>CanOccupy</b> should consider whether the <see cref="IGraphAgent{T}"/> could
        /// <em>permanently</em> occupy the specified <paramref name="target"/> node. Assuming that
        /// the <b>IGraphAgent</b> has already reached <paramref name="target"/>, <b>CanOccupy</b>
        /// determines whether movement could stop at this node.
        /// </para><para>
        /// <b>CanOccupy</b> should <em>not</em> consider whether the <b>IGraphAgent</b> could
        /// temporarily occupy <paramref name="target"/> during a continuing multi-step movement, or
        /// whether <paramref name="target"/> could be reached at all from any other node.
        /// </para><para>
        /// The default implementation of <b>CanOccupy</b> should simply return <c>true</c>.
        /// Pathfinding algorithms always specify a <paramref name="target"/> node for which <see
        /// cref="CanMakeStep"/> has already succeeded, so you should return <c>false</c> only if
        /// you wish to specifically prevent the <b>IGraphAgent</b> from ending a path on the
        /// <paramref name="target"/> node.</para></remarks>

        bool CanOccupy(T target);

        #endregion
        #region GetStepCost

        /// <summary>
        /// Returns the cost for moving the <see cref="IGraphAgent{T}"/> from one specified <see
        /// cref="IGraph2D{T}"/> node to another neighboring node.</summary>
        /// <param name="source">
        /// The <see cref="IGraph2D{T}"/> node where the move starts.</param>
        /// <param name="target">
        /// The <see cref="IGraph2D{T}"/> node where the move ends. This node must be a neighbor of
        /// <paramref name="source"/>.</param>
        /// <returns>
        /// The cost for moving the <see cref="IGraphAgent{T}"/> from <paramref name="source"/> to
        /// <paramref name="target"/>. This value cannot be less than the result of <see
        /// cref="IGraph2D{T}.GetDistance"/> for the two nodes.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="source"/> and <paramref name="target"/> are not valid neighboring <see
        /// cref="IGraph2D{T}"/> nodes.</exception>
        /// <remarks><para>
        /// <b>GetStepCost</b> should not attempt to verify that the <see cref="IGraphAgent{T}"/>
        /// can actually move from the specified <paramref name="source"/> node to the specified
        /// <paramref name="target"/> node. Clients should call <see cref="CanMakeStep"/> to ensure
        /// this condition.
        /// </para><para>
        /// Moreover, <b>GetStepCost</b> should compute the movement cost under the assumption that
        /// the <b>IGraphAgent</b> was already placed on the <paramref name="source"/> node. The
        /// cost of reaching <paramref name="source"/> from its actual present <see
        /// cref="IGraph2D{T}"/> node, if any, should be ignored.</para></remarks>

        double GetStepCost(T source, T target);

        #endregion
        #region IsNearTarget

        /// <summary>
        /// Determines whether the specified <see cref="IGraph2D{T}"/> node is near enough to the
        /// specified target node to be considered equivalent.</summary>
        /// <param name="source">
        /// The <see cref="IGraph2D{T}"/> node to consider.</param>
        /// <param name="target">
        /// The target node within the <see cref="IGraph2D{T}"/>.</param>
        /// <param name="distance">
        /// The distance between <paramref name="source"/> and <paramref name="target"/>, according
        /// to <see cref="IGraph2D{T}.GetDistance"/>. This argument may be negative to indicate that
        /// the <see cref="IGraphAgent{T}"/> should calculate the distance.</param>
        /// <returns>
        /// <c>true</c> if a movement towards <paramref name="target"/> should be considered
        /// complete when <paramref name="source"/> is reached; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="source"/> and <paramref name="target"/> are not valid <see
        /// cref="IGraph2D{T}"/> nodes.</exception>
        /// <remarks><para>
        /// A default implementation of <b>IsNearTarget</b> should merely check whether the
        /// specified <paramref name="source"/> and <paramref name="target"/> nodes are identical.
        /// This can be accomplished by comparing the specified or calculated <paramref
        /// name="distance"/> to zero.
        /// </para><para>
        /// More complex implementations might check for a maximum <paramref name="distance"/>, or
        /// examine application-specific properties of the <paramref name="source"/> and <paramref
        /// name="target"/> nodes.</para></remarks>

        bool IsNearTarget(T source, T target, double distance);

        #endregion
    }
}
