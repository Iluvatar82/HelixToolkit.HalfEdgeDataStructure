using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using HalfEdgeDataStructure;
using HelixToolkit.Wpf;

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
        private static readonly Color DefaultHighlightColor = Colors.Orange;
        public static readonly double DegreeToRadians = Math.PI / 180;
        public static readonly double RadiansToDegree = 1.0 / DegreeToRadians;

        /// <summary>
        /// Depending of the HalfEdgeMesh's <see cref="HalfEdgeMesh.HalfEdges"/> Property, a MeshGeometryVisual3D or a MeshVisual3D is generated.
        /// </summary>
        /// <param name="mesh">The HalfEdgeMesh.</param>
        /// <param name="frontMaterial">The front Material.</param>
        /// <param name="backMaterial">The back Material.</param>
        /// <returns></returns>
        public static ModelVisual3D CreateVisual3D(this HalfEdgeMesh mesh,
            Material frontMaterial, Material backMaterial)
        {
            if(mesh.HasNormals)
                return CreateVisual3DWithNormals(mesh, frontMaterial, backMaterial);
            else
                return CreateVisual3DWithoutNormals(mesh, frontMaterial, backMaterial);
        }

        /// <summary>
        /// Create a MeshGeometryVisual3D from an existing HalfEdgeMesh with specified Colors.
        /// </summary>
        /// <param name="mesh">The existing HalfEdgeMesh.</param>
        /// <param name="foreground">The Color for the Triangles of the Mesh.</param>
        /// <param name="backGround">The Background Color for the Triangles of the Mesh.</param>
        /// <returns>MeshGeometryVisual3D of the HalfEdgeMesh with the specified Materials.</returns>
        private static MeshGeometryVisual3D CreateVisual3DWithNormals(this HalfEdgeMesh mesh,
            Material frontMaterial, Material backMaterial)
        {
            if(frontMaterial == default(Material))
                frontMaterial = new DiffuseMaterial(new SolidColorBrush(DefaultForegroundColor));
            if(backMaterial == default(Material))
                backMaterial = new DiffuseMaterial(new SolidColorBrush(DefaultBackgroundColor));

            var mesh3D = new MeshGeometry3D();
            mesh3D.Positions = new Point3DCollection(mesh.Vertices.Select(p => new Point3D(p.X, p.Y, p.Z)));
            mesh3D.TriangleIndices = new Int32Collection(mesh.Triangles.SelectMany(t => t.VertexIndizes));
            if (mesh.HasNormals)
                mesh3D.Normals = new Vector3DCollection(mesh.Vertices.Select(v => new Vector3D(v.Normal.X, v.Normal.Y, v.Normal.Z)));

            var meshVisual = new MeshGeometryVisual3D()
            {
                MeshGeometry = mesh3D,
                Material = frontMaterial,
                BackMaterial = backMaterial
            };

            return meshVisual;
        }

        /// <summary>
        /// Create a MeshGeometryVisual3D from an existing HalfEdgeMesh with specified Colors.
        /// </summary>
        /// <param name="mesh">The existing HalfEdgeMesh.</param>
        /// <param name="foreground">The Color for the Triangles of the Mesh.</param>
        /// <param name="backGround">The Background Color for the Triangles of the Mesh.</param>
        /// <returns>MeshVisual3D of the HalfEdgeMesh with the specified Materials.</returns>
        private static MeshVisual3D CreateVisual3DWithoutNormals(this HalfEdgeMesh mesh,
            Material frontMaterial, Material backMaterial)
        {
            if(frontMaterial == default(Material))
                frontMaterial = new DiffuseMaterial(new SolidColorBrush(DefaultForegroundColor));
            if(backMaterial == default(Material))
                backMaterial = new DiffuseMaterial(new SolidColorBrush(DefaultBackgroundColor));

            var mesh3D = new Mesh3D(mesh.Vertices.Select(p => new Point3D(p.X, p.Y, p.Z)), mesh.Triangles.Where(t => t.HalfEdge != null).SelectMany(t => t.VertexIndizes));

            var meshVisual = new MeshVisual3D()
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
        /// <returns>LinesVisual3D representing the Boundary of the HalfEdgeMesh if the <paramref name="lineColor"/> is not closed.</returns>
        public static LinesVisual3D CreateBoundaryVisual3D(this HalfEdgeMesh mesh, Color lineColor, double thickness = 2)
        {
            if(mesh.IsClosed)
                return null;

            if(lineColor == default(Color))
                lineColor = DefaultHighlightColor;

            var borderVertices = new Point3DCollection(mesh.BoundaryVertices.Select(p => new Point3D(p.X, p.Y, p.Z)));
            var lineVisual = new LinesVisual3D
            {
                Points = borderVertices,
                Color = lineColor,
                Thickness = thickness,
                DepthOffset = Offset
            };

            return lineVisual;
        }



        public static LinesVisual3D CreateHalfEdgeVisual(List<HalfEdge> halfEdges, Color lineColor, double thickness = 2)
        {
            var borderVertices = new Point3DCollection(halfEdges.SelectMany(e => new List<Vertex> { e.StartVertex, e.EndVertex })
                .Select(v => new Point3D(v.X, v.Y, v.Z)));
            var lineVisual = new LinesVisual3D
            {
                Points = borderVertices,
                Color = lineColor,
                Thickness = thickness,
                DepthOffset = Offset
            };

            return lineVisual;
        }




        /// <summary>
        /// Create a Ray3D from the Perspective Camera and the X- and Y Values of the MousePosition.
        /// </summary>
        /// <param name="camera">The PerspectiveCamera.</param>
        /// <param name="xPosFromCenter">The X Position of the Mouse in the Range [-maxXValue, maxXValue].</param>
        /// <param name="yPosFromCenter">The Y Position of the Mouse in the Range [-maxYValue, maxYValue].</param>
        /// <returns>Ray3D originating in the Camera's Position, pointing to the MousePosition in 3D Space.</returns>
        public static Ray3D GetRay3D(this PerspectiveCamera camera, double xPosFromCenter, double yPosFromCenter)
        {
            var ray = new Ray3D();
            ray.Origin = camera.Position;
            var forward = camera.LookDirection;
            forward.Normalize();
            var up = camera.UpDirection;
            up.Normalize();
            var left = Vector3D.CrossProduct(up, forward);
            left.Normalize();
            up  = Vector3D.CrossProduct(camera.LookDirection, left);
            up.Normalize();
            var rayDirection = forward + -left * xPosFromCenter + up * yPosFromCenter;
            rayDirection.Normalize();
            ray.Direction = rayDirection;

            return ray;
        }

        /// <summary>
        /// Convert the Point3D to a HalfEdgeDataStructure Vector.
        /// </summary>
        /// <param name="point">The Point3D to convert.</param>
        /// <returns>The generated Vector.</returns>
        public static Vector ToVector(this Point3D point)
        {
            return new Vector((float)point.X, (float)point.Y, (float)point.Z);
        }

        /// <summary>
        /// Add custom ToString for Point3D Objects.
        /// </summary>
        /// <param name="point">The Point3D to use.</param>
        /// <param name="digits">Number of Digits to use after the Comma.</param>
        /// <returns>String Representatin of the Point3D.</returns>
        public static string ToString(this Point3D point, uint digits)
        {
            var format = "#.".PadRight((int)digits + 2, '0');
            return $"X: {point.X.ToString(format)}, Y: {point.Y.ToString(format)}, Z: {point.Z.ToString(format)}";
        }

        /// <summary>
        /// Add custom ToString for Vector3D Objects.
        /// </summary>
        /// <param name="vector">The Vector3D to use.</param>
        /// <param name="digits">Number of Digits to use after the Comma.</param>
        /// <returns>String Representatin of the Vector3D.</returns>
        public static string ToString(this Vector3D vector, uint digits)
        {
            return vector.ToPoint3D().ToString(digits);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="viewport"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static List<Visual3D> FindHits(this HelixViewport3D viewport, double x, double y)
        {
            ///TODO implement
            var ray = GetRay3D(viewport.Camera as PerspectiveCamera, x, y);
            var results = new List<Visual3D>();

            foreach(var element in viewport.Children)
            {

            }

            return results;
        }
    }
}