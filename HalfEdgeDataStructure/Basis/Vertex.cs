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
        /// First HalfEdge incident to this Vertex.
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
        /// Index of the Outgoing HalfEdge.
        /// </summary>
        public int HalfEdgeIndex {
            set { _halfEdgeIndex = value; }
        }

        /// <summary>
        /// Is the Vertex on the Border or not.
        /// </summary>
        public bool IsOnBorder {
            get { return IncidentHalfEdges.Any(he => he.IsOnBorder); }
        }

        /// <summary>
        /// All Vertices ajdacent to this Vertex (i.e. directly connected via a HalfEdge) in Clockwise manner.
        /// </summary>
        public IEnumerable<Vertex> NeighborVertices {
            get
            {
                foreach(var incidentHalfEdge in IncidentHalfEdges)
                    yield return incidentHalfEdge.EndVertex;
            }
        }

        /// <summary>
        /// All HalfEdges with this Vertex as StartPoint.
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
        /// Default Constructor, calls the Base Constructor.
        /// </summary>
        public Vertex()
            :base()
        {
            _position = new Double3();
            _halfEdgeIndex = -1;
        }

        /// <summary>
        /// Constructor with Position Values.
        /// </summary>
        /// <param name="x">X Position of the Vertex.</param>
        /// <param name="y">Y Position of the Vertex.</param>
        /// <param name="z">Z Position of the Vertex.</param>
        public Vertex(double x, double y, double z)
            :this()
        {
            _position = new Double3(x, y, z);
        }

        /// <summary>
        /// Constructor with Position Values.
        /// </summary>
        /// <param name="position">The Position of the Vertex.</param>
        public Vertex(Double3 position)
            :this(position.X, position.Y, position.Z)
        {
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
        /// Constructor that uses an existing Vertex to create a new Vertex.
        /// </summary>
        /// <param name="vertex">Existing Vertex.</param>
        public Vertex(Vertex vertex)
            :this(vertex.TriangleMesh, vertex.X, vertex.Y, vertex.Z)
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
        }

        /// <summary>
        /// HalfEdge connecting this Vertex to the <paramref name="other"/> Vertex.
        /// </summary>
        /// <param name="other"></param>
        /// <returns>A HalfEdge from this Vertex to the <paramref name="other"/> Vertex, if it exists. Null otherwise.</returns>
        public HalfEdge HalfEdgeTo(Vertex other)
        {
            foreach(var incidentHalfEdge in IncidentHalfEdges)
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
        /// Creates a String Representation of this Vertex.
        /// </summary>
        /// <returns>String representation of this Vertex.</returns>
        public override string ToString()
        {
            return $"[{_position.ToString()}]";
        }
    }
}