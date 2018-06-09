using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using HalfEdgeDataStructure;

namespace Helper
{
    /// <summary>
    /// Extension Methods that help with existing Classes.
    /// </summary>
    public static class ExtensionMethods
    {
        private static readonly double EdgeDiameter = 0.002;
        private static readonly double VertexRadius = EdgeDiameter * 0.75;
        private static readonly double Offset = 0.0001;
        private static readonly Color DefaultForegroundColor = Colors.White;
        private static readonly Color DefaultBackgroundColor = Colors.LightYellow;
        private static readonly Color DefaultHighlightColor = Colors.Red;

        /// <summary>
        /// Create a MeshVisual3D from an existing HalfEdgeMesh with specified Colors.
        /// </summary>
        /// <param name="mesh">The existing HalfEdgeMesh.</param>
        /// <param name="foreground">The Color for the Triangles of the Mesh.</param>
        /// <param name="backGround">The Background Color for the Triangles of the Mesh.</param>
        /// <returns></returns>
        public static HelixToolkit.Wpf.MeshVisual3D CreateMeshVisual3D(this HalfEdgeMesh mesh, Color foreground, Color backGround)
        {
            if(foreground == default(Color))
                foreground = DefaultForegroundColor;
            if(backGround == default(Color))
                backGround = DefaultBackgroundColor;

            var frontMaterial = new DiffuseMaterial()
            {
                Brush = new SolidColorBrush(foreground)
            };
            var backMaterial = new DiffuseMaterial()
            {
                Brush = new SolidColorBrush(backGround)
            };

            HelixToolkit.Wpf.Mesh3D mesh3D = new HelixToolkit.Wpf.Mesh3D(mesh.Vertices.Select(p => new Point3D(p.X, p.Y, p.Z)), mesh.Triangles.SelectMany(t => t.VertexIndizes));
            HelixToolkit.Wpf.MeshVisual3D meshVisual = new HelixToolkit.Wpf.MeshVisual3D()
            {
                Mesh = mesh3D,
                FaceMaterial = frontMaterial,
                FaceBackMaterial = backMaterial,
                EdgeDiameter = 0,
                VertexRadius = 0
            };

            return meshVisual;
        }

        /// <summary>
        /// Create a LinesVisual3D from the Border Information of an existing HalfEdgeMesh.
        /// </summary>
        /// <param name="mesh">The existing HalfEdgeMesh.</param>
        /// <param name="lineColor">The Line Color.</param>
        /// <param name="thickness">The Line Thickness of the Border Lines.</param>
        /// <returns></returns>
        public static HelixToolkit.Wpf.LinesVisual3D CreateBoundaryVisual3D(this HalfEdgeMesh mesh, Color lineColor, double thickness = 2)
        {
            if(lineColor == default(Color))
                lineColor = DefaultHighlightColor;

            var borderPoints = new Point3DCollection(mesh.BoundaryPoints.Select(p => new Point3D(p.X, p.Y, p.Z)));
            HelixToolkit.Wpf.LinesVisual3D lineVisual = new HelixToolkit.Wpf.LinesVisual3D
            {
                Points = borderPoints,
                Color = lineColor,
                Thickness = thickness,
                DepthOffset = Offset
            };

            return lineVisual;
        }
    }
}