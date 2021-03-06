﻿using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using HalfEdgeDataStructureDemo.ViewModels;
using HelixToolkit.Wpf;
using Helper;

namespace HalfEdgeDataStructureDemo
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// The ViewModel of the MainWindow.
        /// </summary>
        public MainWindowViewModel ViewModel
        {
            get { return (MainWindowViewModel)DataContext; }
        }

        /// <summary>
        /// Default Constructor of the MainWindow.
        /// </summary>
        public MainWindow()
        {
            DataContext = new MainWindowViewModel();
            InitializeComponent();

            ///Default Scene loading
            LoadDemoSceneMenuItem_Click(this, new RoutedEventArgs());
        }

        private void LoadDemoSceneMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.AddedSceneElements = new ObservableCollection<Visual3DViewModel>();
            ViewPort.Children.Clear();

            ///Test the HalfEdge Manipulations
            /*var triMesh = HalfEdgeMeshGenerator.GenerateHalfEdgeTests();
            triMesh.RemoveTriangle(triMesh.Triangles[0]);
            triMesh.RemoveTriangle(triMesh.Triangles[4]);
            triMesh.RemoveTriangle(triMesh.Triangles[3]);
            triMesh.RemoveTriangle(triMesh.Triangles[1]);
            triMesh.RemoveTriangle(triMesh.Triangles[2]);
            triMesh.RemoveTriangle(triMesh.Triangles[7]);
            ViewModel.AddedSceneElements.Add(new Visual3DViewModel(triMesh.CreateVisual3D(default(Material), default(Material)), "TestVisual"));
            ViewModel.AddedSceneElements.Add(new Visual3DViewModel(triMesh.CreateBoundaryVisual3D(default(Color)), "TestVisual Boundary"));
            ViewPort.DebugInfo = string.Join(" -> ", triMesh.BoundaryVertices.Where((v, i) => i == 0 || i % 2 == 1).Select(v => v.ToString()));*/

            ///Demo Scene
            var triMesh = HalfEdgeMeshGenerator.GenerateCube(HalfEdgeDataStructure.Vector.Zero, 2, CubeSides.All);
            var bigCube = triMesh.CreateVisual3D(default(Material), default(Material));
            ViewModel.AddedSceneElements.Add(new Visual3DViewModel(triMesh, triMesh.CreateVisual3D(default(Material), default(Material)), "Big Cube"));

            triMesh = HalfEdgeMeshGenerator.GenerateCube(new HalfEdgeDataStructure.Vector(1, 1, 2.0005f), .499f, CubeSides.Z | CubeSides.X);
            var cubeVisual3D = triMesh.CreateVisual3D(default(Material), default(Material));
            var cubeBoundaryVisual3D = triMesh.CreateBoundaryVisual3D(default(Color));
            cubeVisual3D.Transform = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(0, 0, 1), 15));
            cubeBoundaryVisual3D.Transform = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(0, 0, 1), 15));
            ViewModel.AddedSceneElements.Add(new Visual3DViewModel(triMesh, cubeVisual3D, "Small Open Cube"));
            ViewModel.AddedSceneElements.Add(new Visual3DViewModel(triMesh, cubeBoundaryVisual3D, "Small Open Cube Border"));

            triMesh = HalfEdgeMeshGenerator.GenerateCube(new HalfEdgeDataStructure.Vector(0.5f, 0.5f, 2.5f), 1, CubeSides.All & ~CubeSides.PositiveZ, true);
            ViewModel.AddedSceneElements.Add(new Visual3DViewModel(triMesh, triMesh.CreateVisual3D(new DiffuseMaterial(new SolidColorBrush(Colors.OrangeRed)), default(Material)), "Orange Open Cube"));
            ViewModel.AddedSceneElements.Add(new Visual3DViewModel(triMesh, triMesh.CreateBoundaryVisual3D(Colors.LightGreen), "Orange Open Cube Border"));

            MeshBuilder mb = new MeshBuilder();
            mb.AddSphere(new Point3D(3, -1, 0.5), 0.5);
            var mg = mb.ToMesh();
            triMesh = HalfEdgeMeshGenerator.GenerateFromMeshGeometry3D(mg, true);
            ViewModel.AddedSceneElements.Add(new Visual3DViewModel(triMesh, triMesh.CreateVisual3D(default(Material), default(Material)), "HelixToolkit Sphere"));
            ViewModel.AddedSceneElements.Add(new Visual3DViewModel(triMesh, triMesh.CreateBoundaryVisual3D(default(Color)), "HelixToolkit Sphere Border"));

            var sphere = HalfEdgeMeshGenerator.GenerateSphere(new HalfEdgeDataStructure.Vector(-2, 1, 1), 1, 4,
                CubeSides.All & ~CubeSides.PositiveX & ~CubeSides.NegativeY & ~CubeSides.PositiveZ);
            ViewModel.AddedSceneElements.Add(new Visual3DViewModel(sphere, sphere.CreateVisual3D(default(Material), default(Material)), "Coarse Sphere Part"));
            ViewModel.AddedSceneElements.Add(new Visual3DViewModel(sphere, sphere.CreateBoundaryVisual3D(default(Color)), "Coarse Sphere Part Border"));

            var material = MaterialHelper.CreateMaterial(new SolidColorBrush(Colors.LightSkyBlue), 1, 128, 255, false);
            MaterialHelper.ChangeOpacity(material, 0.5);

            var sphere2 = HalfEdgeMeshGenerator.GenerateSphere(new HalfEdgeDataStructure.Vector(-1.975f, 0.975f, 1.025f), 1, 32,
                CubeSides.All & ~CubeSides.NegativeX & ~CubeSides.PositiveY & ~CubeSides.NegativeZ);
            var numSphere2Triangles = sphere2.Triangles.Count;
            var numSphere2Vertices = sphere2.Vertices.Count;
            var rand = new Random();
            var numRemove = (int)(numSphere2Vertices * 0.2f);
            var removalList = new int[numRemove];
            for(int i = 0; i < numRemove; i++)
                removalList[i] = rand.Next(numSphere2Triangles);
            sphere2.RemoveVertices(removalList);

            ViewModel.AddedSceneElements.Add(new Visual3DViewModel(sphere2, sphere2.CreateVisual3D(material, material), "Fine Transparent Sphere Part"));
            ViewModel.AddedSceneElements.Add(new Visual3DViewModel(sphere2, sphere2.CreateBoundaryVisual3D(default(Color)), "Fine Transparent Sphere Part Border"));

            material = MaterialHelper.CreateMaterial(new SolidColorBrush(Colors.LightGoldenrodYellow), 1, 128, 255, false);
            MaterialHelper.ChangeOpacity(material, 0.5);
            var sphere3 = HalfEdgeMeshGenerator.GenerateSphere(new HalfEdgeDataStructure.Vector(2, -0.5f, 0.5f), 0.5f, 16);
            ViewModel.AddedSceneElements.Add(new Visual3DViewModel(sphere3, sphere3.CreateVisual3D(material, material), "Transparent Sphere"));

            AddSceneElements();

            ///AddSilhoutetteForVisual3D();
        }

        /// <summary>
        /// Add all SceneElements to the ViewPort and to the Elements List.
        /// </summary>
        private void AddSceneElements()
        {
            foreach(var sceneElement in ViewModel.AllElements)
                ViewPort.Children.Add(sceneElement.Visual3D);
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
            var sceneElement = checkBox.DataContext as Visual3DViewModel;

            if (checkBox.IsChecked == false)
                ViewPort.Children.Remove(sceneElement.Visual3D);
            if(checkBox.IsChecked == true)
                ViewPort.Children.Add(sceneElement.Visual3D);
        }

        /// <summary>
        /// Handle Mouse Move when the Mouse is in the 3D View.
        /// </summary>
        /// <param name="sender">The Sender.</param>
        /// <param name="e">The MouseEventArgs.</param>
        private void ViewPort_MouseMove(object sender, MouseEventArgs e)
        {
            if(!ViewModel.ShowHoveredElement)
                return;

            var maxValue = Math.Tan((ViewPort.Camera as PerspectiveCamera).FieldOfView * 0.5 * ExtensionMethods.DegreeToRadians);
            var width = ViewPort.ActualWidth;
            var height = ViewPort.ActualHeight;
            var aspectRatio = height / width;
            var mousePosition = e.GetPosition(ViewPort);
            var xValue = mousePosition.X / width * 2 * maxValue - maxValue;
            var yValue = (height - mousePosition.Y) / height * maxValue - 0.5;
            var ray = Camera.GetRay3D(xValue, yValue);

            var material = MaterialHelper.CreateMaterial(new SolidColorBrush(Colors.Orange), 1, 128, 255, false);
            MaterialHelper.ChangeOpacity(material, 0.5);

            var hitElements = Viewport3DHelper.FindHits(ViewPort.Viewport, mousePosition)
                .Where(el => ViewModel.AddedSceneElements.Select(ae => ae.Visual3D).Contains(el.Visual));
            if(hitElements.Count() > 0)
            {
                var hitElement = hitElements.First().Visual;
                if(hitElement == ViewPort.Children.Last())
                    return;
                else if(ViewModel.HoveredElement != null)
                    RemoveHoveredElement();

                if(hitElement is MeshVisual3D)
                    ViewModel.HoveredElement = new MeshVisual3D()
                    {
                        Mesh = (hitElement as MeshVisual3D).Mesh,
                        FaceMaterial = material,
                        FaceBackMaterial = material,
                        Transform = hitElement.Transform,
                        EdgeDiameter = 0,
                        VertexRadius = 0
                    };
                else if (hitElement is MeshGeometryVisual3D)
                {
                    ViewModel.HoveredElement = new MeshGeometryVisual3D()
                    {
                        MeshGeometry = (hitElement as MeshGeometryVisual3D).MeshGeometry,
                        Material = material,
                        BackMaterial = material,
                        Transform = hitElement.Transform,
                    };
                }

                if(ViewModel.HoveredElement != null)
                    ViewPort.Children.Add(ViewModel.HoveredElement);
            }
            else
                RemoveHoveredElement();

            ViewPort.DebugInfo = $"Origin: {ray.Origin.ToString(3)}, Direction: {ray.Direction.ToString(3)}";
        }

        /// <summary>
        /// Change the Option to show the hovered Element or not.
        /// </summary>
        /// <param name="sender">The Sender.</param>
        /// <param name="e">The RoutedEventArgs.</param>
        private void ShowHoveredSceneElementsMenuItem_Checked(object sender, RoutedEventArgs e)
        {
            RemoveHoveredElement();
        }

        /// <summary>
        /// Remove the hovered Element from the ViewPort and the ViewModel.
        /// </summary>
        private void RemoveHoveredElement()
        {
            ViewPort.Children.Remove(ViewModel.HoveredElement);
            ViewModel.HoveredElement = null;
        }

        /// <summary>
        /// Handle the Change of the Camera.
        /// </summary>
        /// <param name="sender">The Sender.</param>
        /// <param name="e">The RoutedEventArgs.</param>
        private void ViewPort_CameraChanged(object sender, RoutedEventArgs e)
        {
            /*var silhouette = ViewModel.AddedSceneElements.FirstOrDefault(svm => svm.Name == "Silhouette Edges");
            if(silhouette != default(Visual3DViewModel))
            {
                ViewModel.AddedSceneElements.Remove(silhouette);
                ViewPort.Children.Remove(silhouette.Visual3D);
            }

            AddSilhoutetteForVisual3D();*/
        }

        /// <summary>
        /// Add a Silhouette for a specific Visual3D Object.
        /// </summary>
        /// <param name="objectName">The Name of the Visual3D to create and add a Silhouette.</param>
        private void AddSilhoutetteForVisual3D(string objectName = "HelixToolkit Sphere")
        {
            /*var triMesh = ViewModel.AddedSceneElements.First(svm => svm.Name == objectName).Mesh;
            var triMeshSilhouette = triMesh.CalculateSilhouette(new HalfEdgeDataStructure.Vector(Camera.Position.ToVector())).SelectMany(l => l).ToList();
            var silhouetteEdges = ExtensionMethods.CreateHalfEdgeVisual(triMeshSilhouette, Colors.Red);
            var newVisual3DVM = new Visual3DViewModel(triMesh, silhouetteEdges, "Silhouette Edges");
            ViewModel.AddedSceneElements.Add(newVisual3DVM);
            ViewPort.Children.Add(newVisual3DVM.Visual3D);*/
        }
    }
}