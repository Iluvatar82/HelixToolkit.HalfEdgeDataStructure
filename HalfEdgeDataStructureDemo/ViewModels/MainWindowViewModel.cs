using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using HelixToolkit.Wpf;
using Helper;

namespace HalfEdgeDataStructureDemo.ViewModels
{
    public class MainWindowViewModel : BaseViewModel
    {
        private ObservableCollection<Visual3DViewModel> _startElements;
        private ObservableCollection<Visual3DViewModel> _addedSceneElements;
        private List<Visual3D> _selectedElements;
        private Visual3D _hoveredElement;


        public ObservableCollection<Visual3DViewModel> StartElements
        {
            get { return _startElements; }
            set {
                SetValue(ref _startElements, value);
            }
        }

        public ObservableCollection<Visual3DViewModel> AddedSceneElements
        {
            get { return _addedSceneElements; }
            set {
                SetValue(ref _addedSceneElements, value);
            }
        }

        public IEnumerable<Visual3DViewModel> AllElements
        {
            get { return _startElements.Concat(_addedSceneElements); }
        }

        public List<Visual3D> SelectedElements
        {
            get { return _selectedElements; }
            set { _selectedElements = value; }
        }

        public Visual3D HoveredElement
        {
            get { return _hoveredElement; }
            set { _hoveredElement = value; }
        }


        public MainWindowViewModel()
        {
            _startElements = new ObservableCollection<Visual3DViewModel>();
            _addedSceneElements = new ObservableCollection<Visual3DViewModel>();
            _selectedElements = new List<Visual3D>();
            _hoveredElement = default(Visual3D);

            SetupStartElements();
        }


        private void SetupStartElements()
        {
            _startElements.Add(new Visual3DViewModel(new SunLight(), "Sunlight"));
            var floor = new QuadVisual3D()
            {
                Fill = new SolidColorBrush(Colors.LightGray),
                Point1 = new Point3D(-10, -10, -0.001),
                Point2 = new Point3D(10, -10, -0.001),
                Point3 = new Point3D(10, 10, -0.001),
                Point4 = new Point3D(-10, 10, -0.001)
            };
            _startElements.Add(new Visual3DViewModel(floor, "Floor"));

            var gridLines = new GridLinesVisual3D()
            {
                MinorDistance = 2,
                Thickness = 0.02,
            };
            _startElements.Add(new Visual3DViewModel(gridLines, "Grid Lines"));

            AddAxisInfo(new Vector3D(5, 0, 0), new SolidColorBrush(Colors.Green), new Point3D(5.1, 0, 0), "X");
            AddAxisInfo(new Vector3D(0, 5, 0), new SolidColorBrush(Colors.Blue), new Point3D(0, 5.1, 0), "Y");
            AddAxisInfo(new Vector3D(0, 0, 5), new SolidColorBrush(Colors.Red), new Point3D(0, 0, 5.1), "Z");
        }

        private void AddAxisInfo(Vector3D direction, Brush brush, Point3D position, string text)
        {
            var asix = new ArrowVisual3D()
            {
                Direction = direction,
                HeadLength = 2,
                Diameter = 0.05,
                Fill = brush
            };
            _startElements.Add(new Visual3DViewModel(asix, $"{text} - Axis"));

            var axisLabel = new BillboardTextVisual3D()
            {
                Position = position,
                Text = text,
                Foreground = brush,
                FontSize = 18
            };
            _startElements.Add(new Visual3DViewModel(axisLabel, text));
        }
    }
}