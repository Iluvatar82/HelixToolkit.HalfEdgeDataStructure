using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace HalfEdgeDataStructure
{
    /// <summary>
    /// Represents a Triangle of the HalfEdge DataStructure.
    /// </summary>
    [Serializable]
    public class Triangle : AbstractMeshElement, ICloneable, ISerializable
    {
        private int _vertexIndex1;
        private int _vertexIndex2;
        private int _vertexIndex3;
        private int _halfEdgeIndex;
        private Vector _normal;
        private double _area;


        /// <summary>
        /// First Vertex of this Triangle.
        /// </summary>
        public Vertex Vertex1 {
            get
            {
                if(TriangleMesh != null && _vertexIndex1 > -1 && _vertexIndex1 < TriangleMesh.Vertices.Count)
                    return TriangleMesh.Vertices[_vertexIndex1];

                return null;
            }
        }

        /// <summary>
        /// Second Vertex of this Triangle.
        /// </summary>
        public Vertex Vertex2 {
            get
            {
                if(TriangleMesh != null && _vertexIndex2 > -1 && _vertexIndex2 < TriangleMesh.Vertices.Count)
                    return TriangleMesh.Vertices[_vertexIndex2];

                return null;
            }
        }

        /// <summary>
        /// Third Vertex of this Triangle.
        /// </summary>
        public Vertex Vertex3 {
            get
            {
                if(TriangleMesh != null && _vertexIndex3 > -1 && _vertexIndex3 < TriangleMesh.Vertices.Count)
                    return TriangleMesh.Vertices[_vertexIndex3];

                return null;
            }
        }

        /// <summary>
        /// Indices of all three Vertices of this Triangle
        /// </summary>
        public int[] VertexIndizes {
            get
            {
                return new int[]
                {
                    _vertexIndex1,
                    _vertexIndex2,
                    _vertexIndex3
                };
            }
        }

        /// <summary>
        /// The Vertices of this Triangle (first <see cref="Vertex1"/>, <see cref="Vertex2"/> then <see cref="Vertex3"/>).
        /// </summary>
        public Vertex[] Vertices {
            get
            {
                return new Vertex[]
                {
                    Vertex1,
                    Vertex2,
                    Vertex3
                };
            }
        }

        /// <summary>
        /// First HalfEdge incident to this Triangle.
        /// </summary>
        public HalfEdge HalfEdge {
            get
            {
                if(TriangleMesh != null && _halfEdgeIndex > -1 && _halfEdgeIndex < TriangleMesh.HalfEdges.Count)
                    return TriangleMesh.HalfEdges[_halfEdgeIndex];

                return null;
            }
        }

        /// <summary>
        /// All HalfEdges of this Triangle.
        /// </summary>
        public HalfEdge[] HalfEdges {
            get
            {
                return new HalfEdge[]
                {
                    HalfEdge,
                    HalfEdge.NextHalfEdge,
                    HalfEdge.PreviousHalfEdge
                };
            }
        }

        /// <summary>
        /// All adjacent Triangles of this Triangle.
        /// </summary>
        public Triangle[] Triangles {
            get
            {
                return new Triangle[]
                {
                    HalfEdge.OppositeTriangle,
                    HalfEdge.NextHalfEdge.OppositeTriangle,
                    HalfEdge.PreviousHalfEdge.OppositeTriangle
                };
            }
        }

        /// <summary>
        /// All HalfEdges that exist between the three Vertices.
        /// </summary>
        public IEnumerable<HalfEdge> ExistingHalfEdgesBetweenVertices {
            get
            {
                var result = Vertex1.HalfEdgeTo(Vertex2);
                if(result != null)
                    yield return result;
                result = Vertex2.HalfEdgeTo(Vertex3);
                if(result != null)
                    yield return result;
                result = Vertex3.HalfEdgeTo(Vertex1);
                if(result != null)
                    yield return result;
            }
        }

        /// <summary>
        /// The Index of the first HalfEdge of this Triangle.
        /// </summary>
        public int HalfEdgeIndex {
            set { _halfEdgeIndex = value; }
        }

        /// <summary>
        /// The Normal of this Triangle.
        /// </summary>
        public Vector Normal {
            get { return _normal; }
            set { _normal = value; }
        }

        /// <summary>
        /// The Area of this Triangle.
        /// </summary>
        public double Area {
            get { return _area; }
        }

        /// <summary>
        /// The signed Volume of the Triangle.
        /// </summary>
        public double SignedVolume {
            get
            {
                if(TriangleMesh == null)
                    return 0;

                return Vector.Dot(Vertex1, Vector.Cross(Vertex2, Vertex3)) / 6.0;
            }
        }

        /// <summary>
        /// Reference to the TriangleMesh.
        /// </summary>
        public override HalfEdgeMesh TriangleMesh {
            get { return base.TriangleMesh; }
            set {
                base.TriangleMesh = value;

                if(value != null)
                    Calculate();
            }
        }

        /// <summary>
        /// Is the Triangle on the Border or not.
        /// </summary>
        public bool IsOnBorder {
            get { return HalfEdge.IsOnBorder || HalfEdge.NextHalfEdge.IsOnBorder || HalfEdge.PreviousHalfEdge.IsOnBorder; }
        }


        /// <summary>
        /// Default Constructor, calls the Base Constructor.
        /// </summary>
        public Triangle()
            :base()
        {
            _vertexIndex1 = -1;
            _vertexIndex2 = -1;
            _vertexIndex3 = -1;
            _halfEdgeIndex = -1;

            _normal = new Vector();
            _area = 0;
        }

        /// <summary>
        /// Constructor with three Vertex Indices.
        /// </summary>
        /// <param name="vertex1">Index of the first Vertex.</param>
        /// <param name="vertex2">Index of the second Vertex.</param>
        /// <param name="vertex3">Index of the third Vertex.</param>
        public Triangle(int vertex1, int vertex2, int vertex3)
            :this()
        {
            _vertexIndex1 = vertex1;
            _vertexIndex2 = vertex2;
            _vertexIndex3 = vertex3;
        }

        /// <summary>
        /// Constructor with the HalfEdgeMesh and three Vertex Indices.
        /// Since also the Positions are clear, the <see cref="Normal"/> and the <see cref="Area"/> are calculated.
        /// </summary>
        /// <param name="triangleMesh">The HalfEdgeMesh this Vertex belongs to.</param>
        /// <param name="vertex1">Index of the first Vertex.</param>
        /// <param name="vertex2">Index of the second Vertex.</param>
        /// <param name="vertex3">Index of the third Vertex.</param>
        public Triangle(HalfEdgeMesh triangleMesh, int vertex1, int vertex2, int vertex3)
            :this(vertex1, vertex2, vertex3)
        {
            TriangleMesh = triangleMesh;

            if (TriangleMesh != null)
                Calculate();
        }

        /// <summary>
        /// Constructor that uses an existing Triangle to create a new Triangle.
        /// </summary>
        /// <param name="triangle">Existing Triangle.</param>
        public Triangle(Triangle triangle)
            : this(triangle.TriangleMesh, triangle._vertexIndex1, triangle._vertexIndex2, triangle._vertexIndex3)
        { }

        /// <summary>
        /// Constructor that is used by the Deserialization.
        /// </summary>
        /// <param name="info">The SerializationInfo.</param>
        /// <param name="context">The StreamingContext.</param>
        public Triangle(SerializationInfo info, StreamingContext context)
        {
            TriangleMesh = (HalfEdgeMesh)info.GetValue("TriangleMesh", typeof(HalfEdgeMesh));
            Index = -1;
            _vertexIndex1 = (int)info.GetValue("VertexIndex1", typeof(int));
            _vertexIndex2 = (int)info.GetValue("VertexIndex2", typeof(int));
            _vertexIndex3 = (int)info.GetValue("VertexIndex3", typeof(int));
            _halfEdgeIndex = -1;

            Calculate();
        }


        /// <summary>
        /// Clone this Triangle and return a Copy of it.
        /// </summary>
        /// <returns>Cloned Object.</returns>
        public object Clone()
        {
            return new Triangle(this);
        }

        /// <summary>
        /// Serialize the Triangle.
        /// </summary>
        /// <param name="info">The SerializationInfo.</param>
        /// <param name="context">The StreamingContext.</param>
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("TriangleMesh", TriangleMesh, typeof(HalfEdgeMesh));
            info.AddValue("VertexIndex1", _vertexIndex1, typeof(int));
            info.AddValue("VertexIndex2", _vertexIndex2, typeof(int));
            info.AddValue("VertexIndex3", _vertexIndex3, typeof(int));
        }

        /// <summary>
        /// Calculate additional Properties of the Triangle like <see cref="Normal"/> and <see cref="Area"/>.
        /// </summary>
        private void Calculate()
        {
            if(TriangleMesh == null)
                return;

            var vector12 = TriangleMesh.Vertices[_vertexIndex2] - TriangleMesh.Vertices[_vertexIndex1];
            var vector13 = TriangleMesh.Vertices[_vertexIndex3] - TriangleMesh.Vertices[_vertexIndex1];

            var cross = Vector.Cross(vector12, vector13);
            _area = cross.Length * 0.5;
            cross.Normalize();
            _normal = cross;
        }

        /// <summary>
        /// Shift the Indices of the Vertieces so that the provided Index ist first in the List.
        /// If the <paramref name="firstPointIndex"/> is not in the List of Vertex Indices of this Triangle nothing happens.
        /// </summary>
        /// <param name="firstPointIndex">The Index of the Vertex which should be first in the List of Indices.</param>
        public void SetFirstPointIndex(int firstPointIndex)
        {
            if(_vertexIndex1 != firstPointIndex && _vertexIndex2 != firstPointIndex && _vertexIndex3 != firstPointIndex)
                return;

            while(_vertexIndex1 != firstPointIndex)
            {
                var tempIndex = _vertexIndex1;
                _vertexIndex1 = _vertexIndex2;
                _vertexIndex2 = _vertexIndex3;
                _vertexIndex3 = tempIndex;
            }
        }

        /// <summary>
        /// The Angle between the two adjacent <see cref="Triangles"/> if the Triangle <paramref name="triangle"/> is adjacent to this Triangle.
        /// </summary>
        public double AngleTo(Triangle triangle)
        {
            if(!Triangles.Contains(triangle))
                return 0;

            return HalfEdges.First(he => he.Triangles[1] == triangle).Angle;
        }


        /// <summary>
        /// Creates a String Representation of this Triangle.
        /// </summary>
        /// <returns>String representation of this Triangle.</returns>
        public override string ToString()
        {
            return $"Vertex1: {Vertex1.ToString()} " +
                   $"Vertex2: {Vertex2.ToString()} " +
                   $"Vertex3: {Vertex3.ToString()} " +
                   $"Normal: {_normal.ToString()} " +
                   $"Area: [{_area:0.00}]";
        }
    }
}