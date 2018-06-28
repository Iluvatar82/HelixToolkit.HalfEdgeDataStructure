using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using HalfEdgeDataStructure;
using Helper;

namespace HalfEdgeDataStructureDemo.ViewModels
{
    public class Visual3DViewModel : BaseViewModel
    {
        private HalfEdgeMesh _mesh;
        private Visual3D _visual3D;
        private string _name;


        public HalfEdgeMesh Mesh
        {
            get { return _mesh; }
            set { _mesh = value; }
        }

        public Visual3D Visual3D
        {
            get { return _visual3D; }
            set {
                SetValue(ref _visual3D, value);
            }
        }

        public string Name
        {
            get { return _name; }
            set {
                SetValue(ref _name, value);
            }
        }

        public Visual3DViewModel(HalfEdgeMesh mesh, Visual3D visual3D, string name)
        {
            _mesh = mesh;
            _visual3D = visual3D;
            _name = name;
        }
    }
}