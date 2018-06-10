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
        private Double3 _position;
        private Double3 _textureCoordinate;
        private Vector _normal;
        private int _halfEdgeIndex;


        /// <summary>
        /// X Position.
        /// </summary>
        public double X {
            get { return _position.X; }
            set { _position.X = value; }
        }

        /// <summary>
        /// Y Position.
        /// </summary>
        public double Y {
            get { return _position.Y; }
            set { _position.Y = value; }
        }

        /// <summary>
        /// Z Position.
        /// </summary>
        public double Z {
            get { return _position.Z; }
            set { _position.Z = value; }
        }

        /// <summary>
        /// Position of the Vertex.
        /// </summary>
        public Double3 Position {
            get { return _position; }
            set { _position = value; }
        }

        /// <summary>
        /// TextureCoordinate of the Vertex.
        /// </summary>
        public Double3 TextureCoordinate {
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
        /// All Triangles adjacent to this Vertex in Counter Clockwise manner.
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
            _position = new Double3();
            _textureCoordinate = new Double3();
            _normal = new Vector(0, 0, 0);
            _halfEdgeIndex = -1;
        }

        /// <summary>
        /// Constructor with Position Values.
        /// </summary>
        /// <param name="x">X Position of the Vertex.</param>
        /// <param name="y">Y Position of the Vertex.</param>
        /// <param name="z">Z Position of the Vertex.</param>
        public Vertex(double x, double y, double z)
            : this()
        {
            _position = new Double3(x, y, z);
            _textureCoordinate = new Double3();
            _normal = new Vector(0, 0, 0);
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
        public Vertex(double x, double y, double z, double u, double v, double w)
            : this(x, y, z)
        {
            _textureCoordinate = new Double3(u, v, w);
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
        public Vertex(double x, double y, double z, double u, double v, double w, double nx, double ny, double nz)
            : this(x, y, z, u, v, w)
        {
            _normal = new Vector(nx, ny, nz);
        }

        /// <summary>
        /// Constructor with Position Values.
        /// </summary>
        /// <param name="position">The Position of the Vertex.</param>
        public Vertex(Double3 position)
            :this()
        {
            _position = position;
        }

        /// <summary>
        /// Constructor with Position and TextureCoordinate Values.
        /// </summary>
        /// <param name="position">The Position of the Vertex.</param>
        /// <param name="textureCoordinate">The TextureCoordinate of the Vertex.</param>
        public Vertex(Double3 position, Double3 textureCoordinate)
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
        public Vertex(Double3 position, Double3 textureCoordinate, Vector normal)
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
        public Vertex(HalfEdgeMesh triangleMesh, double x, double y, double z)
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
        public Vertex(HalfEdgeMesh triangleMesh, double x, double y, double z, double u, double v, double w)
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
        public Vertex(HalfEdgeMesh triangleMesh, double x, double y, double z, double u, double v, double w, double nx, double ny, double nz)
            : this(x, y, z, u, v, w, nx, ny, nz)
        {
            TriangleMesh = triangleMesh;
        }

        /// <summary>
        /// Constructor that uses an existing Vertex to create a new Vertex.
        /// </summary>
        /// <param name="vertex">Existing Vertex.</param>
        public Vertex(Vertex vertex)
            :this(vertex.TriangleMesh, vertex.X, vertex.Y, vertex.Z,
                 vertex.TextureCoordinate.U, vertex.TextureCoordinate.V, vertex.TextureCoordinate.W,
                 vertex.Normal.X, vertex.Normal.Y, vertex.Normal.Z)
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
            _position = (Double3)info.GetValue("Position", typeof(Double3));
            _textureCoordinate = (Double3)info.GetValue("TextureCoordinate", typeof(Double3));
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
            info.AddValue("Position", Position, typeof(Double3));
            info.AddValue("TextureCoordinate", TextureCoordinate, typeof(Double3));
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
        public static implicit operator Double3(Vertex vertex)
        {
            return new Double3(vertex.Position);
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