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

        private EBlockType[,,] _blocks;

        public MeshFilter MeshFilter { get; private set; }
        public MeshRenderer MeshRenderer { get; private set; }
        public MeshCollider MeshCollider { get; private set; }

        private void Awake()
        {
            MeshFilter = GetComponent<MeshFilter>();
            MeshRenderer = GetComponent<MeshRenderer>();
            MeshCollider = GetComponent<MeshCollider>();

            _blocks = new EBlockType[SizeX, SizeY, SizeZ];
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

        public bool IsInside(int x, int y, int z)
        {
            return x >= 0 && x < SizeX &&
                   y >= 0 && y < SizeY &&
                   z >= 0 && z < SizeZ;
        }

        public void RebuildMesh()
        {
            Mesh mesh = ChunkMeshBuilder.BuildMesh(this);

            MeshFilter.sharedMesh = mesh;
            MeshCollider.sharedMesh = mesh;

            MeshRenderer.sharedMaterials = new[]
            {
                _blockDatabase.Get(EBlockType.Stone).Material,
                _blockDatabase.Get(EBlockType.Grass).Material,
                _blockDatabase.Get(EBlockType.Snow).Material
            };
        }
    }
}
