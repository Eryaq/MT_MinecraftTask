using MT_MiencraftTask.Voxels;
using UnityEngine;

namespace MT_MiencraftTask.World
{
    public class WorldGenerator : MonoBehaviour
    {
        [SerializeField] private Chunk _chunk;
        [SerializeField] private int _seed = 12345;
        [SerializeField] private float _noiseScale = 24f;
        [SerializeField] private int _baseHeight = 24;
        [SerializeField] private int _heightVariation = 18;

        private void Start()
        {
            GenerateChunk(_chunk);
        }

        private void GenerateChunk(Chunk targetChunk)
        {
            Random.InitState(_seed);

            float offsetX = Random.Range(-10000f, 10000f);
            float offsetZ = Random.Range(-10000f, 10000f);

            for (int x = 0; x < Chunk.SizeX; x++)
            {
                for (int z = 0; z < Chunk.SizeZ; z++)
                {
                    float noise = Mathf.PerlinNoise((x + offsetX) / _noiseScale, (z + offsetZ) / _noiseScale);

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
    }
}
