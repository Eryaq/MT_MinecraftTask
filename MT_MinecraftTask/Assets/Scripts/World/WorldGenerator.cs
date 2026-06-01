using MT_MiencraftTask.Voxels;
using UnityEngine;

namespace MT_MiencraftTask.World
{
    public class WorldGenerator : MonoBehaviour
    {
        [SerializeField] private bool _randomizeSeedOnStart = true;
        [SerializeField] private int _seed = 12345;
        [SerializeField] private float _noiseScale = 24f;
        [SerializeField] private int _baseHeight = 24;
        [SerializeField] private int _heightVariation = 18;

        private float _offsetX;
        private float _offsetZ;

        public int Seed => _seed;

        private void Awake()
        {
            if (_randomizeSeedOnStart)
                _seed = Random.Range(int.MinValue, int.MaxValue);

            InitializeSeed(_seed);
        }

        public void InitializeSeed(int seed)
        {
            _seed = seed;

            System.Random random = new(_seed);

            _offsetX = random.Next(-10000, 10000);
            _offsetZ = random.Next(-10000, 10000);
        }

        public void GenerateChunk(Chunk targetChunk, ChunkCoord coord)
        {
            for (int x = 0; x < Chunk.SizeX; x++)
            {
                for (int z = 0; z < Chunk.SizeZ; z++)
                {
                    int worldX = coord.X * Chunk.SizeX + x;
                    int worldZ = coord.Z * Chunk.SizeZ + z;

                    float noise = Mathf.PerlinNoise((worldX + _offsetX) / _noiseScale, (worldZ + _offsetZ) / _noiseScale);

                    int height = _baseHeight + Mathf.RoundToInt(noise * _heightVariation);
                    height = Mathf.Clamp(height, 0, Chunk.SizeY - 1);

                    for (int y = 0; y <= height; y++)
                    {
                        EBlockType type;

                        if (y < height - 4)
                            type = EBlockType.Stone;
                        else if (y == height)
                            type = GetSurfaceBlock(height);
                        else
                            type = EBlockType.Grass;

                        targetChunk.SetBlock(x, y, z, type);
                    }
                }
            }

            targetChunk.RebuildMesh();
        }

        private EBlockType GetSurfaceBlock(int height)
        {
            if (height <= 22)
                return EBlockType.Stone;

            if (height >= 36)
                return EBlockType.Snow;

            return EBlockType.Grass;
        }

        public void RandomizeSeed()
        {
            int newSeed = Random.Range(int.MinValue, int.MaxValue);
            InitializeSeed(newSeed);
        }
    }
}
