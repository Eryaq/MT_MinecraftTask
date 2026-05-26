using MT_MiencraftTask.Voxels;
using System.Collections.Generic;
using UnityEngine;

namespace MT_MiencraftTask.World
{
    public static class ChunkMeshBuilder
    {
        private static readonly Vector3Int[] Directions =
        {
            Vector3Int.forward,
            Vector3Int.back,
            Vector3Int.left,
            Vector3Int.right,
            Vector3Int.up,
            Vector3Int.down
        };

        public static Mesh BuildMesh(Chunk chunk)
        {
            var vertices = new List<Vector3>();

            var stoneTriangles = new List<int>();
            var grassTriangles = new List<int>();
            var snowTriangles = new List<int>();

            for (int x = 0; x < Chunk.SizeX; x++)
            {
                for (int y = 0; y < Chunk.SizeY; y++)
                {
                    for (int z = 0; z < Chunk.SizeZ; z++)
                    {
                        EBlockType block = chunk.GetBlock(x, y, z);

                        if (block == EBlockType.Air)
                            continue;

                        Vector3Int blockPos = new(x, y, z);

                        List<int> targetTriangles = GetTriangleList(block, stoneTriangles, grassTriangles, snowTriangles);

                        for (int i = 0; i < Directions.Length; i++)
                        {
                            Vector3Int neighborLocalPos = blockPos + Directions[i];
                            EBlockType neighborBlock;

                            if (chunk.IsInside(neighborLocalPos.x, neighborLocalPos.y, neighborLocalPos.z))
                            {
                                neighborBlock = chunk.GetBlock(neighborLocalPos);
                            }
                            else
                            {
                                Vector3Int worldBlockPos = chunk.LocalToWorldBlockPosition(neighborLocalPos);
                                neighborBlock = chunk.World.GetBlockWorld(worldBlockPos);
                            }

                            if (neighborBlock == EBlockType.Air)
                            {
                                AddFace(vertices, targetTriangles, blockPos, Directions[i]);
                            }
                        }
                    }
                }
            }

            var mesh = new Mesh
            {
                indexFormat = UnityEngine.Rendering.IndexFormat.UInt32,
                subMeshCount = 3
            };

            mesh.SetVertices(vertices);

            mesh.SetTriangles(stoneTriangles, 0);
            mesh.SetTriangles(grassTriangles, 1);
            mesh.SetTriangles(snowTriangles, 2);

            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            return mesh;
        }

        private static List<int> GetTriangleList(EBlockType block, List<int> stoneTriangles, List<int> grassTriangles, List<int> snowTriangles)
        {
            return block switch
            {
                EBlockType.Stone => stoneTriangles,
                EBlockType.Grass => grassTriangles,
                EBlockType.Snow => snowTriangles,
                _ => stoneTriangles
            };
        }

        private static void AddFace(List<Vector3> vertices, List<int> triangles, Vector3 position, Vector3Int direction)
        {
            int startIndex = vertices.Count;

            if (direction == Vector3Int.forward)
            {
                vertices.Add(position + new Vector3(0, 0, 1));
                vertices.Add(position + new Vector3(1, 0, 1));
                vertices.Add(position + new Vector3(1, 1, 1));
                vertices.Add(position + new Vector3(0, 1, 1));
            }
            else if (direction == Vector3Int.back)
            {
                vertices.Add(position + new Vector3(1, 0, 0));
                vertices.Add(position + new Vector3(0, 0, 0));
                vertices.Add(position + new Vector3(0, 1, 0));
                vertices.Add(position + new Vector3(1, 1, 0));
            }
            else if (direction == Vector3Int.left)
            {
                vertices.Add(position + new Vector3(0, 0, 0));
                vertices.Add(position + new Vector3(0, 0, 1));
                vertices.Add(position + new Vector3(0, 1, 1));
                vertices.Add(position + new Vector3(0, 1, 0));
            }
            else if (direction == Vector3Int.right)
            {
                vertices.Add(position + new Vector3(1, 0, 1));
                vertices.Add(position + new Vector3(1, 0, 0));
                vertices.Add(position + new Vector3(1, 1, 0));
                vertices.Add(position + new Vector3(1, 1, 1));
            }
            else if (direction == Vector3Int.up)
            {
                vertices.Add(position + new Vector3(0, 1, 1));
                vertices.Add(position + new Vector3(1, 1, 1));
                vertices.Add(position + new Vector3(1, 1, 0));
                vertices.Add(position + new Vector3(0, 1, 0));
            }
            else if (direction == Vector3Int.down)
            {
                vertices.Add(position + new Vector3(0, 0, 0));
                vertices.Add(position + new Vector3(1, 0, 0));
                vertices.Add(position + new Vector3(1, 0, 1));
                vertices.Add(position + new Vector3(0, 0, 1));
            }

            triangles.Add(startIndex);
            triangles.Add(startIndex + 1);
            triangles.Add(startIndex + 2);

            triangles.Add(startIndex);
            triangles.Add(startIndex + 2);
            triangles.Add(startIndex + 3);
        }
    }
}
