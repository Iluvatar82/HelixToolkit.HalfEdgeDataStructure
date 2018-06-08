using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace HalfEdgeDataStructure
{
    [Serializable]
    public class HalfEdgeMesh : ICloneable, ISerializable
    {
        private List<Vertex> _vertices;
        private List<Triangle> _triangles;
        private List<HalfEdge> _halfEdges;


        public List<Vertex> Vertices {
            get { return _vertices; }
            set { _vertices = value; }
        }

        public List<Triangle> Triangles {
            get { return _triangles; }
            set { _triangles = value; }
        }

        public List<HalfEdge> HalfEdges {
            get { return _halfEdges; }
            set { _halfEdges = value; }
        }

        public IEnumerable<Vertex> BoundaryPoints {
            get
            {
                var foundBoundaryHalfEdge = new HashSet<HalfEdge>();
                HalfEdge currentBoundaryHalfEdge = null;
                do
                {
                    currentBoundaryHalfEdge = _halfEdges.FirstOrDefault(he => he.Triangle == null && he.IsOnBorder &&
                        !foundBoundaryHalfEdge.Contains(he));

                    if(currentBoundaryHalfEdge == default(HalfEdge))
                        break;

                    while(!foundBoundaryHalfEdge.Contains(currentBoundaryHalfEdge))
                    {
                        yield return currentBoundaryHalfEdge.StartPoint;
                        yield return currentBoundaryHalfEdge.EndPoint;

                        foundBoundaryHalfEdge.Add(currentBoundaryHalfEdge);
                        if (currentBoundaryHalfEdge.OppositeHalfEdge != default(HalfEdge))
                            foundBoundaryHalfEdge.Add(currentBoundaryHalfEdge.OppositeHalfEdge);
                        currentBoundaryHalfEdge = currentBoundaryHalfEdge.NextHalfEdge;
                    }
                }
                while(currentBoundaryHalfEdge != default(HalfEdge));
            }
        }


        public HalfEdgeMesh()
        {
            _vertices = new List<Vertex>();
            _triangles = new List<Triangle>();
            _halfEdges = new List<HalfEdge>();
        }

        public HalfEdgeMesh(List<Vertex> points, List<Triangle> triangles)
            :this()
        {
            AddPoints(points);
            AddTriangles(triangles);
        }

        public HalfEdgeMesh(HalfEdgeMesh mesh)
            :this(mesh.Vertices, mesh.Triangles)
        { }

        public HalfEdgeMesh(SerializationInfo info, StreamingContext context)
            :this()
        {
            AddPoints((List<Vertex>)info.GetValue("Points", typeof(List<Vertex>)));
            AddTriangles((List<Triangle>)info.GetValue("Triangles", typeof(List<Triangle>)));
        }


        public object Clone()
        {
            return new HalfEdgeMesh(this);
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Points", _vertices, typeof(List<Vertex>));
            info.AddValue("Triangles", _triangles, typeof(List<Triangle>));
        }

        public void AddPoint(Vertex point)
        {
            point.TriangleMesh = this;
            point.Index = _vertices.Count;
            _vertices.Add(point);
        }

        public void AddPoints(IEnumerable<Vertex> points)
        {
            foreach(var point in points)
                AddPoint(point);
        }

        public void AddTriangle(Triangle triangle)
        {
            triangle.TriangleMesh = this;
            triangle.Index = _triangles.Count;
            _triangles.Add(triangle);

            HandleHalfEdges(triangle);
        }

        public void AddTriangles(IEnumerable<Triangle> triangles)
        {
            foreach(var triangle in triangles)
                AddTriangle(triangle);
        }

        private void HandleHalfEdges(Triangle triangle)
        {
            var foundPointIndicesWithHalfEdges = new List<int>(triangle.PointIndizes
                .Select(p => _vertices[p].OutgoingHalfEdge).Where(he => he != null).Select(he => he.StartPoint.Index));
            var foundExistingHalfEdges = triangle.ExistingHalfEdgesBetweenVertices.ToList();

            ///Create 6 HalfEdges
            if (foundPointIndicesWithHalfEdges.Count <= 1 || foundPointIndicesWithHalfEdges.Count == 2 && foundExistingHalfEdges.Count == 0)
            {
                var attachedPointIndex = 0;
                if(foundPointIndicesWithHalfEdges.Count == 1)
                {
                    attachedPointIndex = foundPointIndicesWithHalfEdges[0];
                    triangle.SetFirstPointIndex(attachedPointIndex);
                }
                else if(foundPointIndicesWithHalfEdges.Count == 2)
                    triangle.SetFirstPointIndex(triangle.PointIndizes.First(p => !foundPointIndicesWithHalfEdges.Contains(p)));

                ///Create HalfEdges and Add
                var halfEdgeCount = _halfEdges.Count;
                var halfEdge1 = new HalfEdge(this, halfEdgeCount, triangle.Point1.Index, triangle.Index);
                _halfEdges.Add(halfEdge1);
                var halfEdge1Opposite = new HalfEdge(this, halfEdgeCount + 1, triangle.Point2.Index, -1);
                _halfEdges.Add(halfEdge1Opposite);

                var halfEdge2 = new HalfEdge(this, halfEdgeCount + 2, triangle.Point2.Index, triangle.Index);
                _halfEdges.Add(halfEdge2);
                var halfEdge2Opposite = new HalfEdge(this, halfEdgeCount + 3, triangle.Point3.Index, -1);
                _halfEdges.Add(halfEdge2Opposite);

                var halfEdge3 = new HalfEdge(this, halfEdgeCount + 4, triangle.Point3.Index, triangle.Index);
                _halfEdges.Add(halfEdge3);
                var halfEdge3Opposite = new HalfEdge(this, halfEdgeCount + 5, triangle.Point1.Index, -1);
                _halfEdges.Add(halfEdge3Opposite);

                HalfEdge incidentBorderHalfEdge1 = null;
                HalfEdge incidentBorderHalfEdge2 = null;
                HalfEdge adjacentBorderHalfEdge1 = null;
                HalfEdge adjacentBorderHalfEdge2 = null;
                if(foundPointIndicesWithHalfEdges.Count == 1)
                {
                    ///Correct for the already attached Vertex
                    incidentBorderHalfEdge1 = _vertices[attachedPointIndex].IncidentHalfEdges.First(he => he.Triangle == null && he.IsOnBorder);
                    adjacentBorderHalfEdge1 = incidentBorderHalfEdge1.PreviousHalfEdge;
                    adjacentBorderHalfEdge1.NextHalfEdgeIndex = halfEdgeCount + 5;
                    incidentBorderHalfEdge1.PreviousHalfEdgeIndex = halfEdgeCount + 1;
                }
                else if (foundPointIndicesWithHalfEdges.Count == 2)
                {
                    ///Correct for the already attached Vertex
                    incidentBorderHalfEdge1 = _vertices[triangle.Point2.Index].IncidentHalfEdges.First(he => he.Triangle == null && he.IsOnBorder);
                    incidentBorderHalfEdge2 = _vertices[triangle.Point3.Index].IncidentHalfEdges.First(he => he.Triangle == null && he.IsOnBorder);
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

                if(foundPointIndicesWithHalfEdges.Count == 1)
                {
                    ///Set HalfEdge Properties
                    halfEdge1Opposite.NextHalfEdgeIndex = incidentBorderHalfEdge1.Index;

                    halfEdge3Opposite.PreviousHalfEdgeIndex = adjacentBorderHalfEdge1.Index;
                }
                else if(foundPointIndicesWithHalfEdges.Count == 2)
                {
                    ///Set HalfEdge Properties
                    halfEdge1Opposite.PreviousHalfEdgeIndex = adjacentBorderHalfEdge1.Index;

                    halfEdge2Opposite.NextHalfEdgeIndex = incidentBorderHalfEdge1.Index;
                    halfEdge2Opposite.PreviousHalfEdgeIndex = adjacentBorderHalfEdge2.Index;

                    halfEdge3Opposite.NextHalfEdgeIndex = incidentBorderHalfEdge2.Index;
                }

                ///Set Vertex Properties
                if(foundPointIndicesWithHalfEdges.Count != 1)
                    _vertices[triangle.Point1.Index].HalfEdgeIndex = halfEdgeCount;
                if(foundPointIndicesWithHalfEdges.Count <= 1)
                {
                    _vertices[triangle.Point2.Index].HalfEdgeIndex = halfEdgeCount + 2;
                    _vertices[triangle.Point3.Index].HalfEdgeIndex = halfEdgeCount + 4;
                }

                ///Set Face Properties
                triangle.HalfEdgeIndex = halfEdgeCount;
            }
            ///Create 4 HalfEdges
            else if(foundExistingHalfEdges.Count == 1)
            {
                triangle.SetFirstPointIndex(triangle.PointIndizes.First(p => !foundExistingHalfEdges.ElementAt(0).PointIndizes.Contains(p)));

                ///Create HalfEdges and Add
                var halfEdgeCount = _halfEdges.Count;
                var halfEdge1 = new HalfEdge(this, halfEdgeCount, triangle.Point1.Index, triangle.Index);
                _halfEdges.Add(halfEdge1);
                var halfEdge1Opposite = new HalfEdge(this, halfEdgeCount + 1, triangle.Point2.Index, -1);
                _halfEdges.Add(halfEdge1Opposite);

                var halfEdge2 = foundExistingHalfEdges.ElementAt(0);

                var halfEdge3 = new HalfEdge(this, halfEdgeCount + 2, triangle.Point3.Index, triangle.Index);
                _halfEdges.Add(halfEdge3);
                var halfEdge3Opposite = new HalfEdge(this, halfEdgeCount + 3, triangle.Point1.Index, -1);
                _halfEdges.Add(halfEdge3Opposite);

                HalfEdge incidentBorderHalfEdge0 = null;
                HalfEdge adjacentBorderHalfEdge0 = null;
                HalfEdge incidentBorderHalfEdge2 = null;
                HalfEdge adjacentBorderHalfEdge1 = null;
                if(foundPointIndicesWithHalfEdges.Count == 2)
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
                    incidentBorderHalfEdge0 = _vertices[triangle.Point1.Index].IncidentHalfEdges.First(he => he.Triangle == null && he.IsOnBorder);
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

                if(foundPointIndicesWithHalfEdges.Count == 3)
                {
                    halfEdge1Opposite.NextHalfEdgeIndex = incidentBorderHalfEdge0.Index;
                    halfEdge3Opposite.PreviousHalfEdgeIndex = adjacentBorderHalfEdge0.Index;
                }

                if(foundPointIndicesWithHalfEdges.Count == 2)
                    _vertices[triangle.Point1.Index].HalfEdgeIndex = halfEdgeCount;

                ///Set HalfEdge Properties
                halfEdge2.TriangleIndex = triangle.Index;

                ///Set Face Properties
                triangle.HalfEdgeIndex = halfEdgeCount;
            }
            ///Create 2 HalfEdges
            else if(foundExistingHalfEdges.Count == 2)
            {
                    triangle.SetFirstPointIndex(foundPointIndicesWithHalfEdges.First(p => foundExistingHalfEdges.All(f => f.PointIndizes.Contains(p))));

                    ///Create HalfEdges and Add
                    var halfEdgeCount = _halfEdges.Count;
                    var halfEdge1 = foundExistingHalfEdges.First(he => he.StartPoint.Index == triangle.Point1.Index);

                    var halfEdge2 = new HalfEdge(this, halfEdgeCount, triangle.Point2.Index, triangle.Index);
                    _halfEdges.Add(halfEdge2);
                    var halfEdge2Opposite = new HalfEdge(this, halfEdgeCount + 1, triangle.Point3.Index, -1);
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
                var halfEdgeCount = _halfEdges.Count;
                var halfEdge1 = foundExistingHalfEdges.First(he => he.Triangle == null &&
                    he.IsOnBorder && he.StartPoint.Index == triangle.Point1.Index);
                var halfEdge2 = halfEdge1.NextHalfEdge;
                var halfEdge3 = halfEdge2.NextHalfEdge;

                ///Set HalfEdge Properties
                halfEdge1.TriangleIndex = triangle.Index;
                halfEdge2.TriangleIndex = triangle.Index;
                halfEdge3.TriangleIndex = triangle.Index;

                ///Set Face Properties
                triangle.HalfEdgeIndex = halfEdgeCount;
            }
        }
    }
}