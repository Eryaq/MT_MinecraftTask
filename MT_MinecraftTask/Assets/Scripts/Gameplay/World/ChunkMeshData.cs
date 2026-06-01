using System.Collections.Generic;
using UnityEngine;

namespace MT_MinecraftTask.World
{
    public sealed class ChunkMeshData
    {
        public readonly List<Vector3> Vertices = new(4096);
        public readonly List<int> StoneTriangles = new(4096);
        public readonly List<int> GrassTriangles = new(4096);
        public readonly List<int> SnowTriangles = new(4096);

        public void Clear()
        {
            Vertices.Clear();
            StoneTriangles.Clear();
            GrassTriangles.Clear();
            SnowTriangles.Clear();
        }
    }
}
