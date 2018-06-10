using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media.Media3D;
using HalfEdgeDataStructure;
using HelixToolkit.Wpf;
using Triangle = HalfEdgeDataStructure.Triangle;

namespace Helper
{
    /// <summary>
    /// Enumeration for all Cube Sides.
    /// </summary>
    [Flags]
    public enum CubeSides
    {
        None = 0x000000,
        PositiveX = 0x100000,
        NegativeX = 0x010000,
        X = PositiveX | NegativeX,
        PositiveY = 0x001000,
        NegativeY = 0x000100,
        Y = PositiveY | NegativeY,
        PositiveZ = 0x000010,
        NegativeZ = 0x000001,
        Z = PositiveZ | NegativeZ,
        All = X | Y | Z
    }

    /// <summary>
    /// Helper Class to generate HalfEdgeMeshes.
    /// </summary>
    public static class HalfEdgeMeshGenerator
    {
        /// <summary>
        /// Generate a Test HalfEdgeMesh that was used to test the Creation and Modification of the HalfEdges.
        /// </summary>
        /// <returns>The generated HalfEdgeMesh.</returns>
        public static HalfEdgeMesh GenerateHalfEdgeTests()
        {
            var points = new List<Vertex> {
                new Vertex(0, 0, 0), new Vertex(2, 0, 0), new Vertex(1, 1, 0),
                new Vertex(4, 0, 0), new Vertex(3, 1, 0),
                new Vertex(2, 2, 0),
                new Vertex(4, 2, 0),
                new Vertex(6, 2, 0), new Vertex(5, 3, 0)

            };

            var triangles = new List<Triangle> {
                new Triangle(0, 1, 2),
                new Triangle(3, 4, 1),
                new Triangle(4, 5, 2),
                new Triangle(5, 4, 6),
                new Triangle(8, 6, 7),
                new Triangle(4, 3, 7),
                new Triangle(5, 6, 8),
                new Triangle(4, 2, 1),
                new Triangle(4, 7, 6)
            };

            return new HalfEdgeMesh(points, triangles);
        }

        /// <summary>
        /// Generate a HalfEdgeMesh of a Cube.
        /// </summary>
        /// <param name="origin">The Origin of the Cube.</param>
        /// <param name="size">The Size of the Cube.</param>
        /// <param name="sides">Defines which Sides of the Cube should be generated (default = All Sides).</param>
        /// <returns>The generated HalfEdgeMesh.</returns>
        public static HalfEdgeMesh GenerateCube(Vector origin, double size = 1, CubeSides sides = CubeSides.All)
        {
            var points = new List<Vertex>()
            {
                new Vertex(0, 0, 0),
                new Vertex(1, 0 ,0),
                new Vertex(1, 1 ,0),
                new Vertex(0, 1 ,0),
                new Vertex(0, 0, 1),
                new Vertex(1, 0 ,1),
                new Vertex(1, 1 ,1),
                new Vertex(0, 1 ,1),
            };
            points = points.Select(p => new Vertex(new Vector(p) * size + origin)).ToList();

            var triangles = new List<Triangle>();
            if(sides.HasFlag(CubeSides.NegativeZ))
            {
                triangles.Add(new Triangle(0, 2, 1));
                triangles.Add(new Triangle(0, 3, 2));
            }
            if(sides.HasFlag(CubeSides.PositiveZ))
            {
                triangles.Add(new Triangle(4, 5, 6));
                triangles.Add(new Triangle(4, 6, 7));
            }
            if(sides.HasFlag(CubeSides.NegativeY))
            {
                triangles.Add(new Triangle(0, 1, 5));
                triangles.Add(new Triangle(0, 5, 4));
            }
            if(sides.HasFlag(CubeSides.PositiveX))
            {
                triangles.Add(new Triangle(1, 2, 6));
                triangles.Add(new Triangle(1, 6, 5));
            }
            if(sides.HasFlag(CubeSides.PositiveY))
            {
                triangles.Add(new Triangle(2, 3, 7));
                triangles.Add(new Triangle(2, 7, 6));
            }
            if(sides.HasFlag(CubeSides.NegativeX))
            {
                triangles.Add(new Triangle(3, 0, 4));
                triangles.Add(new Triangle(3, 4, 7));
            }

            return new HalfEdgeMesh(points, triangles);
        }

        /// <summary>
        /// Generate a HalfEdgeMesh of a Sphere.
        /// </summary>
        /// <param name="center">The Center of the Sphere.</param>
        /// <param name="radius">The Radius of the Sphere.</param>
        /// <param name="numSides">Number of Sides of the Sphere (multiply by 4 for one Ring).</param>
        /// <param name="sphereSides">Which Sides of the Sphere should be generated.</param>
        /// <returns>The generated HalfEdgeMesh.</returns>
        public static HalfEdgeMesh GenerateSphere(Vector center, double radius = 1, int numSides = 8, CubeSides sphereSides = CubeSides.All)
        {
            var shiftVector = Vector.One() * 0.5;
            var mesh = new HalfEdgeMesh();
            var sideLength = 1.0 / numSides;
            var pointPerSide = numSides + 1;
            var vertices = new List<Vertex>();

            var bottomPoints = new List<Vertex>(pointPerSide * pointPerSide);
            var topPoints = new List<Vertex>(pointPerSide * pointPerSide);
            var frontPoints = new List<Vertex>(pointPerSide * pointPerSide);
            var backPoints = new List<Vertex>(pointPerSide * pointPerSide);
            var leftPoints = new List<Vertex>(pointPerSide * pointPerSide);
            var rightPoints = new List<Vertex>(pointPerSide * pointPerSide);
            var idx = 0;
            for(int z = 0; z <= numSides; z++)
            {
                for(int y = 0; y <= numSides; y++)
                {
                    for(int x = 0; x <= numSides; x++)
                    {
                        if(z == 0 || z == numSides || x == 0 || x == numSides || y == 0 || y == numSides)
                        {
                            var vertex = new Vertex(x * sideLength - 0.5, y * sideLength - 0.5, z * sideLength - 0.5)
                            {
                                Index = idx,
                            };
                            if(x == 0 && sphereSides.HasFlag(CubeSides.NegativeZ))
                                bottomPoints.Add(vertex);
                            else if(x == numSides && sphereSides.HasFlag(CubeSides.PositiveZ))
                                topPoints.Add(vertex);

                            if(y == 0 && sphereSides.HasFlag(CubeSides.NegativeY))
                                frontPoints.Add(vertex);
                            else if(y == numSides && sphereSides.HasFlag(CubeSides.PositiveY))
                                backPoints.Add(vertex);

                            if(z == 0 && sphereSides.HasFlag(CubeSides.NegativeX))
                                leftPoints.Add(vertex);
                            else if(z == numSides && sphereSides.HasFlag(CubeSides.PositiveX))
                                rightPoints.Add(vertex);

                            vertices.Add(vertex);
                            idx++;
                        }
                        else
                            x += numSides - 2;
                    }
                }
            }

            foreach(var vertex in vertices)
            {
                var vertexVector = (Vector)vertex;
                vertexVector.Normalize();
                vertex.Position = vertexVector * radius + center;
            }
            mesh.AddPoints(vertices);

            var triangles = new List<Triangle>();
            for(int i = 0; i < numSides * numSides; i++)
            {
                idx = i + i / numSides;
                var nextIdx = idx + 1;
                var nextRowIdx = nextIdx + numSides;
                var nextRowNextIdx = nextRowIdx + 1;
                if(sphereSides.HasFlag(CubeSides.NegativeZ))
                {
                    mesh.AddTriangle(new Triangle(bottomPoints[idx].Index, bottomPoints[nextRowNextIdx].Index, bottomPoints[nextIdx].Index));
                    mesh.AddTriangle(new Triangle(bottomPoints[idx].Index, bottomPoints[nextRowIdx].Index, bottomPoints[nextRowNextIdx].Index));
                }

                if(sphereSides.HasFlag(CubeSides.PositiveZ))
                {
                    mesh.AddTriangle(new Triangle(topPoints[idx].Index, topPoints[nextIdx].Index, topPoints[nextRowNextIdx].Index));
                    mesh.AddTriangle(new Triangle(topPoints[idx].Index, topPoints[nextRowNextIdx].Index, topPoints[nextRowIdx].Index));
                }

                if(sphereSides.HasFlag(CubeSides.NegativeY))
                {
                    mesh.AddTriangle(new Triangle(frontPoints[idx].Index, frontPoints[nextIdx].Index, frontPoints[nextRowNextIdx].Index));
                    mesh.AddTriangle(new Triangle(frontPoints[idx].Index, frontPoints[nextRowNextIdx].Index, frontPoints[nextRowIdx].Index));
                }

                if(sphereSides.HasFlag(CubeSides.PositiveY))
                {
                    mesh.AddTriangle(new Triangle(backPoints[idx].Index, backPoints[nextRowNextIdx].Index, backPoints[nextIdx].Index));
                    mesh.AddTriangle(new Triangle(backPoints[idx].Index, backPoints[nextRowIdx].Index, backPoints[nextRowNextIdx].Index));
                }

                if(sphereSides.HasFlag(CubeSides.NegativeX))
                {
                    mesh.AddTriangle(new Triangle(leftPoints[idx].Index, leftPoints[nextRowNextIdx].Index, leftPoints[nextIdx].Index));
                    mesh.AddTriangle(new Triangle(leftPoints[idx].Index, leftPoints[nextRowIdx].Index, leftPoints[nextRowNextIdx].Index));
                }

                if(sphereSides.HasFlag(CubeSides.PositiveX))
                {
                    mesh.AddTriangle(new Triangle(rightPoints[idx].Index, rightPoints[nextIdx].Index, rightPoints[nextRowNextIdx].Index));
                    mesh.AddTriangle(new Triangle(rightPoints[idx].Index, rightPoints[nextRowNextIdx].Index, rightPoints[nextRowIdx].Index));
                }
            }

            return mesh;
        }

        /// <summary>
        /// Generate a HalfEdgeMesh from an existing MeshGeometry3D Object.
        /// </summary>
        /// <param name="meshGeometry">The existing MeshGeometry3D Object.</param>
        /// <returns>The generated HalfEdgeMesh.</returns>
        public static HalfEdgeMesh GenerateFromMeshGeometry3D(MeshGeometry3D meshGeometry)
        {
            var points = meshGeometry.Positions.Select(p => new Vertex(p.X, p.Y, p.Z)).ToList();
            var triangles = new List<Triangle>();
            for(int i = 0; i < meshGeometry.TriangleIndices.Count; i += 3)
                triangles.Add(new Triangle(meshGeometry.TriangleIndices.ElementAt(i),
                    meshGeometry.TriangleIndices.ElementAt(i + 1),
                    meshGeometry.TriangleIndices.ElementAt(i + 2)));

            return new HalfEdgeMesh(points, triangles);
        }
    }
}