namespace HalfEdgeDataStructure
{
    /// <summary>
    /// Basic Properties for the HalfEdge DataStructure
    /// </summary>
    public abstract class AbstractMeshElement
    {
        private HalfEdgeMesh _triangleMesh;
        private int _index;

        /// <summary>
        /// Reference to the TriangleMesh
        /// </summary>
        public virtual HalfEdgeMesh TriangleMesh {
            get { return _triangleMesh; }
            set { _triangleMesh = value; }
        }

        /// <summary>
        /// Index of this HalfEdge Component
        /// </summary>
        public int Index {
            get { return _index; }
            set { _index = value; }
        }

        /// <summary>
        /// Abstract Constructor, initializes basic Properties
        /// </summary>
        public AbstractMeshElement()
        {
            _triangleMesh = null;
            _index = -1;
        }
    }
}