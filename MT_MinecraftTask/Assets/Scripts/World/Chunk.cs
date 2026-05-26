using MT_MiencraftTask.Voxels;
using UnityEngine;

namespace MT_MiencraftTask.World
{
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider))]
    public class Chunk : MonoBehaviour
    {
        [SerializeField] private BlockDatabase _blockDatabase;

        public const int SizeX = 16;
        public const int SizeY = 64;
        public const int SizeZ = 16;

        public const int MinBuildY = 0;
        public const int MaxBuildY = SizeY - 1;

        private EBlockType[,,] _blocks;

        private Mesh _mesh;

        public MeshFilter MeshFilter { get; private set; }
        public MeshRenderer MeshRenderer { get; private set; }
        public MeshCollider MeshCollider { get; private set; }
        public ChunkCoord Coord { get; private set; }
        public WorldManager World { get; private set; }

        private void Awake()
        {
            MeshFilter = GetComponent<MeshFilter>();
            MeshRenderer = GetComponent<MeshRenderer>();
            MeshCollider = GetComponent<MeshCollider>();

            _blocks = new EBlockType[SizeX, SizeY, SizeZ];
        }

        public void Initialize(ChunkCoord coord, WorldManager world)
        {
            Coord = coord;
            World = world;
        }

        public EBlockType GetBlock(int x, int y, int z)
        {
            if (!IsInside(x, y, z))
                return EBlockType.Air;

            return _blocks[x, y, z];
        }

        public void SetBlock(int x, int y, int z, EBlockType type)
        {
            if (!IsInside(x, y, z))
                return;

            _blocks[x, y, z] = type;
        }

        public bool TrySetBlock(Vector3Int localPosition, EBlockType type)
        {
            if (!IsInside(localPosition.x, localPosition.y, localPosition.z))
                return false;

            if (!IsWithinBuildLimits(localPosition.y))
                return false;

            if (localPosition.y == MinBuildY && type == EBlockType.Air)
                return false;

            _blocks[localPosition.x, localPosition.y, localPosition.z] = type;

            World.RebuildChunkAndAffectedNeighbors(this, localPosition);

            return true;
        }

        public EBlockType GetBlock(Vector3Int localPosition)
        {
            return GetBlock(localPosition.x, localPosition.y, localPosition.z);
        }

        public bool IsInside(int x, int y, int z)
        {
            return x >= 0 && x < SizeX && y >= 0 && y < SizeY && z >= 0 && z < SizeZ;
        }

        public void RebuildMesh()
        {
            if (_mesh != null)
            {
                Destroy(_mesh);
                _mesh = null;
            }

            _mesh = ChunkMeshBuilder.BuildMesh(this);

            MeshFilter.sharedMesh = _mesh;
            MeshCollider.sharedMesh = _mesh;

            MeshRenderer.sharedMaterials = new[]
            {
                _blockDatabase.Get(EBlockType.Stone).Material,
                _blockDatabase.Get(EBlockType.Grass).Material,
                _blockDatabase.Get(EBlockType.Snow).Material
            };
        }

        public bool IsWithinBuildLimits(int y)
        {
            return y >= MinBuildY && y <= MaxBuildY;
        }

        public Vector3Int LocalToWorldBlockPosition(Vector3Int localPosition)
        {
            return new Vector3Int(Coord.X * SizeX + localPosition.x, localPosition.y, Coord.Z * SizeZ + localPosition.z);
        }

        public void ClearBlocks()
        {
            for (int x = 0; x < SizeX; x++)
            {
                for (int y = 0; y < SizeY; y++)
                {
                    for (int z = 0; z < SizeZ; z++)
                    {
                        _blocks[x, y, z] = EBlockType.Air;
                    }
                }
            }
        }

        public void ClearMesh()
        {
            MeshFilter.sharedMesh = null;
            MeshCollider.sharedMesh = null;

            if (_mesh != null)
            {
                Destroy(_mesh);
                _mesh = null;
            }
        }
    }
}
