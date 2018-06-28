using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace HalfEdgeDataStructure
{
    public enum SubdivisionType
    {
        Default
    }


    /// <summary>
    /// Represents the complete Mesh of the HalfEdge DataStructure.
    /// </summary>
    [Serializable]
    public class HalfEdgeMesh : ICloneable, ISerializable
    {
        private List<Vertex> _vertices;
        private List<Triangle> _triangles;
        private List<HalfEdge> _halfEdges;
        private bool _hasNormals;
        private const float Epsilon = 0.001f;


        /// <summary>
        /// All Vertices of this Mesh.
        /// </summary>
        public List<Vertex> Vertices {
            get { return _vertices; }
            set { _vertices = value; }
        }

        /// <summary>
        /// All Triangles of this Mesh.
        /// </summary>
        public List<Triangle> Triangles {
            get { return _triangles; }
            set { _triangles = value; }
        }

        /// <summary>
        /// All HalfEdges of this Mesh.
        /// </summary>
        public List<HalfEdge> HalfEdges {
            get { return _halfEdges; }
            set { _halfEdges = value; }
        }

        /// <summary>
        /// All BoundaryVertices of this HalfEdgeMesh.
        /// Returns each Boundary Vertex two times to make it easy to render the Boundary.
        /// </summary>
        public IEnumerable<Vertex> BoundaryVertices {
            get
            {
                var foundBoundaryHalfEdge = new HashSet<HalfEdge>();
                HalfEdge currentBoundaryHalfEdge = null;
                do
                {
                    currentBoundaryHalfEdge = _halfEdges.FirstOrDefault(he => he.TriangleMesh != null && he.Triangle == null
                    && !foundBoundaryHalfEdge.Contains(he) && he.OppositeHalfEdge.Triangle != null);

                    if(currentBoundaryHalfEdge == default(HalfEdge))
                        break;

                    while(!foundBoundaryHalfEdge.Contains(currentBoundaryHalfEdge))
                    {
                        yield return currentBoundaryHalfEdge.StartVertex;
                        yield return currentBoundaryHalfEdge.EndVertex;

                        foundBoundaryHalfEdge.Add(currentBoundaryHalfEdge);

                        if (currentBoundaryHalfEdge.OppositeHalfEdge != default(HalfEdge))
                            foundBoundaryHalfEdge.Add(currentBoundaryHalfEdge.OppositeHalfEdge);

                        currentBoundaryHalfEdge = currentBoundaryHalfEdge.NextHalfEdge;
                    }
                }
                while(currentBoundaryHalfEdge != default(HalfEdge));
            }
        }

        /// <summary>
        /// Is the HalfEdgeMesh completely closed or not.
        /// </summary>
        public bool IsClosed {
            get { return BoundaryVertices.Count() == 0; }
        }

        /// <summary>
        /// The Volume of the HalfEdgeMesh if it is closed.
        /// </summary>
        public float Volume {
            get
            {
                if(!IsClosed)
                    return 0;

                return MathF.Abs(_triangles.Select(t => t.SignedVolume).Sum());
            }
        }

        /// <summary>
        /// Does the Mesh have Vertex Normals calcualted or not.
        /// </summary>
        public bool HasNormals {
            get { return _hasNormals; }
            set { _hasNormals = value; }
        }


        /// <summary>
        /// Default Constructor.
        /// </summary>
        public HalfEdgeMesh()
        {
            _vertices = new List<Vertex>();
            _triangles = new List<Triangle>();
            _halfEdges = new List<HalfEdge>();
            _hasNormals = false;
        }

        /// <summary>
        /// Constructor with all <see cref="Vertices"/> and <see cref="Triangles"/> of this Mesh.
        /// </summary>
        /// <param name="vertices">All Vertices of this Mesh.</param>
        /// <param name="triangles">All Triangles of this Mesh.</param>
        public HalfEdgeMesh(List<Vertex> vertices, List<Triangle> triangles)
            :this()
        {
            AddVertices(vertices);
            AddTriangles(triangles);
        }

        /// <summary>
        /// Constructor that uses an existing HalfEdgeMesh to create a new HalfEdgeMesh.
        /// </summary>
        /// <param name="triangle">Existing HalfEdgeMesh.</param>
        public HalfEdgeMesh(HalfEdgeMesh mesh)
        {
            _vertices = new List<Vertex>(mesh.Vertices);
            _triangles = new List<Triangle>(mesh.Triangles);
            _halfEdges = new List<HalfEdge>(mesh.HalfEdges);
        }

        /// <summary>
        /// Constructor that is used by the Deserialization.
        /// </summary>
        /// <param name="info">The SerializationInfo.</param>
        /// <param name="context">The StreamingContext.</param>
        public HalfEdgeMesh(SerializationInfo info, StreamingContext context)
            :this()
        {
            AddVertices((List<Vertex>)info.GetValue("Vertices", typeof(List<Vertex>)));
            AddTriangles((List<Triangle>)info.GetValue("Triangles", typeof(List<Triangle>)));
            _hasNormals = (bool)info.GetValue("HasNormals", typeof(bool));
            if(_hasNormals)
                CalculateVertexNormals();
        }


        /// <summary>
        /// Clone this HalfEdgeMesh and return a Copy of it.
        /// </summary>
        /// <returns>Cloned Object.</returns>
        public object Clone()
        {
            return new HalfEdgeMesh(this);
        }

        /// <summary>
        /// Serialize the HalfEdgeMesh.
        /// </summary>
        /// <param name="info">The SerializationInfo.</param>
        /// <param name="context">The StreamingContext.</param>
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Vertices", _vertices, typeof(List<Vertex>));
            info.AddValue("Triangles", _triangles, typeof(List<Triangle>));
            info.AddValue("HasNormals", _hasNormals, typeof(bool));
        }

        /// <summary>
        /// Add a Vertex to the HalfEdgeMesh <see cref="Vertices"/>.
        /// </summary>
        /// <param name="vertex">The Vertex to add.</param>
        public void AddVertex(Vertex vertex)
        {
            vertex.TriangleMesh = this;
            vertex.Index = _vertices.Count;
            _vertices.Add(vertex);
        }

        /// <summary>
        /// Add Vertices to the HalfEdgeMesh <see cref="Vertices"/>.
        /// </summary>
        /// <param name="vertices">The Vertices to add.</param>
        public void AddVertices(IEnumerable<Vertex> vertices)
        {
            foreach(var vertex in vertices)
                AddVertex(vertex);
        }

        /// <summary>
        /// Remove a Vertex from the HalfEdgeMesh <see cref="Vertices"/>.
        /// </summary>
        /// <param name="vertex">The Vertex to add.</param>
        public void RemoveVertex(Vertex vertex)
        {
            RemoveVertexFromHalfEdge(vertex);
        }

        /// <summary>
        /// Remove a Vertex from the HalfEdgeMesh <see cref="Vertices"/>.
        /// </summary>
        /// <param name="vertexIndex">The Vertex to add.</param>
        public void RemoveVertex(int vertexIndex)
        {
            var vertex = _vertices[vertexIndex];
            RemoveVertexFromHalfEdge(vertex);
        }

        /// <summary>
        /// Remove Vertices from the HalfEdgeMesh <see cref="Vertices"/>.
        /// </summary>
        /// <param name="vertices">The Vertices to remove.</param>
        public void RemoveVertices(IEnumerable<Vertex> vertices)
        {
            foreach(var vertex in vertices)
                RemoveVertex(vertex);
        }

        /// <summary>
        /// Remove Vertices from the HalfEdgeMesh <see cref="Vertices"/>.
        /// </summary>
        /// <param name="vertexIndices">The Indices of the Vertices to remove.</param>
        public void RemoveVertices(IEnumerable<int> vertexIndices)
        {
            foreach(var vertexIndex in vertexIndices)
                RemoveVertex(vertexIndex);
        }

        /// <summary>
        /// Add a Triangle to the HalfEdgeMesh <see cref="Triangles"/>.
        /// </summary>
        /// <param name="triangle">The Triangle to add.</param>
        public void AddTriangle(Triangle triangle)
        {
            triangle.TriangleMesh = this;
            triangle.Index = _triangles.Count;
            _triangles.Add(triangle);

            AddTriangleToHalfEdge(triangle);
        }

        /// <summary>
        /// Add Triangles to the HalfEdgeMesh <see cref="Triangles"/>.
        /// </summary>
        /// <param name="triangles">The Triangles to add.</param>
        public void AddTriangles(IEnumerable<Triangle> triangles)
        {
            foreach(var triangle in triangles)
                AddTriangle(triangle);
        }

        /// <summary>
        /// Remove a Triangle from the HalfEdgeMesh <see cref="Triangles"/>.
        /// </summary>
        /// <param name="triangle">The Triangle to remove.</param>
        public void RemoveTriangle(Triangle triangle)
        {
            RemoveTrianlgeFromHalfEdge(triangle);
        }
        /// <summary>
        /// Remove a Triangle from the HalfEdgeMesh <see cref="Triangles"/>.
        /// </summary>
        /// <param name="triangleIndex">The Index of the Triangle to remove.</param>
        public void RemoveTriangle(int triangleIndex)
        {
            var triangle = _triangles[triangleIndex];
            RemoveTrianlgeFromHalfEdge(triangle);
        }

        /// <summary>
        /// Remove Triangles from the HalfEdgeMesh <see cref="Triangles"/>.
        /// </summary>
        /// <param name="triangles">The Triangles to remove.</param>
        public void RemoveTriangles(IEnumerable<Triangle> triangles)
        {
            foreach(var triangle in triangles)
                RemoveTriangle(triangle);
        }

        /// <summary>
        /// Remove Triangles from the HalfEdgeMesh <see cref="Triangles"/>.
        /// </summary>
        /// <param name="triangleIndices">The Triangles to remove.</param>
        public void RemoveTriangles(IEnumerable<int> triangleIndices)
        {
            foreach(var triangleIndex in triangleIndices)
                RemoveTriangle(triangleIndex);
        }

        /// <summary>
        /// Handle the HalfEdges of the HalfEdgeMesh when a Triangle is added to it.
        /// </summary>
        /// <param name="triangle">The Triangle that was added to the <see cref="Triangles"/> of the HalfEdgeMesh.</param>
        private void AddTriangleToHalfEdge(Triangle triangle)
        {
            var foundVertexIndicesWithHalfEdges = new List<int>();
            if(triangle.Vertex1.HalfEdge != null)
                foundVertexIndicesWithHalfEdges.Add(triangle.Vertex1.Index);
            if(triangle.Vertex2.HalfEdge != null)
                foundVertexIndicesWithHalfEdges.Add(triangle.Vertex2.Index);
            if(triangle.Vertex3.HalfEdge != null)
                foundVertexIndicesWithHalfEdges.Add(triangle.Vertex3.Index);
            var foundVerticesWithHalfEdgesCount = foundVertexIndicesWithHalfEdges.Count;

            var foundExistingHalfEdges = triangle.ExistingHalfEdgesBetweenVertices;
            var foundExistingHalfEdgesCount = foundExistingHalfEdges.Count;

            ///Create 6 HalfEdges
            if (foundVerticesWithHalfEdgesCount <= 1 || foundVerticesWithHalfEdgesCount == 2 && foundExistingHalfEdgesCount == 0)
            {
                var attachedVertexIndex = 0;
                if(foundVerticesWithHalfEdgesCount == 1)
                {
                    attachedVertexIndex = foundVertexIndicesWithHalfEdges.ElementAt(0);
                    triangle.SetFirstVertexIndex(attachedVertexIndex);
                }
                else if(foundVerticesWithHalfEdgesCount == 2)
                    triangle.SetFirstVertexIndex(triangle.VertexIndizes.First(p => !foundVertexIndicesWithHalfEdges.Contains(p)));

                ///Create HalfEdges and Add
                var halfEdgeCount = _halfEdges.Count;
                var halfEdge1 = new HalfEdge(this, halfEdgeCount, triangle.Vertex1.Index, triangle.Vertex2.Index, triangle.Index);
                _halfEdges.Add(halfEdge1);
                var halfEdge1Opposite = new HalfEdge(this, halfEdgeCount + 1, triangle.Vertex2.Index, triangle.Vertex1.Index, -1);
                _halfEdges.Add(halfEdge1Opposite);

                var halfEdge2 = new HalfEdge(this, halfEdgeCount + 2, triangle.Vertex2.Index, triangle.Vertex3.Index, triangle.Index);
                _halfEdges.Add(halfEdge2);
                var halfEdge2Opposite = new HalfEdge(this, halfEdgeCount + 3, triangle.Vertex3.Index, triangle.Vertex2.Index, -1);
                _halfEdges.Add(halfEdge2Opposite);

                var halfEdge3 = new HalfEdge(this, halfEdgeCount + 4, triangle.Vertex3.Index, triangle.Vertex1.Index, triangle.Index);
                _halfEdges.Add(halfEdge3);
                var halfEdge3Opposite = new HalfEdge(this, halfEdgeCount + 5, triangle.Vertex1.Index, triangle.Vertex3.Index, -1);
                _halfEdges.Add(halfEdge3Opposite);

                HalfEdge incidentBorderHalfEdge1 = null;
                HalfEdge incidentBorderHalfEdge2 = null;
                HalfEdge adjacentBorderHalfEdge1 = null;
                HalfEdge adjacentBorderHalfEdge2 = null;
                if(foundVerticesWithHalfEdgesCount == 1)
                {
                    ///Correct for the already attached Vertex
                    incidentBorderHalfEdge1 = _vertices[attachedVertexIndex].HalfEdges.First(he => he.Triangle == null);
                    adjacentBorderHalfEdge1 = incidentBorderHalfEdge1.PreviousHalfEdge;
                    adjacentBorderHalfEdge1.NextHalfEdgeIndex = halfEdgeCount + 5;
                    incidentBorderHalfEdge1.PreviousHalfEdgeIndex = halfEdgeCount + 1;
                }
                else if (foundVerticesWithHalfEdgesCount == 2)
                {
                    ///Correct for the already attached Vertex
                    incidentBorderHalfEdge1 = _vertices[triangle.Vertex2.Index].HalfEdges.First(he => he.Triangle == null);
                    incidentBorderHalfEdge2 = _vertices[triangle.Vertex3.Index].HalfEdges.First(he => he.Triangle == null);
                    adjacentBorderHalfEdge1 = incidentBorderHalfEdge1.PreviousHalfEdge;
                    adjacentBorderHalfEdge2 = incidentBorderHalfEdge2.PreviousHalfEdge;
                    adjacentBorderHalfEdge1.NextHalfEdgeIndex = halfEdgeCount + 1;
                    adjacentBorderHalfEdge2.NextHalfEdgeIndex = halfEdgeCount + 3;
                    incidentBorderHalfEdge1.PreviousHalfEdgeIndex = halfEdgeCount + 3;
                    incidentBorderHalfEdge2.PreviousHalfEdgeIndex = halfEdgeCount + 5;
                }

                ///Set HalfEdge Properties
                halfEdge1.NextHalfEdgeIndex = halfEdgeCount + 2;
                halfEdge1.OppositeHalfEdgeIndex = halfEdgeCount + 1;
                halfEdge1.PreviousHalfEdgeIndex = halfEdgeCount + 4;

                halfEdge1Opposite.NextHalfEdgeIndex = halfEdgeCount + 5;
                halfEdge1Opposite.OppositeHalfEdgeIndex = halfEdgeCount;
                halfEdge1Opposite.PreviousHalfEdgeIndex = halfEdgeCount + 3;

                halfEdge2.NextHalfEdgeIndex = halfEdgeCount + 4;
                halfEdge2.OppositeHalfEdgeIndex = halfEdgeCount + 3;
                halfEdge2.PreviousHalfEdgeIndex = halfEdgeCount;

                halfEdge2Opposite.NextHalfEdgeIndex = halfEdgeCount + 1;
                halfEdge2Opposite.OppositeHalfEdgeIndex = halfEdgeCount + 2;
                halfEdge2Opposite.PreviousHalfEdgeIndex = halfEdgeCount + 5;

                halfEdge3.NextHalfEdgeIndex = halfEdgeCount;
                halfEdge3.OppositeHalfEdgeIndex = halfEdgeCount + 5;
                halfEdge3.PreviousHalfEdgeIndex = halfEdgeCount + 2;

                halfEdge3Opposite.NextHalfEdgeIndex = halfEdgeCount + 3;
                halfEdge3Opposite.OppositeHalfEdgeIndex = halfEdgeCount + 4;
                halfEdge3Opposite.PreviousHalfEdgeIndex = halfEdgeCount + 1;

                if(foundVerticesWithHalfEdgesCount == 1)
                {
                    ///Set HalfEdge Properties
                    halfEdge1Opposite.NextHalfEdgeIndex = incidentBorderHalfEdge1.Index;

                    halfEdge3Opposite.PreviousHalfEdgeIndex = adjacentBorderHalfEdge1.Index;
                }
                else if(foundVerticesWithHalfEdgesCount == 2)
                {
                    ///Set HalfEdge Properties
                    halfEdge1Opposite.PreviousHalfEdgeIndex = adjacentBorderHalfEdge1.Index;

                    halfEdge2Opposite.NextHalfEdgeIndex = incidentBorderHalfEdge1.Index;
                    halfEdge2Opposite.PreviousHalfEdgeIndex = adjacentBorderHalfEdge2.Index;

                    halfEdge3Opposite.NextHalfEdgeIndex = incidentBorderHalfEdge2.Index;
                }

                ///Set Vertex Properties
                if(foundVerticesWithHalfEdgesCount != 1)
                    _vertices[triangle.Vertex1.Index].HalfEdgeIndex = halfEdgeCount;
                if(foundVerticesWithHalfEdgesCount <= 1)
                {
                    _vertices[triangle.Vertex2.Index].HalfEdgeIndex = halfEdgeCount + 2;
                    _vertices[triangle.Vertex3.Index].HalfEdgeIndex = halfEdgeCount + 4;
                }

                ///Set Face Properties
                triangle.HalfEdgeIndex = halfEdgeCount;
            }
            ///Create 4 HalfEdges
            else if(foundExistingHalfEdgesCount == 1)
            {
                triangle.SetFirstVertexIndex(triangle.VertexIndizes.First(p => !foundExistingHalfEdges.ElementAt(0).VertexIndizes.Contains(p)));

                ///Create HalfEdges and Add
                var halfEdgeCount = _halfEdges.Count;
                var halfEdge1 = new HalfEdge(this, halfEdgeCount, triangle.Vertex1.Index, triangle.Vertex2.Index, triangle.Index);
                _halfEdges.Add(halfEdge1);
                var halfEdge1Opposite = new HalfEdge(this, halfEdgeCount + 1, triangle.Vertex2.Index, triangle.Vertex1.Index, -1);
                _halfEdges.Add(halfEdge1Opposite);

                var halfEdge2 = foundExistingHalfEdges.ElementAt(0);

                var halfEdge3 = new HalfEdge(this, halfEdgeCount + 2, triangle.Vertex3.Index, triangle.Vertex1.Index, triangle.Index);
                _halfEdges.Add(halfEdge3);
                var halfEdge3Opposite = new HalfEdge(this, halfEdgeCount + 3, triangle.Vertex1.Index, triangle.Vertex3.Index, -1);
                _halfEdges.Add(halfEdge3Opposite);

                HalfEdge incidentBorderHalfEdge0 = null;
                HalfEdge adjacentBorderHalfEdge0 = null;
                HalfEdge incidentBorderHalfEdge2 = null;
                HalfEdge adjacentBorderHalfEdge1 = null;
                if(foundVerticesWithHalfEdgesCount == 2)
                {
                    ///Correct for the already attached Vertex
                    incidentBorderHalfEdge2 = halfEdge2.NextHalfEdge;
                    adjacentBorderHalfEdge1 = halfEdge2.PreviousHalfEdge;
                    adjacentBorderHalfEdge1.NextHalfEdgeIndex = halfEdgeCount + 1;
                    incidentBorderHalfEdge2.PreviousHalfEdgeIndex = halfEdgeCount + 3;
                }
                else
                {
                    ///Correct for the already attached Vertex
                    incidentBorderHalfEdge0 = _vertices[triangle.Vertex1.Index].HalfEdges.First(he => he.Triangle == null);
                    adjacentBorderHalfEdge0 = incidentBorderHalfEdge0.PreviousHalfEdge;
                    incidentBorderHalfEdge2 = halfEdge2.NextHalfEdge;
                    adjacentBorderHalfEdge1 = halfEdge2.PreviousHalfEdge;
                    incidentBorderHalfEdge0.PreviousHalfEdgeIndex = halfEdgeCount + 1;
                    adjacentBorderHalfEdge0.NextHalfEdgeIndex = halfEdgeCount + 3;
                    adjacentBorderHalfEdge1.NextHalfEdgeIndex = halfEdgeCount + 1;
                    incidentBorderHalfEdge2.PreviousHalfEdgeIndex = halfEdgeCount + 3;
                }

                ///Set HalfEdge Properties
                halfEdge1.NextHalfEdgeIndex = halfEdge2.Index;
                halfEdge1.OppositeHalfEdgeIndex = halfEdgeCount + 1;
                halfEdge1.PreviousHalfEdgeIndex = halfEdgeCount + 2;

                halfEdge1Opposite.NextHalfEdgeIndex = halfEdgeCount + 3;
                halfEdge1Opposite.OppositeHalfEdgeIndex = halfEdgeCount;
                halfEdge1Opposite.PreviousHalfEdgeIndex = adjacentBorderHalfEdge1.Index;

                halfEdge2.NextHalfEdgeIndex = halfEdgeCount + 2;
                halfEdge2.PreviousHalfEdgeIndex = halfEdgeCount;

                halfEdge3.NextHalfEdgeIndex = halfEdgeCount;
                halfEdge3.OppositeHalfEdgeIndex = halfEdgeCount + 3;
                halfEdge3.PreviousHalfEdgeIndex = halfEdge2.Index;

                halfEdge3Opposite.NextHalfEdgeIndex = incidentBorderHalfEdge2.Index;
                halfEdge3Opposite.OppositeHalfEdgeIndex = halfEdgeCount + 2;
                halfEdge3Opposite.PreviousHalfEdgeIndex = halfEdgeCount + 1;

                if(foundVerticesWithHalfEdgesCount == 3)
                {
                    halfEdge1Opposite.NextHalfEdgeIndex = incidentBorderHalfEdge0.Index;
                    halfEdge3Opposite.PreviousHalfEdgeIndex = adjacentBorderHalfEdge0.Index;
                }

                if(foundVerticesWithHalfEdgesCount == 2)
                    _vertices[triangle.Vertex1.Index].HalfEdgeIndex = halfEdgeCount;

                ///Set HalfEdge Properties
                halfEdge2.TriangleIndex = triangle.Index;

                ///Set Face Properties
                triangle.HalfEdgeIndex = halfEdgeCount;
            }
            ///Create 2 HalfEdges
            else if(foundExistingHalfEdgesCount == 2)
            {
                    triangle.SetFirstVertexIndex(foundVertexIndicesWithHalfEdges.First(p => foundExistingHalfEdges.All(f => f.VertexIndizes.Contains(p))));

                    ///Create HalfEdges and Add
                    var halfEdgeCount = _halfEdges.Count;
                    var halfEdge1 = foundExistingHalfEdges.First(he => he.StartVertex.Index == triangle.Vertex1.Index);

                    var halfEdge2 = new HalfEdge(this, halfEdgeCount, triangle.Vertex2.Index, triangle.Vertex3.Index, triangle.Index);
                    _halfEdges.Add(halfEdge2);
                    var halfEdge2Opposite = new HalfEdge(this, halfEdgeCount + 1, triangle.Vertex3.Index, triangle.Vertex2.Index, -1);
                    _halfEdges.Add(halfEdge2Opposite);

                    var halfEdge3 = halfEdge1.PreviousHalfEdge;

                    ///Correct for the already attached Vertex
                    var incidentBorderHalfEdge1 = halfEdge1.NextHalfEdge;
                    var adjacentBorderHalfEdge2 = halfEdge3.PreviousHalfEdge;
                    incidentBorderHalfEdge1.PreviousHalfEdgeIndex = halfEdgeCount + 1;
                    adjacentBorderHalfEdge2.NextHalfEdgeIndex = halfEdgeCount + 1;

                    ///Set HalfEdge Properties
                    halfEdge1.NextHalfEdgeIndex = halfEdgeCount;
                    halfEdge1.PreviousHalfEdgeIndex = halfEdge3.Index;

                    halfEdge2.NextHalfEdgeIndex = halfEdge3.Index;
                    halfEdge2.OppositeHalfEdgeIndex = halfEdgeCount + 1;
                    halfEdge2.PreviousHalfEdgeIndex = halfEdge1.Index;

                    halfEdge2Opposite.NextHalfEdgeIndex = incidentBorderHalfEdge1.Index;
                    halfEdge2Opposite.OppositeHalfEdgeIndex = halfEdgeCount;
                    halfEdge2Opposite.PreviousHalfEdgeIndex = adjacentBorderHalfEdge2.Index;

                    halfEdge3.NextHalfEdgeIndex = halfEdge1.Index;
                    halfEdge3.PreviousHalfEdgeIndex = halfEdgeCount;

                    ///Set HalfEdge Properties
                    halfEdge1.TriangleIndex = triangle.Index;
                    halfEdge3.TriangleIndex = triangle.Index;

                    ///Set Face Properties
                    triangle.HalfEdgeIndex = halfEdgeCount;
            }
            ///Create no new HalfEdges
            else
            {
                ///Set HalfEdge Properties
                var halfEdge1 = foundExistingHalfEdges.First(he => he.Triangle == null
                    && he.StartVertex.Index == triangle.Vertex1.Index);
                var halfEdge2 = halfEdge1.NextHalfEdge;
                var halfEdge3 = halfEdge2.NextHalfEdge;

                ///Set HalfEdge Properties
                halfEdge1.TriangleIndex = triangle.Index;
                halfEdge2.TriangleIndex = triangle.Index;
                halfEdge3.TriangleIndex = triangle.Index;

                ///Set Face Properties
                triangle.HalfEdgeIndex = halfEdge1.Index;
            }
        }

        /// <summary>
        /// Handle the HalfEdges of the HalfEdgeMesh when a Vertex is removed from it.
        /// </summary>
        /// <param name="vertex">The Vertex that is removed from the <see cref="Vertices"/> of the HalfEdgeMesh.</param>
        private void RemoveVertexFromHalfEdge(Vertex vertex)
        {
            if(vertex.TriangleMesh == null)
                return;

            var triangles = vertex.Triangles.Where(t => t != null).ToList();
            foreach(var triangle in triangles)
                RemoveTrianlgeFromHalfEdge(triangle);
        }

        /// <summary>
        /// Handle the HalfEdges of the HalfEdgeMesh when a Triangle is removed from it.
        /// </summary>
        /// <param name="triangle">The Triangle that was removed from the <see cref="Triangles"/> of the HalfEdgeMesh.</param>
        private void RemoveTrianlgeFromHalfEdge(Triangle triangle)
        {
            if(triangle.TriangleMesh == null)
                return;

            var borderHalfEdges = triangle.HalfEdges.Where(he => he.IsOnBorder);

            if(borderHalfEdges.Count() == 1)
                triangle.SetFirstVertexIndex(borderHalfEdges.ElementAt(0).StartVertex.Index);
            else if(borderHalfEdges.Count() == 2)
            {
                triangle.SetFirstVertexIndex(borderHalfEdges.ElementAt(0).StartVertex.Index);

                if(borderHalfEdges.ElementAt(1).Index == borderHalfEdges.ElementAt(0).PreviousHalfEdge.Index)
                    triangle.SetFirstVertexIndex(borderHalfEdges.ElementAt(1).StartVertex.Index);
            }

            var vertices = triangle.Vertices.ToList();
            var halfEdges = triangle.HalfEdges.ToList();
            var triangles = triangle.Triangles.Where(t => t != null).ToList();

            foreach(var vertex in vertices)
            {
                if(halfEdges.Contains(vertex.HalfEdge))
                {
                    var nextHe = vertex.Triangles.Where(t => t != null).Count() == 1 ?
                        -1 : vertex.HalfEdges.First(vHe => !halfEdges.Contains(vHe) && vHe.Triangle != null).Index;
                    vertex.HalfEdgeIndex = nextHe;

                    if (nextHe == -1)
                    {
                        vertex.Index = -1;
                        vertex.TriangleMesh = null;
                    }
                }
            }

            triangle.VertexIndizes = new int[] { -1, -1, -1 };
            triangle.HalfEdgeIndex = -1;
            triangle.Index = -1;
            triangle.TriangleMesh = null;

            foreach(var halfEdge in halfEdges)
            {
                if(halfEdge.OppositeHalfEdge.Triangle == null)
                {
                    halfEdge.OppositeHalfEdge.VertexIndizes = new int[] { -1, -1 };
                    halfEdge.OppositeHalfEdge.PreviousHalfEdge.NextHalfEdgeIndex = halfEdge.NextHalfEdge.Index;
                    halfEdge.NextHalfEdge.PreviousHalfEdgeIndex = halfEdge.OppositeHalfEdge.PreviousHalfEdge.Index;

                    halfEdge.OppositeHalfEdge.NextHalfEdge.PreviousHalfEdgeIndex = halfEdge.PreviousHalfEdge.Index;
                    halfEdge.PreviousHalfEdge.NextHalfEdgeIndex = halfEdge.OppositeHalfEdge.NextHalfEdge.Index;

                    halfEdge.TriangleIndex = -1;
                    halfEdge.VertexIndizes = new int[] { -1, -1 };
                    halfEdge.Index = -1;
                    halfEdge.TriangleMesh = null;
                }
                else
                    halfEdge.TriangleIndex = -1;
            }
        }

        /// <summary>
        /// Calculate the Vertex Normals for the HalfEdgeMesh.
        /// </summary>
        public void CalculateVertexNormals()
        {
            foreach(var vertex in _vertices)
            {
                var allNeighboringTriangles = vertex.Triangles;
                var neighboringTriangles = new List<Triangle>();
                var existingNormals = new HashSet<Vector>();

                foreach(var neighbor in allNeighboringTriangles)
                {
                    if(!existingNormals.Contains(neighbor.Normal))
                    {
                        neighboringTriangles.Add(neighbor);
                        existingNormals.Add(neighbor.Normal);
                    }
                }

                vertex.Normal = Vector.Zero;
                var inverseSumArea = 1f / neighboringTriangles.Sum(t => t.Area);
                foreach(var triangle in neighboringTriangles)
                    vertex.Normal += triangle.Normal * triangle.Area * inverseSumArea;

                vertex.Normal.Normalize();
            }

            _hasNormals = true;
        }

        /// <summary>
        /// Optimizes the Mesh by eliminating close Points.
        /// </summary>
        /// <param name="eps">The maximum Distance of the Points that should be merged into one Point.</param>
        /// <returns>Optimized HalfEdgeMesh.</returns>
        public HalfEdgeMesh Optimize(float eps = Epsilon)
        {
            ///TODO implement
            return this;
        }

        /// <summary>
        /// Creates a Convex Hull for the HalfEdgeMesh.
        /// </summary>
        /// <returns>The Convex Hull of the HalfEdgeMesh.</returns>
        public HalfEdgeMesh CreateConvexHull()
        {
            ///TODO implement
            return this;
        }

        /// <summary>
        /// Subdivide the HalfEdgeMesh.
        /// </summary>
        /// <param name="type">The Subdivision Algorithm Type.</param>
        /// <param name="steps">How many Subdivision Steps should be performed.</param>
        /// <returns>New subdivided HalfEdgeMesh.</returns>
        public HalfEdgeMesh Subdivide(SubdivisionType type = SubdivisionType.Default, int steps = 1)
        {
            ///TODO implement
            return this;
        }

        /// <summary>
        /// Calculates the Silhouette of the HalfEdgeMesh for a given Position.
        /// </summary>
        /// <param name="position">The Position to calculate the Silhouette for.</param>
        /// <returns>List of HalfEdge Lists because a Mesh can have more than one Silhouette.</returns>
        public List<List<HalfEdge>> CalculateSilhouette(Vector position)
        {
            var result = new List<List<HalfEdge>>();

            var halfEdges = _halfEdges.Where(h => !h.IsOnBorder && Math.Sign(Vector.Dot(position - h.StartVertex.Position, h.Triangles[0].Normal)) !=
                Math.Sign(Vector.Dot(position - h.StartVertex.Position, h.Triangles[1].Normal)));


            return halfEdges.Select(h => new List<HalfEdge> { h }).ToList();
        }

        /// <summary>
        /// Get all Triangles that are intersecting the provided Plane.
        /// </summary>
        /// <param name="plane">The Intersection Plane.</param>
        /// <returns>List of <see cref="Triangle"/>that all intersect the Plane.</returns>
        public List<Triangle> IntersectingTriangles(Plane plane)
        {
            ///TODO implement
            return null;
        }

        /// <summary>
        /// Cut the HalfEdgeMesh along the Plane and calculate the individual cut Parts of the HalfEdgeMesh.
        /// </summary>
        /// <param name="plane">The Plane to cut the HalfEdgeMesh.</param>
        /// <param name="closed">Indicates if the resulting HalfEdgeMeshes should be closed at the cutting Plane or not (default = false)</param>
        /// <returns></returns>
        public List<HalfEdgeMesh> Cut(Plane plane, bool closed = false)
        {
            ///TODO implement
            return null;
        }

        /// <summary>
        /// Merge the HalfEdgeMesh with a number of other HalfEdgeMeshes and return the merged Mesh.
        /// </summary>
        /// <param name="meshes">The Meshes that should be merges into this HalfEdgeMesh.</param>
        /// <param name="optimize">Indicates the need to also optimize the merged HalfEdgeMesh, i.e. merge Vertices etc. (default = false).</param>
        /// <returns>Merged HalfEdgeMesh.</returns>
        public HalfEdgeMesh MergeWith(IEnumerable<HalfEdgeMesh> meshes, bool optimize = false)
        {
            ///TODO implement
            return null;
        }

        /// <summary>
        /// Smoothes all Edges, that are sharper than the provided <paramref name="maxAngle"/> Value.
        /// </summary>
        /// <param name="maxAngle">The maximum Angle that may exist in the smoothed HalfEdgeMesh (default = 90°).</param>
        /// <returns>Smoothed HalfEdgeMesh with Edge Angles <= <paramref name="maxAngle"/>.</returns>
        public HalfEdgeMesh SmoothEdges(float maxAngle = 90 * MathF.DegreeToRadians)
        {
            ///TODO implement
            return null;
        }

        /// <summary>
        /// Fractures (splits) the HalfedgeMesh into many smaller Parts.
        /// </summary>
        /// <param name="position">The Position of the Fracture Point. The Fractures near this Point are smaller than the one's farther away.</param>
        /// <param name="numParts">The Number of Parts that should be generated from the existing HalfEdgeMesh.</param>
        /// <param name="closed">Indicates if the resulting HalfEdgeMeshes should be closed or not (default = false)</param>
        /// <returns>List of HalfEdgeMesh Objects that are parts of the original HalfEdgeMesh.</returns>
        public List<HalfEdgeMesh> Fracture(Float3 position = default(Float3), int numParts = -1, bool closed = false)
        {
            ///TODO implement
            return null;
        }
    }
}