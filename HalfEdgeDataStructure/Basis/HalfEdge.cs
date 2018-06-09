using System;

namespace HalfEdgeDataStructure
{
    /// <summary>
    /// Represents a HalfEdge of the HalfEdge DataStructure.
    /// </summary>
    public class HalfEdge : AbstractMeshElement, ICloneable
    {
        private int _startVertexIndex;
        private int _triangleIndex;
        private double _length;
        private int _nextHalfEdgeIndex;
        private int _previousHalfEdgeIndex;
        private int _oppositeHalfeEdgeIndex;


        /// <summary>
        /// The Start Vertex of this HalfEdge.
        /// </summary>
        public Vertex StartVertex {
            get
            {
                if(TriangleMesh != null && _startVertexIndex > -1 && _startVertexIndex < TriangleMesh.Vertices.Count)
                    return TriangleMesh.Vertices[_startVertexIndex];

                return null;
            }
        }

        /// <summary>
        /// The End Vertex of this HalfEdge.
        /// </summary>
        public Vertex EndVertex {
            get
            {
                var opposite = OppositeHalfEdge;
                if(TriangleMesh != null && opposite != null)
                    return opposite.StartVertex;

                return null;
            }
        }

        /// <summary>
        /// The Indices of the Vertices of this HalfEdge.
        /// </summary>
        public int[] VertexIndizes {
            get
            {
                return new int[]
                {
                    _startVertexIndex,
                    EndVertex.Index
                };
            }
        }

        /// <summary>
        /// The Triangle this HalfEdge belongs to.
        /// </summary>
        public Triangle Triangle {
            get
            {
                if(TriangleMesh != null && _triangleIndex > -1 && _triangleIndex < TriangleMesh.Triangles.Count)
                    return TriangleMesh.Triangles[_triangleIndex];

                return null;
            }
        }

        /// <summary>
        /// The Index of <see cref="Triangle"/> this HalfEdge belongs to.
        /// </summary>
        public int TriangleIndex {
            set { _triangleIndex = value; }
        }

        /// <summary>
        /// The next HalfEdge of this HalfEdge.
        /// It's <see cref="StartVertex"/> is the <see cref="EndVertex"/> of this HalfEdge and it's <see cref="Triangle"/> is the same.
        /// </summary>
        public HalfEdge NextHalfEdge {
            get
            {
                if(TriangleMesh != null && _nextHalfEdgeIndex > -1 && _nextHalfEdgeIndex < TriangleMesh.HalfEdges.Count)
                    return TriangleMesh.HalfEdges[_nextHalfEdgeIndex];

                return null;
            }
        }

        /// <summary>
        /// The Index of the next HalfEdge of this HalfEdge.
        /// </summary>
        public int NextHalfEdgeIndex {
            set { _nextHalfEdgeIndex = value; }
        }

        /// <summary>
        /// The opposite HalfEdge of this HalfEdge. It's <see cref="StartVertex"/> is this HalfEdge's <see cref="EndVertex"/> and vice versa.
        /// The opposite HalfEdge belongs to another <see cref="Triangle"/>.
        /// </summary>
        public HalfEdge OppositeHalfEdge {
            get
            {
                if(TriangleMesh != null && _oppositeHalfeEdgeIndex > -1 && _oppositeHalfeEdgeIndex < TriangleMesh.HalfEdges.Count)
                    return TriangleMesh.HalfEdges[_oppositeHalfeEdgeIndex];

                return null;
            }
        }

        /// <summary>
        /// The Index of the opposite HalfEdge of this HalfEdge.
        /// </summary>
        public int OppositeHalfEdgeIndex {
            set { _oppositeHalfeEdgeIndex = value; }
        }

        /// <summary>
        /// The previous HalfEdge of this HalfEdge.
        /// It's <see cref="EndVertex"/> is the <see cref="StartVertex"/> of this HalfEdge and it's <see cref="Triangle"/> is the same.
        /// </summary>
        public HalfEdge PreviousHalfEdge {
            get
            {
                if(TriangleMesh != null && _previousHalfEdgeIndex > -1 && _previousHalfEdgeIndex < TriangleMesh.HalfEdges.Count)
                    return TriangleMesh.HalfEdges[_previousHalfEdgeIndex];

                return null;
            }
        }

        /// <summary>
        /// The Index of the previous HalfEdge of this HalfEdge.
        /// </summary>
        public int PreviousHalfEdgeIndex {
            set { _previousHalfEdgeIndex = value; }
        }

        /// <summary>
        /// The Length of the HalfEdge.
        /// </summary>
        public double Length {
            get { return _length; }
        }

        /// <summary>
        /// Reference to the TriangleMesh.
        /// </summary>
        public override HalfEdgeMesh TriangleMesh {
            get { return base.TriangleMesh; }
            set
            {
                base.TriangleMesh = value;

                if(value != null)
                    Calculate();
            }
        }

        /// <summary>
        /// Is the HalfEdge on the Border or not.
        /// </summary>
        public bool IsOnBorder {
            get { return Triangle == null || OppositeHalfEdge.Triangle == null; }
        }



        /// <summary>
        /// Default Constructor, calls the Base Constructor.
        /// </summary>
        public HalfEdge()
            :base()
        {
            _startVertexIndex = -1;
            _length = 0;
            _triangleIndex = -1;
            _nextHalfEdgeIndex = -1;
            _previousHalfEdgeIndex = -1;
            _oppositeHalfeEdgeIndex = -1;
        }

        /// <summary>
        /// Constructor with the Index of the <see cref="StartVertex"/>.
        /// </summary>
        /// <param name="startIndex">Index of the <see cref="StartVertex"/>.</param>
        public HalfEdge(int startIndex)
            :this()
        {
            _startVertexIndex = startIndex;
        }

        /// <summary>
        /// Constructor with the HalfEdgeMesh and the Index of the <see cref="StartVertex"/>.
        /// Since also the Positions are clear, the <see cref="Length"/> is calculated.
        /// </summary>
        /// <param name="triangleMesh">The HalfEdgeMesh this HalfEdge belongs to.</param>
        /// <param name="startIndex">Index of the <see cref="StartVertex"/>.</param>
        public HalfEdge(HalfEdgeMesh triangleMesh, int startIndex)
            :this(startIndex)
        {
            TriangleMesh = triangleMesh;

            if(TriangleMesh != null)
                Calculate();
        }

        /// <summary>
        /// Constructor with the HalfEdgeMesh and other Arguments.
        /// </summary>
        /// <param name="triangleMesh">The HalfEdgeMesh this HalfEdge belongs to.</param>
        /// <param name="index">The Index of this HalfEdge in the HalfEdgeMesh.</param>
        /// <param name="startIndex">Index of the <see cref="StartVertex"/>.</param>
        /// <param name="triangleIndex">The Index of the Triangle this HalfEdge belongs to.</param>
        public HalfEdge(HalfEdgeMesh triangleMesh, int index, int startIndex, int triangleIndex)
            :this(triangleMesh, startIndex)
        {
            Index = index;
            _triangleIndex = triangleIndex;
        }

        /// <summary>
        /// Constructor that uses an existing HalfEdge to create a new HalfEdge.
        /// </summary>
        /// <param name="triangle">Existing HalfEdge.</param>
        public HalfEdge(HalfEdge halfEdge)
            : this(halfEdge.TriangleMesh, halfEdge._startVertexIndex)
        { }


        /// <summary>
        /// Clone this HalfEdge and return a Copy of it.
        /// </summary>
        /// <returns>Cloned Object.</returns>
        public object Clone()
        {
            return new HalfEdge(this);
        }

        /// <summary>
        /// Calculate the <see cref="Length"/> of this HalfEdge.
        /// </summary>
        private void Calculate()
        {
            if (OppositeHalfEdge != null)
                _length = (EndVertex - StartVertex).Length;
        }


        /// <summary>
        /// Creates a String Representation of this HalfEdge.
        /// </summary>
        /// <returns>String representation of this HalfEdge.</returns>
        public override string ToString()
        {
            return $"From: {StartVertex.ToString()} " +
                   $"To: {EndVertex.ToString()}";
        }
    }
}