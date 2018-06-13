using System;

namespace HalfEdgeDataStructure
{
    /// <summary>
    /// Represents a HalfEdge of the HalfEdge DataStructure.
    /// </summary>
    public class HalfEdge : AbstractMeshElement, ICloneable
    {
        private int _startVertexIndex;
        private int _endVertexIndex;
        private int _triangleIndex;
        private double _length;
        private int _nextHalfEdgeIndex;
        private int _previousHalfEdgeIndex;
        private int _oppositeHalfeEdgeIndex;


        /// <summary>
        /// The Start Vertex of this HalfEdge.
        /// </summary>
        public Vertex StartVertex {
            get { return TriangleMesh.Vertices[_startVertexIndex]; }
        }

        /// <summary>
        /// The End Vertex of this HalfEdge.
        /// </summary>
        public Vertex EndVertex {
            get { return TriangleMesh.Vertices[_endVertexIndex]; }
        }

        /// <summary>
        /// The Indices of the Vertices of this HalfEdge (first Index of the <see cref="StartVertex"/> then Index of the <see cref="EndVertex"/>).
        /// </summary>
        public int[] VertexIndizes {
            get
            {
                return new int[]
                {
                    _startVertexIndex,
                    _endVertexIndex
                };
            }
            set
            {
                _startVertexIndex = value[0];
                _endVertexIndex = value[1];
            }

        }

        /// <summary>
        /// The Vertices of this HalfEdge (first <see cref="StartVertex"/> then <see cref="EndVertex"/>).
        /// </summary>
        public Vertex[] Vertices {
            get
            {
                return new Vertex[]
                {
                    StartVertex,
                    EndVertex
                };
            }
        }

        /// <summary>
        /// The Length of the HalfEdge.
        /// </summary>
        public double Length {
            get {
                if(_length == -1)
                    Calculate();

                return _length;
            }
        }

        /// <summary>
        /// The Triangle this HalfEdge belongs to.
        /// </summary>
        public Triangle Triangle {
            get
            {
                if(_triangleIndex == -1)
                    return null;

                return TriangleMesh.Triangles[_triangleIndex];
            }
        }

        /// <summary>
        /// The Triangle of the opposite HalfEdge of this HalfEdge.
        /// </summary>
        public Triangle OppositeTriangle {
            get { return OppositeHalfEdge.Triangle; }
        }

        /// <summary>
        /// The <see cref="Triangle"/> of this HalfEdge and the Triangle of it's <see cref="OppositeHalfEdge"/>.
        /// </summary>
        public Triangle[] Triangles {
            get
            {
                return new Triangle[]
                {
                    Triangle,
                    OppositeTriangle
                };
            }
        }

        /// <summary>
        /// The Angle between the two adjacent <see cref="Triangles"/> of this HalfEdge if possible.
        /// </summary>
        public double Angle {
            get
            {
                var triangles = Triangles;
                if(triangles[0] == null || triangles[1] == null)
                    return 0;

                return Vector.CalculateAngle(triangles[0].Normal, triangles[1].Normal);
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
            get { return TriangleMesh.HalfEdges[_nextHalfEdgeIndex]; }
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
                if(_oppositeHalfeEdgeIndex == -1)
                    return null;

                return TriangleMesh.HalfEdges[_oppositeHalfeEdgeIndex];
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
            get { return TriangleMesh.HalfEdges[_previousHalfEdgeIndex]; }
        }

        /// <summary>
        /// The Index of the previous HalfEdge of this HalfEdge.
        /// </summary>
        public int PreviousHalfEdgeIndex {
            set { _previousHalfEdgeIndex = value; }
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
        private HalfEdge()
            :base()
        {
            _startVertexIndex = -1;
            _endVertexIndex = -1;
            _length = -1;
            _triangleIndex = -1;
            _nextHalfEdgeIndex = -1;
            _previousHalfEdgeIndex = -1;
            _oppositeHalfeEdgeIndex = -1;
        }

        /// <summary>
        /// Constructor with the Index of the <see cref="StartVertex"/>.
        /// </summary>
        /// <param name="startIndex">Index of the <see cref="StartVertex"/>.</param>
        /// <param name="endIndex">Index of the <see cref="EndVertex"/>.</param>
        public HalfEdge(int startIndex, int endIndex)
            :this()
        {
            _startVertexIndex = startIndex;
            _endVertexIndex = endIndex;
        }

        /// <summary>
        /// Constructor with the HalfEdgeMesh and the Index of the <see cref="StartVertex"/>.
        /// Since also the Positions are clear, the <see cref="Length"/> is calculated.
        /// </summary>
        /// <param name="triangleMesh">The HalfEdgeMesh this HalfEdge belongs to.</param>
        /// <param name="startIndex">Index of the <see cref="StartVertex"/>.</param>
        /// <param name="endIndex">Index of the <see cref="EndVertex"/>.</param>
        public HalfEdge(HalfEdgeMesh triangleMesh, int startIndex, int endIndex)
            :this(startIndex, endIndex)
        {
            TriangleMesh = triangleMesh;
        }

        /// <summary>
        /// Constructor with the HalfEdgeMesh and other Arguments.
        /// </summary>
        /// <param name="triangleMesh">The HalfEdgeMesh this HalfEdge belongs to.</param>
        /// <param name="index">The Index of this HalfEdge in the HalfEdgeMesh.</param>
        /// <param name="startIndex">Index of the <see cref="StartVertex"/>.</param>
        /// <param name="endIndex">Index of the <see cref="EndVertex"/>.</param>
        /// <param name="triangleIndex">The Index of the Triangle this HalfEdge belongs to.</param>
        public HalfEdge(HalfEdgeMesh triangleMesh, int index, int startIndex, int endIndex, int triangleIndex)
            :this(triangleMesh, startIndex, endIndex)
        {
            Index = index;
            _triangleIndex = triangleIndex;
        }

        /// <summary>
        /// Constructor that uses an existing HalfEdge to create a new HalfEdge.
        /// </summary>
        /// <param name="triangle">Existing HalfEdge.</param>
        public HalfEdge(HalfEdge halfEdge)
            : this(halfEdge.TriangleMesh, halfEdge.Index, halfEdge._startVertexIndex, halfEdge._endVertexIndex, halfEdge._triangleIndex)
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