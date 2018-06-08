using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using HelixToolkit.Wpf;
using Helper;

namespace HalfEdgeDataStructureDemo
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            var triMesh = HalfEdgeMeshGenerator.GenerateCube(HalfEdgeDataStructure.Vector.Zero(), 2, CubeSides.All);
            ViewPort.Children.Add(triMesh.CreateMeshVisual3D(default(Color), default(Color)));
            ViewPort.Children.Add(triMesh.CreateBoundaryVisual3D(default(Color)));

            triMesh = HalfEdgeMeshGenerator.GenerateCube(new HalfEdgeDataStructure.Vector(1, 1, 2.0005), .499, CubeSides.Z | CubeSides.X);
            var cubeVisual3D = triMesh.CreateMeshVisual3D(default(Color), default(Color));
            var cubeBoundaryVisual3D = triMesh.CreateBoundaryVisual3D(default(Color));
            cubeVisual3D.Transform = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(0, 0, 1), 15));
            cubeBoundaryVisual3D.Transform = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(0, 0, 1), 15));
            ViewPort.Children.Add(cubeVisual3D);
            ViewPort.Children.Add(cubeBoundaryVisual3D);

            triMesh = HalfEdgeMeshGenerator.GenerateCube(new HalfEdgeDataStructure.Vector(.5, .5, 2.5), 1, CubeSides.All & ~CubeSides.PositiveZ);
            ViewPort.Children.Add(triMesh.CreateMeshVisual3D(Colors.OrangeRed, default(Color)));
            ViewPort.Children.Add(triMesh.CreateBoundaryVisual3D(Colors.LightGreen));


            MeshBuilder mb = new MeshBuilder();
            mb.AddSphere(new Point3D(3, -1, .5), .5);
            var mg = mb.ToMesh();
            triMesh = HalfEdgeMeshGenerator.GenerateFromMeshGeometry3D(mg);
            ViewPort.Children.Add(triMesh.CreateMeshVisual3D(default(Color), default(Color)));
            ViewPort.Children.Add(triMesh.CreateBoundaryVisual3D(default(Color)));
        }
    }
}
