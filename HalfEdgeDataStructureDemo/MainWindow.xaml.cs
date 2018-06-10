using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
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
        private List<Visual3D> _sceneElements;
        private int _numDefaultSceneElements;

        public MainWindow()
        {
            InitializeComponent();

            _numDefaultSceneElements = ViewPort.Children.Count;

            ///Default Scene loading
            LoadDemoSceneMenuItem_Click(this, new RoutedEventArgs());
        }

        private void LoadDemoSceneMenuItem_Click(object sender, RoutedEventArgs e)
        {
            _sceneElements = new List<Visual3D>();
            SceneItems.Items.Clear();
            while(ViewPort.Children.Count > _numDefaultSceneElements)
                ViewPort.Children.RemoveAt(_numDefaultSceneElements);

            var start = DateTime.Now;

            ///Test the HalfEdge Manipulations
            /*var triMesh = HalfEdgeMeshGenerator.GenerateHalfEdgeTests();
            _sceneElements.Add(triMesh.CreateMeshVisual3D(default(Material), default(Material)));
            _sceneElements.Add(triMesh.CreateBoundaryVisual3D(default(Color)));*/

            ///Demo Scene
            var triMesh = HalfEdgeMeshGenerator.GenerateCube(HalfEdgeDataStructure.Vector.Zero(), 2, CubeSides.All);
            var bigCube = triMesh.CreateVisual3D(default(Material), default(Material));
            _sceneElements.Add(triMesh.CreateVisual3D(default(Material), default(Material)));
            _sceneElements.Add(triMesh.CreateBoundaryVisual3D(default(Color)));

            triMesh = HalfEdgeMeshGenerator.GenerateCube(new HalfEdgeDataStructure.Vector(1, 1, 2.0005), .499, CubeSides.Z | CubeSides.X);
            var cubeVisual3D = triMesh.CreateVisual3D(default(Material), default(Material));
            var cubeBoundaryVisual3D = triMesh.CreateBoundaryVisual3D(default(Color));
            cubeVisual3D.Transform = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(0, 0, 1), 15));
            cubeBoundaryVisual3D.Transform = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(0, 0, 1), 15));
            _sceneElements.Add(cubeVisual3D);
            _sceneElements.Add(cubeBoundaryVisual3D);

            triMesh = HalfEdgeMeshGenerator.GenerateCube(new HalfEdgeDataStructure.Vector(.5, .5, 2.5), 1, CubeSides.All & ~CubeSides.PositiveZ, true);
            _sceneElements.Add(triMesh.CreateVisual3D(new DiffuseMaterial(new SolidColorBrush(Colors.OrangeRed)), default(Material)));
            _sceneElements.Add(triMesh.CreateBoundaryVisual3D(Colors.LightGreen));

            MeshBuilder mb = new MeshBuilder();
            mb.AddSphere(new Point3D(3, -1, 0.5), 0.5);
            var mg = mb.ToMesh();
            triMesh = HalfEdgeMeshGenerator.GenerateFromMeshGeometry3D(mg, true);
            _sceneElements.Add(triMesh.CreateVisual3D(default(Material), default(Material)));
            _sceneElements.Add(triMesh.CreateBoundaryVisual3D(default(Color)));

            var sphere = HalfEdgeMeshGenerator.GenerateSphere(new HalfEdgeDataStructure.Vector(-2, 1, 1), 1, 4,
                CubeSides.All & ~CubeSides.PositiveX & ~CubeSides.NegativeY & ~CubeSides.PositiveZ);
            _sceneElements.Add(sphere.CreateVisual3D(default(Material), default(Material)));
            _sceneElements.Add(sphere.CreateBoundaryVisual3D(default(Color)));

            var material = MaterialHelper.CreateMaterial(new SolidColorBrush(Colors.LightSkyBlue), 1, 128, 255, false);
            MaterialHelper.ChangeOpacity(material, 0.5);

            var sphere2 = HalfEdgeMeshGenerator.GenerateSphere(new HalfEdgeDataStructure.Vector(-1.975, 0.975, 1.025), 1, 32,
                CubeSides.All & ~CubeSides.NegativeX & ~CubeSides.PositiveY & ~CubeSides.NegativeZ);
            _sceneElements.Add(sphere2.CreateVisual3D(material, material));
            _sceneElements.Add(sphere2.CreateBoundaryVisual3D(default(Color)));

            material = MaterialHelper.CreateMaterial(new SolidColorBrush(Colors.LightGoldenrodYellow), 1, 128, 255, false);
            MaterialHelper.ChangeOpacity(material, 0.5);
            var sphere3 = HalfEdgeMeshGenerator.GenerateSphere(new HalfEdgeDataStructure.Vector(2, -0.5, 0.5), 0.5, 16);
            _sceneElements.Add(sphere3.CreateVisual3D(material, material));

            var timeNeeded = DateTime.Now - start;
            Title = timeNeeded.TotalMilliseconds.ToString();
            ///CloseMenuItem_Click(this, new RoutedEventArgs());

            AddSceneElements();
        }

        /// <summary>
        /// Add all SceneElements to the ViewPort and to the Elements List.
        /// </summary>
        private void AddSceneElements()
        {
            foreach(var sceneElement in _sceneElements)
            {
                ViewPort.Children.Add(sceneElement);
                var checkBox = new CheckBox()
                {
                    IsChecked = true,
                    Content = sceneElement.GetType(),
                    DataContext = sceneElement,
                };
                checkBox.Click += CheckBox_Click;

                SceneItems.Items.Add(checkBox);
            }
        }

        /// <summary>
        /// Close the Application.
        /// </summary>
        /// <param name="sender">The Sender.</param>
        /// <param name="e">The RoutedEventArgs.</param>
        private void CloseMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Remove or add the SceneElement from / to the Scene.
        /// </summary>
        /// <param name="sender">The Sender.</param>
        /// <param name="e">The RoutedEventArgs.</param>
        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            var checkBox = sender as CheckBox;
            var sceneElement = checkBox.DataContext as Visual3D;
            var idx = _sceneElements.IndexOf(sceneElement);
            if(idx == -1)
                return;

            if (checkBox.IsChecked == false)
                ViewPort.Children.Remove(sceneElement);
            if(checkBox.IsChecked == true)
                ViewPort.Children.Add(sceneElement);
        }
    }
}