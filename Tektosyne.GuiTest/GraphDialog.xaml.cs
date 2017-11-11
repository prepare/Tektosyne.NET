using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

using Tektosyne.Geometry;
using Tektosyne.Graph;

namespace Tektosyne.GuiTest {

    /// <summary>
    /// Provides a <see cref="Window"/> for testing graph algorithms.</summary>
    /// <remarks><para>
    /// <b>GraphDialog</b> tests all available <b>Tektosyne.Graph</b> algorithms on all available
    /// implementations of <see cref="IGraph2D{T}"/>. This includes six different <see
    /// cref="PolygonGrid"/> configurations, and a planar <see cref="Subdivision"/> that represents
    /// a Delaunay triangulation with its dual <see cref="Voronoi"/> diagram.
    /// </para><para>
    /// Each graph node is associated with a random step cost, represented on the screen by shades
    /// of grey. Darker shades equal higher costs, and black nodes are inaccessible and opaque. The
    /// actual step cost is printed on every accessible node. If a graph algorithm succeeds, all
    /// result nodes are marked.</para></remarks>

    public partial class GraphDialog: Window {
        #region GraphDialog()

        public GraphDialog() {
            InitializeComponent();
            _isInitialized = true;
            ShowGraph();
        }

        #endregion
        #region Private Fields

        private IGraphManager _manager;
        private readonly bool _isInitialized;

        #endregion
        #region ShowAlgorithm

        private void ShowAlgorithm(bool randomize) {
            if (!_isInitialized || _manager == null) return;
            object item = AlgorithmCombo.SelectedItem;

            string message = null, general =
                "Source is blue diamond, node costs are green numbers, black is impassable.";

            if (item == AStarAlgorithm) {
                message = _manager.ShowAStar() ?
                    "Red squares indicate best path from source to marked target node." :
                    "Could find no connecting path due to impassable random terrain.";
            }
            else if (item == CoverageAlgorithm) {
                message = _manager.ShowCoverage(randomize) ?
                    "Red squares indicate reachable nodes with a maximum path cost of 10." :
                    "Could find no reachable locations due to impassable random terrain.";
            }
            else if (item == FloodFillAlgorithm) {
                message = _manager.ShowFloodFill(randomize) ?
                    "Red squares indicate connected nodes with up to 1/2 maximum node cost." :
                    "Cound find no matching locations due to impassable random terrain.";
            }
            else if (item == VisibilityAlgorithm) {
                message = _manager.ShowVisibility(randomize) ?
                    "Red squares indicate visible nodes. Impassable nodes block the view." :
                    "Cound find no visible locations due to obscuring random terrain.";
            }

            Description.Text = String.Format("{0}\n{1}", general, message);
            RandomSourceButton.IsEnabled = (item != AStarAlgorithm);
            ThresholdUpDown.Enabled = (item == VisibilityAlgorithm);
        }

        #endregion
        #region ShowGraph

        private void ShowGraph() {
            if (!_isInitialized) return;

            object item = GraphCombo.SelectedItem;
            bool vertexNeighbors = (bool) VertexNeighbors.IsChecked;
            RegularPolygon polygon = null;

            if (item == SquareEdgeGraph)
                polygon = new RegularPolygon(24, 4, PolygonOrientation.OnEdge, vertexNeighbors);
            else if (item == SquareVertexGraph)
                polygon = new RegularPolygon(24, 4, PolygonOrientation.OnVertex, vertexNeighbors);
            else if (item == HexagonEdgeGraph)
                polygon = new RegularPolygon(16, 6, PolygonOrientation.OnEdge);
            else if (item == HexagonVertexGraph)
                polygon = new RegularPolygon(16, 6, PolygonOrientation.OnVertex);

            if (polygon == null) {
                _manager = GraphManager<PointD>.CreateSubdivisionManager(this);
                VertexNeighbors.IsEnabled = false;
            } else {
                _manager = GraphManager<PointI>.CreatePolygonGridManager(this, polygon);
                VertexNeighbors.IsEnabled = (polygon.Sides == 4);
            }

            OutputBox.Children.Clear();
            OutputBox.Children.Add(_manager.Renderer);
            ShowAlgorithm(true);
        }

        #endregion
        #region NewCommandExecuted

        private void NewCommandExecuted(object sender, ExecutedRoutedEventArgs args) {
            args.Handled = true;
            ShowGraph();
        }

        #endregion
        #region OnAlgorithmSelected

        private void OnAlgorithmSelected(object sender, SelectionChangedEventArgs args) {
            args.Handled = true;
            ShowAlgorithm(true);
        }

        #endregion
        #region OnGraphSelected

        private void OnGraphSelected(object sender, SelectionChangedEventArgs args) {
            args.Handled = true;
            ShowGraph();
        }

        #endregion
        #region OnRandomSource

        private void OnRandomSource(object sender, RoutedEventArgs args) {
            args.Handled = true;
            ShowAlgorithm(true);
        }

        #endregion
        #region OnThresholdChanged

        private void OnThresholdChanged(object sender, EventArgs args) {
            ShowAlgorithm(false);
        }

        #endregion
        #region OnVertexNeighbors

        private void OnVertexNeighbors(object sender, RoutedEventArgs args) {
            args.Handled = true;

            // toggle vertex neighbors without changing node costs
            bool vertexNeighbors = (bool) VertexNeighbors.IsChecked;
            if (_manager.SetVertexNeighbors(vertexNeighbors))
                ShowAlgorithm(false);
        }

        #endregion
    }

    /// <summary>
    /// Provides all <see cref="GraphManager{T}"/> functionality required by the <see
    /// cref="GraphDialog"/>.</summary>

    internal interface IGraphManager {

        bool ShowAStar();
        bool ShowCoverage(bool randomize);
        bool ShowFloodFill(bool randomize);
        bool ShowVisibility(bool randomize);

        bool SetVertexNeighbors(bool value);
        FrameworkElement Renderer { get; }
    }

    /// <summary>
    /// Provides a generic randomized <see cref="IGraph2D{T}"/> and <see cref="IGraphAgent{T}"/>
    /// with rendering capability.</summary>

    internal class GraphManager<T>: IGraphManager, IGraphAgent<T> {
        #region GraphManager(...)

        private GraphManager(GraphDialog dialog, int maxCost) {
            Dialog = dialog;
            MaxCost = maxCost;

            Locations = new List<T>();
            Highlights = new List<T>(2);
        }

        #endregion
        #region Fields & Properties

        public readonly GraphDialog Dialog;
        public readonly int MaxCost;

        private IGraph2D<T> _graph;
        private double _scaleCost = 1;
        private enum NodeLocation { TopLeft, BottomRight, Random }

        public IList<T> Highlights { get; private set; }
        public IList<T> Locations { get; private set; }
        public Dictionary<T, Int32> NodeCosts { get; private set; }
        public FrameworkElement Renderer { get; private set; }

        #endregion
        #region Graph

        public IGraph2D<T> Graph {
            get { return _graph; }
            set {
                _graph = value;

                // create random step costs for all nodes
                NodeCosts = new Dictionary<T, Int32>(value.NodeCount);
                foreach (T node in value.Nodes)
                    NodeCosts.Add(node, MersenneTwister.Default.Next(1, MaxCost));
            }
        }

        #endregion
        #region CreatePolygonGridManager

        public static GraphManager<PointI> CreatePolygonGridManager(
            GraphDialog dialog, RegularPolygon polygon) {

            int width = 20, height = 20;
            PolygonGrid grid = new PolygonGrid(polygon);
            grid.Size = new SizeI(width, height);

            // shrink grid until it fits in output box
            double outputWidth = dialog.OutputBox.Width - 16;
            double outputHeight = dialog.OutputBox.Height - 16;

            while (grid.DisplayBounds.Width > outputWidth)
                grid.Size = new SizeI(--width, height);
            while (grid.DisplayBounds.Height > outputHeight)
                grid.Size = new SizeI(width, --height);

            var manager = new GraphManager<PointI>(dialog, 4);
            manager.Graph = grid;
            manager.Renderer = new GraphRenderer<PointI>(manager);
            return manager;
        }

        #endregion
        #region CreateSubdivisionManager

        public static GraphManager<PointD> CreateSubdivisionManager(GraphDialog dialog) {

            RectD output = new RectD(0, 0, dialog.OutputBox.Width, dialog.OutputBox.Height);
            RectD bounds = new RectD(8, 8, output.Width - 16, output.Height - 16);
            PointD[] points = GeoAlgorithms.RandomPoints(40, bounds, new PointDComparerX(), 20);

            VoronoiResults results = Voronoi.FindAll(points, output);
            Subdivision division = results.ToDelaunySubdivision(output, true);

            var manager = new GraphManager<PointD>(dialog, 8);
            manager.Graph = division;
            manager.Renderer = new GraphRenderer<PointD>(manager);

            // scaling factor to keep step costs above node distance
            manager._scaleCost = output.Width + output.Height;
            return manager;
        }

        #endregion
        #region FindNode

        private T FindNode(NodeLocation location) {

            double outputWidth = Dialog.OutputBox.Width;
            double outputHeight = Dialog.OutputBox.Height;

            switch (location) {
                case NodeLocation.TopLeft:
                    return Graph.GetNearestNode(PointD.Empty);

                case NodeLocation.BottomRight:
                    return Graph.GetNearestNode(new PointD(outputWidth, outputHeight));

                default:
                    return Graph.GetNearestNode(new PointD(
                        outputWidth * MersenneTwister.Default.NextDouble(),
                        outputHeight * MersenneTwister.Default.NextDouble()));
            }
        }

        #endregion
        #region FindSource

        private T FindSource(bool randomize) {

            if (!randomize && Highlights.Count > 0)
                return Highlights[0];

            T source = FindNode(NodeLocation.Random);
            Highlights.Clear();
            Highlights.Add(source);

            return source;
        }

        #endregion
        #region SetVertexNeighbors

        public bool SetVertexNeighbors(bool value) {

            PolygonGrid grid = _graph as PolygonGrid;
            if (grid == null) return false;

            RegularPolygon element = grid.Element;
            if (element.Sides != 4 || element.VertexNeighbors == value)
                return false;

            grid.Element = new RegularPolygon(element.Length, 4, element.Orientation, value);
            return true;
        }

        #endregion
        #region ShowAStar

        public bool ShowAStar() {

            // highlight top left node
            T source = FindNode(NodeLocation.TopLeft);
            Highlights.Clear();
            Highlights.Add(source);

            // highlight bottom right node
            T target = FindNode(NodeLocation.BottomRight);
            Highlights.Add(target);

            // find best path from top left to bottom right node
            var aStar = new AStar<T>(Graph);
            aStar.UseWorldDistance = true;
            bool success = aStar.FindBestPath(this, source, target);
            Locations = aStar.Nodes;

            Renderer.InvalidateVisual();
            return success;
        }

        #endregion
        #region ShowCoverage

        public bool ShowCoverage(bool randomize) {

            // find all nodes reachable from source node
            // (note that we must scale the maximum step cost for Subdivision)
            T source = FindSource(randomize);
            var coverage = new Coverage<T>(Graph);

            bool success = coverage.FindReachable(this, source, _scaleCost * 10);
            Locations = coverage.Nodes;

            Renderer.InvalidateVisual();
            return success;
        }

        #endregion
        #region ShowFloodFill

        public bool ShowFloodFill(bool randomize) {

            // find all nodes reachable from source node
            T source = FindSource(randomize);
            var floodFill = new FloodFill<T>(Graph);

            Predicate<T> match = p => NodeCosts[p] <= MaxCost / 2;
            bool success = floodFill.FindMatching(match, source);
            Locations = floodFill.Nodes;

            Renderer.InvalidateVisual();
            return success;
        }

        #endregion
        #region ShowVisibility

        public bool ShowVisibility(bool randomize) {

            // find all nodes visible from source node
            T source = FindSource(randomize);
            var visibility = new Visibility<T>(Graph);
            visibility.Threshold = (double) Dialog.ThresholdUpDown.Value;

            Predicate<T> isOpaque = p => NodeCosts[p] >= MaxCost;
            bool success = visibility.FindVisible(isOpaque, source);
            Locations = visibility.Nodes;

            Renderer.InvalidateVisual();
            return success;
        }

        #endregion
        #region IGraphAgent<T> Members

        public bool RelaxedRange {
            get { return false; }
        }

        public bool CanMakeStep(T source, T target) {
            return (NodeCosts[target] < MaxCost);
        }

        public bool CanOccupy(T target) {
            return true;
        }

        public double GetStepCost(T source, T target) {
            /*
             * Subdivision graphs must scale step costs by world distance because IGraph2D<T>
             * requires that the step cost is never less than the GetDistance result. Step costs
             * must be multiplied with the scaling factor (and not added) so that multiple cheap
             * steps are preferred to a single, more expensive step.
             * 
             * 1. Using the current distance makes pathfinding sensitive to both world distance
             *    and step cost. For best results, we would average out the step costs of source
             *    and target. This corresponds exactly to the visible Voronoi region shading,
             *    as Delaunay edges are always halved by region boundaries.
             * 
             * 2. Using a fixed value that equals or exceeds the maximum possible distance
             *    between any two nodes makes pathfinding sensitive only to assigned step costs.
             *    This effectively replicates the behavior on a PolygonGrid.
             */
            //double distance = Graph.GetDistance(source, target);
            //return (distance * (NodeCosts[source] + NodeCosts[target]) / 2);

            return _scaleCost * NodeCosts[target];
        }

        public bool IsNearTarget(T source, T target, double distance) {
            return (distance == 0);
        }

        #endregion
    }

    /// <summary>
    /// Renders a <see cref="GraphManager{T}"/> to a <see cref="FrameworkElement"/>.</summary>

    internal sealed class GraphRenderer<T>: FrameworkElement {
        #region GraphRenderer(...)

        public GraphRenderer(GraphManager<T> manager) {
            _manager = manager;

            // typeface for step costs
            _typeface = _manager.Dialog.FontFamily.GetTypefaces().ElementAt(0);

            // create darkening shades for increasing costs
            _brushes = new Brush[_manager.MaxCost];
            for (byte i = 0; i < _manager.MaxCost; i++) {
                byte channel = (byte) (255 - (255 * i) / _manager.MaxCost);
                Color color = Color.FromRgb(channel, channel, channel);
                _brushes[i] = new SolidColorBrush(color);
                _brushes[i].Freeze();
            }

            _blackPen = new Pen(Brushes.Black, 1); _blackPen.Freeze();
            _bluePen = new Pen(Brushes.Blue, 1); _bluePen.Freeze();
            _redPen = new Pen(Brushes.Red, 1); _redPen.Freeze();

            // pen for Delaunay subdivision edges
            _edgePen = new Pen(Brushes.Gold, 1);
            _edgePen.DashStyle = DashStyles.Dot;
            _edgePen.Freeze();
        }

        #endregion
        #region Private Fields

        private readonly GraphManager<T> _manager;
        private readonly Brush[] _brushes;

        private readonly Typeface _typeface;
        private readonly Pen _blackPen, _bluePen, _edgePen, _redPen;

        #endregion
        #region MeasureOverride

        protected override Size MeasureOverride(Size availableSize) {
            Size measureSize = availableSize;

            // request size for entire output box
            Canvas outputBox = _manager.Dialog.OutputBox;
            measureSize = new Size(
                Math.Min(outputBox.Width, availableSize.Width),
                Math.Min(outputBox.Height, availableSize.Height));

            return measureSize;
        }

        #endregion
        #region OnRender

        protected override void OnRender(DrawingContext dc) {
            base.OnRender(dc);

            // reserve border around centered polygon grid
            PolygonGrid grid = _manager.Graph as PolygonGrid;
            if (grid != null) {
                Canvas output = _manager.Dialog.OutputBox;
                double borderWidth = (output.Width - grid.DisplayBounds.Width) / 2;
                double borderHeight = (output.Height - grid.DisplayBounds.Height) / 2;
                dc.PushTransform(new TranslateTransform(borderWidth, borderHeight));
            }
            DrawNodes(dc);

            // draw clipped Delaunay edges to indicate connections
            Subdivision division = _manager.Graph as Subdivision;
            if (division != null)
                foreach (SubdivisionEdge edge in division.Edges.Values) {
                    if (edge.Key > edge.Twin.Key) continue;
                    PointD p = edge.Origin.Move(edge.Destination, 10);
                    PointD q = edge.Destination.Move(edge.Origin, 10);
                    dc.DrawLine(_edgePen, p.ToWpfPoint(), q.ToWpfPoint());
                }

            if (grid != null) dc.Pop();
        }

        #endregion
        #region DrawNodes

        private void DrawNodes(DrawingContext dc) {

            foreach (T node in _manager.Graph.Nodes) {
                StreamGeometry geometry = new StreamGeometry();
                using (StreamGeometryContext context = geometry.Open()) {
                    PointD[] region = _manager.Graph.GetWorldRegion(node);

                    context.BeginFigure(region[0].ToWpfPoint(), true, true);
                    for (int i = 1; i < region.Length; i++)
                        context.LineTo(region[i].ToWpfPoint(), true, false);
                }
                geometry.Freeze();

                // draw node region, shaded by step cost
                int cost = _manager.NodeCosts[node];
                dc.DrawGeometry(_brushes[cost - 1], _blackPen, geometry);

                // draw step cost and result markers
                DrawNodeCost(dc, node, cost);
            }
        }

        #endregion
        #region DrawNodeCost

        private void DrawNodeCost(DrawingContext dc, T node, int cost) {

            // translate origin to node location
            PointD location = _manager.Graph.GetWorldLocation(node);
            dc.PushTransform(new TranslateTransform(location.X, location.Y));

            // draw cost if less than maximum
            string costText = (cost < _manager.MaxCost ? cost.ToString() : "—");
            Brush brush = (cost <= _manager.MaxCost / 2 ? Brushes.DarkGreen : Brushes.LightGreen);

            var text = new FormattedText(costText, CultureInfo.GetCultureInfo("en-us"),
                FlowDirection.LeftToRight, _typeface, _manager.Dialog.FontSize, brush);

            dc.DrawText(text, new Point(-text.Width / 2.0, -text.Height / 2.0));

            // indicate found nodes by red rectangles
            if (_manager.Locations.Contains(node))
                dc.DrawRectangle(null, _redPen, new Rect(-7, -7, 14, 14));

            // indicate highlights by blue diamonds
            if (_manager.Highlights.Contains(node)) {
                dc.PushTransform(new RotateTransform(45.0));
                dc.DrawRectangle(null, _bluePen, new Rect(-9, -9, 18, 18));
                dc.Pop();
            }

            dc.Pop();
        }

        #endregion
    }
}
