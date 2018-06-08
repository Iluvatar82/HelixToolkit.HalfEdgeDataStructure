namespace HalfEdgeDataStructure
{
    public abstract class AbstractMeshElement
    {
        private HalfEdgeMesh _triangleMesh;
        private int _index;

        public HalfEdgeMesh TriangleMesh {
            get { return _triangleMesh; }
            set { _triangleMesh = value; }
        }

        public int Index {
            get { return _index; }
            set { _index = value; }
        }

        public AbstractMeshElement()
        {
            _triangleMesh = null;
            _index = -1;
        }
    }
}