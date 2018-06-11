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
        /// <returns></returns>
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
        /// <returns></returns>
        private static MeshVisual3D CreateVisual3DWithoutNormals(this HalfEdgeMesh mesh,
            Material frontMaterial, Material backMaterial)
        {
            if(frontMaterial == default(Material))
                frontMaterial = new DiffuseMaterial(new SolidColorBrush(DefaultForegroundColor));
            if(backMaterial == default(Material))
                backMaterial = new DiffuseMaterial(new SolidColorBrush(DefaultBackgroundColor));

            var mesh3D = new Mesh3D(mesh.Vertices.Select(p => new Point3D(p.X, p.Y, p.Z)), mesh.Triangles.SelectMany(t => t.VertexIndizes));

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
        /// <returns></returns>
        public static LinesVisual3D CreateBoundaryVisual3D(this HalfEdgeMesh mesh, Color lineColor, double thickness = 2)
        {
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

        public static Ray3D GetRay3D(this PerspectiveCamera cam, double xPosFromCenter, double yPosFromCenter)
        {
            var ray = new Ray3D();
            ray.Origin = cam.Position;
            var left = Vector3D.CrossProduct(cam.LookDirection, cam.UpDirection);
            var up  = Vector3D.CrossProduct(cam.LookDirection, left);
            var rayDirection = cam.LookDirection + left + up;
            rayDirection.Normalize();
            ray.Direction = rayDirection;

            return ray;
        }
    }
}