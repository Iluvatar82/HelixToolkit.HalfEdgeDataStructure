using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace HalfEdgeDataStructure
{
    /// <summary>
    /// Represents a Vertex of the HalfEdge DataStructure.
    /// </summary>
    [Serializable]
    public class Vertex : AbstractMeshElement, ICloneable, ISerializable
    {
        private Float3 _position;
        private Float3 _textureCoordinate;
        private Vector _normal;
        private int _halfEdgeIndex;


        /// <summary>
        /// X Position.
        /// </summary>
        public float X {
            get { return _position.X; }
            set { _position.X = value; }
        }

        /// <summary>
        /// Y Position.
        /// </summary>
        public float Y {
            get { return _position.Y; }
            set { _position.Y = value; }
        }

        /// <summary>
        /// Z Position.
        /// </summary>
        public float Z {
            get { return _position.Z; }
            set { _position.Z = value; }
        }

        /// <summary>
        /// Position of the Vertex.
        /// </summary>
        public Float3 Position {
            get { return _position; }
            set { _position = value; }
        }

        /// <summary>
        /// TextureCoordinate of the Vertex.
        /// </summary>
        public Float3 TextureCoordinate {
            get { return _textureCoordinate; }
            set { _textureCoordinate = value; }
        }

        /// <summary>
        /// Normal of the Vertex.
        /// </summary>
        public Vector Normal {
            get { return _normal; }
            set { _normal = value; }
        }

        /// <summary>
        /// First HalfEdge incident to this Vertex.
        /// </summary>
        public HalfEdge HalfEdge {
            get
            {
                if(_halfEdgeIndex == -1)
                    return null;

                return TriangleMesh.HalfEdges[_halfEdgeIndex];
            }
        }

        /// <summary>
        /// Index of the Outgoing HalfEdge.
        /// </summary>
        public int HalfEdgeIndex {
            set { _halfEdgeIndex = value; }
        }

        /// <summary>
        /// Is the Vertex on the Border or not.
        /// </summary>
        public bool IsOnBorder {
            get { return HalfEdges.Any(he => he.IsOnBorder); }
        }

        /// <summary>
        /// All Vertices ajdacent to this Vertex (i.e. directly connected via a <see cref="HalfEdge"/>) in Counter Clockwise manner.
        /// </summary>
        public IEnumerable<Vertex> Vertices {
            get
            {
                foreach(var incidentHalfEdge in HalfEdges)
                    yield return incidentHalfEdge.EndVertex;
            }
        }

        /// <summary>
        /// All HalfEdges with this Vertex as <see cref="HalfEdge.StartVertex"/> in Counter Clockwise manner.
        /// </summary>
        public IEnumerable<HalfEdge> HalfEdges {
            get
            {
                var currentHalfEdge = HalfEdge;

                if(HalfEdge != default(HalfEdge))
                {
                    var firstHalfEdge = currentHalfEdge;
                    do
                    {
                        yield return currentHalfEdge;

                        currentHalfEdge = currentHalfEdge.PreviousHalfEdge.OppositeHalfEdge;
                    }
                    while(currentHalfEdge != firstHalfEdge);
                }
            }
        }

        /// <summary>
        /// All existing Triangles adjacent to this Vertex in Counter Clockwise manner.
        /// </summary>
        public IEnumerable<Triangle> Triangles {
            get
            {
                foreach(var halfEdge in HalfEdges)
                {
                    if(halfEdge.Triangle != null)
                        yield return halfEdge.Triangle;
                }
            }
        }

        /// <summary>
        /// Default Constructor, calls the Base Constructor.
        /// </summary>
        private Vertex()
            :base()
        {
            _position = default(Float3);
            _textureCoordinate = default(Float3);
            _normal = Vector.Zero;
            _halfEdgeIndex = -1;
        }

        /// <summary>
        /// Constructor with Position Values.
        /// </summary>
        /// <param name="x">X Position of the Vertex.</param>
        /// <param name="y">Y Position of the Vertex.</param>
        /// <param name="z">Z Position of the Vertex.</param>
        public Vertex(float x, float y, float z)
            : this()
        {
            _position = new Float3(x, y, z);
        }

        /// <summary>
        /// Constructor with Position and TextureCoordinate Values.
        /// </summary>
        /// <param name="x">X Position of the Vertex.</param>
        /// <param name="y">Y Position of the Vertex.</param>
        /// <param name="z">Z Position of the Vertex.</param>
        /// <param name="u">U TextureCoordinate of the Vertex.</param>
        /// <param name="v">V TextureCoordinate of the Vertex.</param>
        /// <param name="w">W TextureCoordinate of the Vertex.</param>
        public Vertex(float x, float y, float z, float u, float v, float w)
            : this(x, y, z)
        {
            _textureCoordinate = new Float3(u, v, w);
        }

        /// <summary>
        /// Constructor with Position and TextureCoordinate Values.
        /// </summary>
        /// <param name="x">X Position of the Vertex.</param>
        /// <param name="y">Y Position of the Vertex.</param>
        /// <param name="z">Z Position of the Vertex.</param>
        /// <param name="u">U TextureCoordinate of the Vertex.</param>
        /// <param name="v">V TextureCoordinate of the Vertex.</param>
        /// <param name="w">W TextureCoordinate of the Vertex.</param>
        /// <param name="nx">X Direction of the Normal of the Vertex.</param>
        /// <param name="ny">Y Direction of the Normal of the Vertex.</param>
        /// <param name="nz">Z Direction of the Normal of the Vertex.</param>
        public Vertex(float x, float y, float z, float u, float v, float w, float nx, float ny, float nz)
            : this(x, y, z, u, v, w)
        {
            _normal = new Vector(nx, ny, nz);
        }

        /// <summary>
        /// Constructor with Position Values.
        /// </summary>
        /// <param name="position">The Position of the Vertex.</param>
        public Vertex(Float3 position)
            :this()
        {
            _position = position;
        }

        /// <summary>
        /// Constructor with Position and TextureCoordinate Values.
        /// </summary>
        /// <param name="position">The Position of the Vertex.</param>
        /// <param name="textureCoordinate">The TextureCoordinate of the Vertex.</param>
        public Vertex(Float3 position, Float3 textureCoordinate)
            : this(position)
        {
            _textureCoordinate = textureCoordinate;
        }

        /// <summary>
        /// Constructor with Position and TextureCoordinate Values.
        /// </summary>
        /// <param name="position">The Position of the Vertex.</param>
        /// <param name="textureCoordinate">The TextureCoordinate of the Vertex.</param>
        /// <param name="normal">The Normal of the Vertex.</param>
        public Vertex(Float3 position, Float3 textureCoordinate, Vector normal)
            : this(position, textureCoordinate)
        {
            _normal = normal;
        }

        /// <summary>
        /// Constructor with the HalfEdgeMesh and Position Values.
        /// </summary>
        /// <param name="triangleMesh">The HalfEdgeMesh this Vertex belongs to.</param>
        /// <param name="x">X Position of the Vertex.</param>
        /// <param name="y">Y Position of the Vertex.</param>
        /// <param name="z">Z Position of the Vertex.</param>
        public Vertex(HalfEdgeMesh triangleMesh, float x, float y, float z)
            :this(x, y, z)
        {
            TriangleMesh = triangleMesh;
        }

        /// <summary>
        /// Constructor with the HalfEdgeMesh and Position Values.
        /// </summary>
        /// <param name="triangleMesh">The HalfEdgeMesh this Vertex belongs to.</param>
        /// <param name="x">X Position of the Vertex.</param>
        /// <param name="y">Y Position of the Vertex.</param>
        /// <param name="z">Z Position of the Vertex.</param>
        /// <param name="u">U TextureCoordinate of the Vertex.</param>
        /// <param name="v">V TextureCoordinate of the Vertex.</param>
        /// <param name="w">W TextureCoordinate of the Vertex.</param>
        public Vertex(HalfEdgeMesh triangleMesh, float x, float y, float z, float u, float v, float w)
            : this(x, y, z, u, v, w)
        {
            TriangleMesh = triangleMesh;
        }


        /// <summary>
        /// Constructor with the HalfEdgeMesh and Position Values.
        /// </summary>
        /// <param name="triangleMesh">The HalfEdgeMesh this Vertex belongs to.</param>
        /// <param name="x">X Position of the Vertex.</param>
        /// <param name="y">Y Position of the Vertex.</param>
        /// <param name="z">Z Position of the Vertex.</param>
        /// <param name="u">U TextureCoordinate of the Vertex.</param>
        /// <param name="v">V TextureCoordinate of the Vertex.</param>
        /// <param name="w">W TextureCoordinate of the Vertex.</param>
        /// <param name="nx">X Direction of the Normal of the Vertex.</param>
        /// <param name="ny">Y Direction of the Normal of the Vertex.</param>
        /// <param name="nz">Z Direction of the Normal of the Vertex.</param>
        public Vertex(HalfEdgeMesh triangleMesh, float x, float y, float z, float u, float v, float w, float nx, float ny, float nz)
            : this(x, y, z, u, v, w, nx, ny, nz)
        {
            TriangleMesh = triangleMesh;
        }

        /// <summary>
        /// Constructor with the HalfEdgeMesh and Position Values.
        /// </summary>
        /// <param name="triangleMesh">The HalfEdgeMesh this Vertex belongs to.</param>
        /// <param name="position">The Position of the Vertex.</param>
        public Vertex(HalfEdgeMesh triangleMesh, Float3 position)
            : this(position)
        {
            TriangleMesh = triangleMesh;
        }

        /// <summary>
        /// Constructor with the HalfEdgeMesh and Position Values.
        /// </summary>
        /// <param name="triangleMesh">The HalfEdgeMesh this Vertex belongs to.</param>
        /// <param name="position">The Position of the Vertex.</param>
        /// <param name="textureCoordinate">The TextureCoordinate of the Vertex.</param>
        public Vertex(HalfEdgeMesh triangleMesh, Float3 position, Float3 textureCoordinate)
            : this(triangleMesh, position)
        {
            _textureCoordinate = textureCoordinate;
        }


        /// <summary>
        /// Constructor with the HalfEdgeMesh and Position Values.
        /// </summary>
        /// <param name="triangleMesh">The HalfEdgeMesh this Vertex belongs to.</param>
        /// <param name="position">The Position of the Vertex.</param>
        /// <param name="textureCoordinate">The TextureCoordinate of the Vertex.</param>
        /// <param name="normal">The Normal of the Vertex.</param>
        public Vertex(HalfEdgeMesh triangleMesh, Float3 position, Float3 textureCoordinate, Vector normal)
            : this (triangleMesh, position, textureCoordinate)
        {
            _normal = normal;
        }


        /// <summary>
        /// Constructor that uses an existing Vertex to create a new Vertex.
        /// </summary>
        /// <param name="vertex">Existing Vertex.</param>
        public Vertex(Vertex vertex)
            :this(vertex.TriangleMesh, vertex.Position, vertex.TextureCoordinate, vertex.Normal)
        { }

        /// <summary>
        /// Constructor that uses an existing Vector to create a new Vertex.
        /// </summary>
        /// <param name="vector">Existing Vector.</param>
        public Vertex(Vector vector)
            : this(vector.X, vector.Y, vector.Z)
        { }

        /// <summary>
        /// Constructor that is used by the Deserialization.
        /// </summary>
        /// <param name="info">The SerializationInfo.</param>
        /// <param name="context">The StreamingContext.</param>
        public Vertex(SerializationInfo info, StreamingContext context)
        {
            TriangleMesh = (HalfEdgeMesh)info.GetValue("HalfEdgeMesh", typeof(HalfEdgeMesh));
            Index = -1;
            _position = (Float3)info.GetValue("Position", typeof(Float3));
            _textureCoordinate = (Float3)info.GetValue("TextureCoordinate", typeof(Float3));
            _normal = (Vector)info.GetValue("Normal", typeof(Vector));
            _halfEdgeIndex = -1;
        }


        /// <summary>
        /// Clone this Vertex and return a Copy of it.
        /// </summary>
        /// <returns>Cloned Object.</returns>
        public object Clone()
        {
            return new Vertex(this);
        }

        /// <summary>
        /// Serialize the Vertex.
        /// </summary>
        /// <param name="info">The SerializationInfo.</param>
        /// <param name="context">The StreamingContext.</param>
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("HalfEdgeMesh", TriangleMesh, typeof(HalfEdgeMesh));
            info.AddValue("Position", Position, typeof(Float3));
            info.AddValue("TextureCoordinate", TextureCoordinate, typeof(Float3));
            info.AddValue("Normal", Normal, typeof(Vector));
        }

        /// <summary>
        /// HalfEdge connecting this Vertex to the <paramref name="other"/> Vertex.
        /// </summary>
        /// <param name="other"></param>
        /// <returns>A HalfEdge from this Vertex to the <paramref name="other"/> Vertex, if it exists. Null otherwise.</returns>
        public HalfEdge HalfEdgeTo(Vertex other)
        {
            foreach(var incidentHalfEdge in HalfEdges)
            {
                if (incidentHalfEdge.EndVertex == other)
                    return incidentHalfEdge;
            }

            return null;
        }

        /// <summary>
        /// Calculates the shortes Path to another Vertex using the existing Edges of the <see cref="HalfEdgeMesh"/>.
        /// </summary>
        /// <param name="other">The other Vertex to search the Path to.</param>
        /// <returns>List of HalfEdges that lead from this Vertex to the other Vertex, if the are both Part of the same HalfEdgeMesh.</returns>
        public List<HalfEdge> ShortestPathTo(Vertex other)
        {
            if(TriangleMesh != other.TriangleMesh)
                return null;


            ///TODO implement
            return null;
        }


        /// <summary>
        /// Calculates the Vector between two Vertices.
        /// </summary>
        /// <param name="end">The End Vertex.</param>
        /// <param name="start">The Start Vertex.</param>
        /// <returns>Vector ranging from the <paramref name="start"/> Vertex to the <paramref name="end"/> Vertex.</returns>
        public static Vector operator -(Vertex end, Vertex start)
        {
            return new Vector(end.X - start.X, end.Y - start.Y, end.Z - start.Z);
        }

        /// <summary>
        /// Implicitly create a Vector from the <see cref="Position"/> of a Vertex.
        /// </summary>
        /// <param name="vertex"></param>
        public static implicit operator Vector(Vertex vertex)
        {
            return new Vector(vertex.Position);
        }

        /// <summary>
        /// Implicitly create a Double3 from the <see cref="Position"/> of a Vertex.
        /// </summary>
        /// <param name="vertex"></param>
        public static implicit operator Float3(Vertex vertex)
        {
            return new Float3(vertex.Position);
        }


        /// <summary>
        /// Creates a String Representation of this Vertex.
        /// </summary>
        /// <returns>String representation of this Vertex.</returns>
        public override string ToString()
        {
            return $"[{_position.ToString()}]";
        }
    }
}