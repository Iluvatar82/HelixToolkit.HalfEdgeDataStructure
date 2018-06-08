using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace HalfEdgeDataStructure
{
    /// <summary>
    /// Represents a Vertex of the HalfEdge DataStructure
    /// </summary>
    [Serializable]
    public class Vertex : AbstractMeshElement, ICloneable, ISerializable
    {
        private double _x;
        private double _y;
        private double _z;
        private int _halfEdgeIndex;

        /// <summary>
        /// X Position
        /// </summary>
        public double X {
            get { return _x; }
            set { _x = value; }
        }

        /// <summary>
        /// Y Position
        /// </summary>
        public double Y {
            get { return _y; }
            set { _y = value; }
        }

        /// <summary>
        /// Z Position
        /// </summary>
        public double Z {
            get { return _z; }
            set { _z = value; }
        }

        /// <summary>
        /// One HalfEdge incident to this Vertex
        /// </summary>
        public HalfEdge OutgoingHalfEdge {
            get
            {
                if (TriangleMesh != null && _halfEdgeIndex > -1 && _halfEdgeIndex < TriangleMesh.HalfEdges.Count)
                    return TriangleMesh.HalfEdges[_halfEdgeIndex];

                return null;
            }
        }

        /// <summary>
        /// Index of the Outgoing HalfEdge
        /// </summary>
        public int HalfEdgeIndex {
            set { _halfEdgeIndex = value; }
        }

        /// <summary>
        /// Is the Vertex on the Border or not
        /// </summary>
        public bool IsOnBorder {
            get { return IncidentHalfEdges.Any(he => he.IsOnBorder); }
        }

        /// <summary>
        /// All Vertices ajdacent to this Vertex (i.e. directly connected via a HalfEdge)
        /// </summary>
        public IEnumerable<Vertex> NeighborVertices {
            get
            {
                foreach(var incidentHalfEdge in IncidentHalfEdges)
                    yield return incidentHalfEdge.EndPoint;
            }
        }

        /// <summary>
        /// All HalfEdges with this Vertex as StartPoint
        /// </summary>
        public IEnumerable<HalfEdge> IncidentHalfEdges {
            get
            {
                var currentHalfEdge = OutgoingHalfEdge;
                if(OutgoingHalfEdge != default(HalfEdge))
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
        /// Default Constructor, calls the Base Constructor
        /// </summary>
        public Vertex()
            :base()
        {
            _x = 0;
            _y = 0;
            _z = 0;
            _halfEdgeIndex = -1;
        }

        /// <summary>
        /// Constructor with Position Values
        /// </summary>
        /// <param name="x">X - Position of the Vertex</param>
        /// <param name="y">Y - Position of the Vertex</param>
        /// <param name="z">Z - Position of the Vertex</param>
        public Vertex(double x, double y, double z)
            :this()
        {
            _x = x;
            _y = y;
            _z = z;
        }

        /// <summary>
        /// Constructor with the HalfEdgeMesh and Position Values
        /// </summary>
        /// <param name="triangleMesh">The HalfEdgeMesh this Vertex belongs to</param>
        /// <param name="x">X - Position of the Vertex</param>
        /// <param name="y">Y - Position of the Vertex</param>
        /// <param name="z">Z - Position of the Vertex</param>
        public Vertex(HalfEdgeMesh triangleMesh, double x, double y, double z)
            :this(x, y, z)
        {
            TriangleMesh = triangleMesh;
        }

        /// <summary>
        /// Constructor that uses an existing Vertex to create a new Vertex
        /// </summary>
        /// <param name="vertex">Existing Vertex</param>
        public Vertex(Vertex vertex)
            :this(vertex.TriangleMesh, vertex.X, vertex.Y, vertex.Z)
        { }

        /// <summary>
        /// Constructor that uses an existing Vector to create a new Vertex
        /// </summary>
        /// <param name="vector">Existing Vector</param>
        public Vertex(Vector vector)
            : this(vector.X, vector.Y, vector.Z)
        { }

        /// <summary>
        /// Constructor that is used by the Deserialization
        /// </summary>
        /// <param name="info">SerializationInfo</param>
        /// <param name="context">StreamingContext</param>
        public Vertex(SerializationInfo info, StreamingContext context)
        {
            TriangleMesh = (HalfEdgeMesh)info.GetValue("HalfEdgeMesh", typeof(HalfEdgeMesh));
            Index = -1;
            _x = (double)info.GetValue("X", typeof(double));
            _y = (double)info.GetValue("Y", typeof(double));
            _z = (double)info.GetValue("Z", typeof(double));
            _halfEdgeIndex = -1;
        }


        /// <summary>
        /// Clone this Vertex and return a Copy of it
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            return new Vertex(this);
        }

        /// <summary>
        /// Serialize the Vertex
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("HalfEdgeMesh", TriangleMesh, typeof(HalfEdgeMesh));
            info.AddValue("X", _x, typeof(double));
            info.AddValue("Y", _x, typeof(double));
            info.AddValue("Z", _x, typeof(double));
        }

        /// <summary>
        /// HalfEdge connecting this Vertex to the <para=other>other</para> Vertex
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public HalfEdge HalfEdgeTo(Vertex other)
        {
            foreach(var incidentHalfEdge in IncidentHalfEdges)
            {
                if (incidentHalfEdge.EndPoint == other)
                    return incidentHalfEdge;
            }

            return null;
        }


        /// <summary>
        /// Calculates the Vector between two Vertices
        /// </summary>
        /// <param name="end">End Vertex</param>
        /// <param name="start">Start Vertex</param>
        /// <returns>Vector ranging from the Start Vertex to the End Vertex</returns>
        public static Vector operator -(Vertex end, Vertex start)
        {
            return new Vector(end.X - start.X, end.Y - start.Y, end.Z - start.Z);
        }

        /// <summary>
        /// Creates a String Representation of the Vertex
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"[{_x:0.00}; {_y:0.00}; {_z:0.00}]";
        }
    }
}