using System;
using System.Collections.Generic;
using System.Windows.Media;

namespace Tektosyne.Geometry {

    /// <summary>
    /// Provides extension methods that render the <see cref="RegularPolygon"/> and <see
    /// cref="PolygonGrid"/> classes to <b>System.Windows</b> types.</summary>

    public static class PolygonExtensions {
        #region Draw(RegularPolygon, ...)

        /// <overloads>
        /// Draws the specified <see cref="RegularPolygon"/> or <see cref="PolygonGrid"/> to the
        /// specified <see cref="StreamGeometry"/>.</overloads>
        /// <summary>
        /// Draws the specified <see cref="RegularPolygon"/> to the specified <see
        /// cref="StreamGeometry"/>.</summary>
        /// <param name="polygon">
        /// The <see cref="RegularPolygon"/> to draw.</param>
        /// <param name="context">
        /// The <see cref="StreamGeometryContext"/> that receives <paramref name="polygon"/>.
        /// </param>
        /// <param name="offset">
        /// The offset by which to shift <paramref name="polygon"/>.</param>
        /// <param name="isFilled">
        /// <c>true</c> to use the area covered by <paramref name="polygon"/> for hit-testing,
        /// rendering, and clipping; otherwise, <c>false</c>.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="polygon"/> or <paramref name="context"/> is a null reference.
        /// </exception>
        /// <remarks>
        /// <b>Draw</b> issues one <see cref="StreamGeometryContext.BeginFigure"/> call with the
        /// specified <paramref name="isFilled"/> parameter for the first <see
        /// cref="RegularPolygon.Vertices"/> element of the specified <paramref name="polygon"/>,
        /// and then one <see cref="StreamGeometryContext.LineTo"/> call for each remaining element.
        /// All coordinates are shifted by the specified <paramref name="offset"/>.</remarks>

        public static void Draw(this RegularPolygon polygon,
            StreamGeometryContext context, PointD offset, bool isFilled) {

            PointD[] vertices = polygon.Vertices;
            PointD start = vertices[0] + offset;
            context.BeginFigure(start.ToWpfPoint(), isFilled, true);

            for (int i = 1; i < vertices.Length; i++) {
                PointD next = vertices[i] + offset;
                context.LineTo(next.ToWpfPoint(), true, true);
            }
        }

        #endregion
        #region Draw(PolygonGrid, ...)

        /// <summary>
        /// Draws the specified <see cref="PolygonGrid"/> to the specified <see
        /// cref="StreamGeometry"/>.</summary>
        /// <param name="grid">
        /// The <see cref="PolygonGrid"/> to draw.</param>
        /// <param name="context">
        /// The <see cref="StreamGeometryContext"/> that receives <paramref name="grid"/>.</param>
        /// <param name="isFilled">
        /// <c>true</c> to use the area covered by <paramref name="grid"/> for hit-testing,
        /// rendering, and clipping; otherwise, <c>false</c>.</param>
        /// <param name="offset">
        /// The offset by which to shift <paramref name="grid"/>.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="grid"/> is a null reference.</exception>
        /// <remarks><para>
        /// <b>Draw</b> invokes the <see cref="RegularPolygon"/> overload of <see cref="Draw"/> for
        /// each <see cref="PolygonGrid.Element"/> in the specified <paramref name="grid"/>, shifted
        /// by the corresponding <see cref="PolygonGrid.GridToDisplay"/> result. Each polygonal <see
        /// cref="PolygonGrid.Element"/> is therefore represented by one figure.
        /// </para><para>
        /// All coordinates are also shifted by the specified <paramref name="offset"/>.
        /// </para></remarks>

        public static void Draw(this PolygonGrid grid,
            StreamGeometryContext context, PointD offset, bool isFilled) {

            int width = grid.Size.Width, height = grid.Size.Height;
            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++) {
                    PointD element = grid.GridToDisplay(x, y);
                    grid.Element.Draw(context, element + offset, isFilled);
                }
        }

        #endregion
        #region DrawOptimized

        /// <summary>
        /// Draws the specified <see cref="PolygonGrid"/> to the specified <see
        /// cref="StreamGeometry"/>, with optimization to remove duplicate lines.</summary>
        /// <param name="grid">
        /// The <see cref="PolygonGrid"/> to draw.</param>
        /// <param name="context">
        /// The <see cref="StreamGeometryContext"/> that receives <paramref name="grid"/>.</param>
        /// <param name="offset">
        /// The offset by which to shift <paramref name="grid"/>.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="grid"/> or <paramref name="context"/> is a null reference.</exception>
        /// <remarks><para>
        /// <b>DrawOptimized</b> shifts the <see cref="RegularPolygon.Vertices"/> of each <see
        /// cref="PolygonGrid.Element"/> in the specified <paramref name="grid"/> by the
        /// corresponding <see cref="PolygonGrid.GridToDisplay"/> result plus the specified
        /// <paramref name="offset"/>.
        /// </para><para>
        /// <b>DrawOptimized</b> skips any <see cref="RegularPolygon"/> edges that coincide with an
        /// edge drawn for a previous <see cref="PolygonGrid.Element"/>. This optimization results
        /// in a total number of lines drawn to the specified <paramref name="context"/> that is
        /// about half that of the unoptimized <see cref="Draw"/> method.
        /// </para><para>
        /// There are two disadvantages. First, <b>DrawOptimized</b> requires more time and memory
        /// than <see cref="Draw"/> due to the additional vertex comparisons. Second, most polygons
        /// are not represented by closed figures within the specified <paramref name="context"/>,
        /// and so the area covered by the specified <paramref name="grid"/> is never considered
        /// filled for hit-testing, rendering, and clipping.</para></remarks>

        public static void DrawOptimized(this PolygonGrid grid,
            StreamGeometryContext context, PointD offset) {

            HashSet<LineI> lines = new HashSet<LineI>();
            double epsilon = grid.Element.Length / 4.0;
            PointD[] vertices = grid.Element.Vertices;

            int width = grid.Size.Width, height = grid.Size.Height;
            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++) {
                    PointD element = grid.GridToDisplay(x, y) + offset;

                    // get & remember first element vertex
                    PointD start = vertices[0] + element;
                    PointD elementStart = start;
                    bool lineSkipped = true;

                    for (int i = 1; i <= vertices.Length; i++) {
                        // close figure by returning to first vertex
                        PointD next = (i == vertices.Length ? elementStart : vertices[i] + element);

                        // reduce line to unique hash coordinates
                        LineI line = new LineI(
                            (int) (start.X / epsilon), (int) (start.Y / epsilon),
                            (int) (next.X / epsilon), (int) (next.Y / epsilon));

                        // skip lines that were drawn (in either direction)
                        if (lines.Contains(line) ||
                            lines.Contains(new LineI(line.End, line.Start))) {
                            lineSkipped = true;
                        } else {
                            // restart figure after skipping lines
                            if (lineSkipped) {
                                context.BeginFigure(start.ToWpfPoint(), false, false);
                                lineSkipped = false;
                            }

                            // draw & remember current line
                            context.LineTo(next.ToWpfPoint(), true, true);
                            lines.Add(line);
                        }

                        // prepare for next vertex
                        start = next;
                    }
                }
        }

        #endregion
        #region ToFigure(RegularPolygon)

        /// <overloads>
        /// Converts the specified <see cref="RegularPolygon"/> to a closed <see
        /// cref="PathFigure"/>.</overloads>
        /// <summary>
        /// Converts the specified <see cref="RegularPolygon"/> to a closed <see
        /// cref="PathFigure"/>.</summary>
        /// <param name="polygon">
        /// The <see cref="RegularPolygon"/> to convert.</param>
        /// <returns>
        /// A closed and frozen <see cref="PathFigure"/> containing all <see
        /// cref="RegularPolygon.Vertices"/> of the specified <paramref name="polygon"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="polygon"/> is a null reference.</exception>

        public static PathFigure ToFigure(this RegularPolygon polygon) {

            PointD[] vertices = polygon.Vertices;
            PathFigure figure = new PathFigure();

            figure.StartPoint = vertices[0].ToWpfPoint();
            for (int i = 1; i < vertices.Length; i++)
                figure.Segments.Add(new LineSegment(vertices[i].ToWpfPoint(), true));

            figure.IsClosed = true;
            figure.Freeze();
            return figure;
        }

        #endregion
        #region ToFigure(RegularPolygon, PointD)

        /// <summary>
        /// Converts the specified <see cref="RegularPolygon"/> to a closed <see
        /// cref="PathFigure"/>, shifted by a specified offset.</summary>
        /// <param name="polygon">
        /// The <see cref="RegularPolygon"/> to convert.</param>
        /// <param name="offset">
        /// The offset by which to shift the <see cref="PathFigure"/>.</param>
        /// <returns>
        /// A closed and frozen <see cref="PathFigure"/> containing all <see
        /// cref="RegularPolygon.Vertices"/> of the specified <paramref name="polygon"/>, shifted by
        /// the specified <paramref name="offset"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="polygon"/> is a null reference.</exception>

        public static PathFigure ToFigure(this RegularPolygon polygon, PointD offset) {

            PointD[] vertices = polygon.Vertices;
            PathFigure figure = new PathFigure();

            figure.StartPoint = (vertices[0] + offset).ToWpfPoint();
            for (int i = 1; i < vertices.Length; i++) {
                PointD next = vertices[i] + offset;
                figure.Segments.Add(new LineSegment(next.ToWpfPoint(), true));
            }

            figure.IsClosed = true;
            figure.Freeze();
            return figure;
        }

        #endregion
        #region ToFigures

        /// <summary>
        /// Converts the specified <see cref="PolygonGrid"/> to a <see
        /// cref="PathFigureCollection"/>.</summary>
        /// <param name="grid">
        /// The <see cref="PolygonGrid"/> to convert.</param>
        /// <returns>
        /// A frozen <see cref="PathFigureCollection"/> containing one <see cref="PathFigure"/> for
        /// each polygonal <see cref="PolygonGrid.Element"/> in the specified <paramref
        /// name="grid"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="grid"/> is a null reference.</exception>
        /// <remarks>
        /// <b>ToFigures</b> returns the results of <see cref="ToFigure"/> for each <see
        /// cref="PolygonGrid.Element"/> in the specified <paramref name="grid"/>, shifted by the
        /// corresponding <see cref="PolygonGrid.GridToDisplay"/> result. Each polygonal <see
        /// cref="PolygonGrid.Element"/> is therefore represented by one <see cref="PathFigure"/>.
        /// </remarks>

        public static PathFigureCollection ToFigures(this PolygonGrid grid) {

            int width = grid.Size.Width, height = grid.Size.Height;
            var figures = new PathFigureCollection(width * height);

            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++) {
                    PointD offset = grid.GridToDisplay(x, y);
                    PathFigure figure = grid.Element.ToFigure(offset);
                    figures.Add(figure);
                }

            figures.Freeze();
            return figures;
        }

        #endregion
    }
}
