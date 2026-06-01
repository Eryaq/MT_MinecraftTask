using System;
using UnityEngine;

namespace MT_MinecraftTask.World
{
    [Serializable]
    public readonly struct ChunkCoord : IEquatable<ChunkCoord>
    {
        public readonly int X;
        public readonly int Z;

        public ChunkCoord(int x, int z)
        {
            X = x;
            Z = z;
        }

        public Vector3 WorldPosition => new(X * Chunk.SizeX, 0, Z * Chunk.SizeZ);

        public bool Equals(ChunkCoord other)
        {
            return X == other.X && Z == other.Z;
        }

        public override bool Equals(object obj)
        {
            return obj is ChunkCoord other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Z);
        }

        public override string ToString()
        {
            return $"({X}, {Z})";
        }

        public static ChunkCoord FromWorldPosition(Vector3 worldPosition)
        {
            int x = Mathf.FloorToInt(worldPosition.x / Chunk.SizeX);
            int z = Mathf.FloorToInt(worldPosition.z / Chunk.SizeZ);

            return new ChunkCoord(x, z);
        }
    }
}
