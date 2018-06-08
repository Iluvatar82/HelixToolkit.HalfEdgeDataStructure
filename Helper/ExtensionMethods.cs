using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using HalfEdgeDataStructure;

namespace Helper
{
    public static class ExtensionMethods
    {
        private static readonly double EdgeDiameter = 0.002;
        private static readonly double VertexRadius = EdgeDiameter * 0.5;
        private static readonly double Offset = 0.0001;
        private static readonly Color DefaultForegroundColor = Colors.White;
        private static readonly Color DefaultBackgroundColor = Colors.LightYellow;
        private static readonly Color DefaultHighlightColor = Colors.Red;

        /// </summary>
        /// <param name="heMesh"></param>
        /// <param name="foreground"></param>
        /// <param name="backGround"></param>
        /// <returns></returns>
        public static HelixToolkit.Wpf.MeshVisual3D CreateMeshVisual3D(this HalfEdgeMesh heMesh, Color foreground, Color backGround)
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

            HelixToolkit.Wpf.Mesh3D mesh = new HelixToolkit.Wpf.Mesh3D(heMesh.Vertices.Select(p => new Point3D(p.X, p.Y, p.Z)), heMesh.Triangles.SelectMany(t => t.PointIndizes));
            HelixToolkit.Wpf.MeshVisual3D meshVisual = new HelixToolkit.Wpf.MeshVisual3D()
            {
                Mesh = mesh,
                FaceMaterial = frontMaterial,
                FaceBackMaterial = backMaterial,
                EdgeDiameter = EdgeDiameter,
                VertexRadius = VertexRadius
            };

            return meshVisual;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="heMesh"></param>
        /// <param name="lineColor"></param>
        /// <param name="thickness"></param>
        /// <returns></returns>
        public static HelixToolkit.Wpf.LinesVisual3D CreateBoundaryVisual3D(this HalfEdgeMesh heMesh, Color lineColor, double thickness = 2)
        {
            if(lineColor == default(Color))
                lineColor = DefaultHighlightColor;

            var borderPoints = new Point3DCollection(heMesh.BoundaryPoints.Select(p => new Point3D(p.X, p.Y, p.Z)));
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