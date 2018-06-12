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
            var vertices = new List<Vertex> {
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

            return new HalfEdgeMesh(vertices, triangles);
        }

        /// <summary>
        /// Generate a HalfEdgeMesh of a Cube.
        /// </summary>
        /// <param name="origin">The Origin of the Cube.</param>
        /// <param name="size">The Size of the Cube.</param>
        /// <param name="sides">Defines which Sides of the Cube should be generated (default = All Sides).</param>
        /// <param name="calculateNormals">Calculate Vertex Normals or not.</param>
        /// <returns>The generated HalfEdgeMesh.</returns>
        public static HalfEdgeMesh GenerateCube(Vector origin,
            float size = 1, CubeSides sides = CubeSides.All, bool calculateNormals = false)
        {
            var vertices = new List<Vertex>()
            {
                new Vertex(origin.X, origin.Y, origin.Z),
                new Vertex(1 * size + origin.X, origin.Y, origin.Z),
                new Vertex(1 * size + origin.X, 1 * size + origin.Y, origin.Z),
                new Vertex(origin.X, 1 * size + origin.Y , origin.Z),
                new Vertex(origin.X, origin.Y, 1 * size + origin.Z),
                new Vertex(1 * size + origin.X, origin.Y, 1 * size + origin.Z),
                new Vertex(1 * size + origin.X, 1 * size + origin.Y ,1 * size + origin.Z),
                new Vertex(origin.X, 1 * size + origin.Y ,1 * size + origin.Z),
            };

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

            var mesh = new HalfEdgeMesh(vertices, triangles);
            if(calculateNormals)
                mesh.CalculateVertexNormals();

            return mesh;
        }

        /// <summary>
        /// Generate a HalfEdgeMesh of a Sphere.
        /// </summary>
        /// <param name="center">The Center of the Sphere.</param>
        /// <param name="radius">The Radius of the Sphere.</param>
        /// <param name="numSides">Number of Sides of the Sphere (multiply by 4 for one Ring).</param>
        /// <param name="sides">Which Sides of the Sphere should be generated.</param>
        /// <param name="calculateNormals">Calculate Vertex Normals or not.</param>
        /// <returns>The generated HalfEdgeMesh.</returns>
        public static HalfEdgeMesh GenerateSphere(Vector center,
            float radius = 1, int numSides = 8, CubeSides sides = CubeSides.All, bool calculateNormals = false)
        {
            var shiftVector = Vector.One * 0.5f;
            var mesh = new HalfEdgeMesh();
            var sideLength = 1f / numSides;
            var verticesPerSide = numSides + 1;
            var vertices = new List<Vertex>();

            var bottomVertices = new List<Vertex>(verticesPerSide * verticesPerSide);
            var topVertices = new List<Vertex>(verticesPerSide * verticesPerSide);
            var frontVertices = new List<Vertex>(verticesPerSide * verticesPerSide);
            var backVertices = new List<Vertex>(verticesPerSide * verticesPerSide);
            var leftVertices = new List<Vertex>(verticesPerSide * verticesPerSide);
            var rightVertices = new List<Vertex>(verticesPerSide * verticesPerSide);
            var idx = 0;
            for(int z = 0; z <= numSides; z++)
            {
                for(int y = 0; y <= numSides; y++)
                {
                    for(int x = 0; x <= numSides; x++)
                    {
                        if(z == 0 || z == numSides || x == 0 || x == numSides || y == 0 || y == numSides)
                        {
                            var vertex = new Vertex(x * sideLength - 0.5f, y * sideLength - 0.5f, z * sideLength - 0.5f)
                            {
                                Index = idx,
                            };
                            if(x == 0 && sides.HasFlag(CubeSides.NegativeZ))
                                bottomVertices.Add(vertex);
                            else if(x == numSides && sides.HasFlag(CubeSides.PositiveZ))
                                topVertices.Add(vertex);

                            if(y == 0 && sides.HasFlag(CubeSides.NegativeY))
                                frontVertices.Add(vertex);
                            else if(y == numSides && sides.HasFlag(CubeSides.PositiveY))
                                backVertices.Add(vertex);

                            if(z == 0 && sides.HasFlag(CubeSides.NegativeX))
                                leftVertices.Add(vertex);
                            else if(z == numSides && sides.HasFlag(CubeSides.PositiveX))
                                rightVertices.Add(vertex);

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
            mesh.AddVertices(vertices);

            var triangles = new List<Triangle>();
            for(int i = 0; i < numSides * numSides; i++)
            {
                idx = i + i / numSides;
                var nextIdx = idx + 1;
                var nextRowIdx = nextIdx + numSides;
                var nextRowNextIdx = nextRowIdx + 1;
                if(sides.HasFlag(CubeSides.NegativeZ))
                {
                    mesh.AddTriangle(new Triangle(bottomVertices[idx].Index, bottomVertices[nextRowNextIdx].Index, bottomVertices[nextIdx].Index));
                    mesh.AddTriangle(new Triangle(bottomVertices[idx].Index, bottomVertices[nextRowIdx].Index, bottomVertices[nextRowNextIdx].Index));
                }

                if(sides.HasFlag(CubeSides.PositiveZ))
                {
                    mesh.AddTriangle(new Triangle(topVertices[idx].Index, topVertices[nextIdx].Index, topVertices[nextRowNextIdx].Index));
                    mesh.AddTriangle(new Triangle(topVertices[idx].Index, topVertices[nextRowNextIdx].Index, topVertices[nextRowIdx].Index));
                }

                if(sides.HasFlag(CubeSides.NegativeY))
                {
                    mesh.AddTriangle(new Triangle(frontVertices[idx].Index, frontVertices[nextIdx].Index, frontVertices[nextRowNextIdx].Index));
                    mesh.AddTriangle(new Triangle(frontVertices[idx].Index, frontVertices[nextRowNextIdx].Index, frontVertices[nextRowIdx].Index));
                }

                if(sides.HasFlag(CubeSides.PositiveY))
                {
                    mesh.AddTriangle(new Triangle(backVertices[idx].Index, backVertices[nextRowNextIdx].Index, backVertices[nextIdx].Index));
                    mesh.AddTriangle(new Triangle(backVertices[idx].Index, backVertices[nextRowIdx].Index, backVertices[nextRowNextIdx].Index));
                }

                if(sides.HasFlag(CubeSides.NegativeX))
                {
                    mesh.AddTriangle(new Triangle(leftVertices[idx].Index, leftVertices[nextRowNextIdx].Index, leftVertices[nextIdx].Index));
                    mesh.AddTriangle(new Triangle(leftVertices[idx].Index, leftVertices[nextRowIdx].Index, leftVertices[nextRowNextIdx].Index));
                }

                if(sides.HasFlag(CubeSides.PositiveX))
                {
                    mesh.AddTriangle(new Triangle(rightVertices[idx].Index, rightVertices[nextIdx].Index, rightVertices[nextRowNextIdx].Index));
                    mesh.AddTriangle(new Triangle(rightVertices[idx].Index, rightVertices[nextRowNextIdx].Index, rightVertices[nextRowIdx].Index));
                }
            }

            if (calculateNormals)
                mesh.CalculateVertexNormals();

            return mesh;
        }

        /// <summary>
        /// Generate a HalfEdgeMesh from an existing MeshGeometry3D Object.
        /// </summary>
        /// <param name="meshGeometry">The existing MeshGeometry3D Object.</param>
        /// <param name="calculateNormals">Calculate Vertex Normals or not.</param>
        /// <returns>The generated HalfEdgeMesh.</returns>
        public static HalfEdgeMesh GenerateFromMeshGeometry3D(MeshGeometry3D meshGeometry, bool calculateNormals = false)
        {
            var vertices = meshGeometry.Positions.Select(p => new Vertex((float)p.X, (float)p.Y, (float)p.Z)).ToList();
            var triangles = new List<Triangle>();
            for(int i = 0; i < meshGeometry.TriangleIndices.Count; i += 3)
                triangles.Add(new Triangle(meshGeometry.TriangleIndices.ElementAt(i),
                    meshGeometry.TriangleIndices.ElementAt(i + 1),
                    meshGeometry.TriangleIndices.ElementAt(i + 2)));

            var mesh = new HalfEdgeMesh();
            mesh.AddVertices(vertices);
            mesh.AddTriangles(triangles);

            if(calculateNormals)
                mesh.CalculateVertexNormals();

            return mesh;
        }
    }
}