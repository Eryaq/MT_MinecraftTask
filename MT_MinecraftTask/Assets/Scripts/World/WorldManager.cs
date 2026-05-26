using System.Collections.Generic;
using UnityEngine;

namespace MT_MiencraftTask.World
{
    public class WorldManager : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Chunk _chunkPrefab;
        [SerializeField] private WorldGenerator _worldGenerator;
        [SerializeField] private Transform _player;

        [Header("Chunks")]
        [SerializeField] private int _viewDistance = 2;

        private readonly Dictionary<ChunkCoord, Chunk> _loadedChunks = new();

        private ChunkCoord _currentPlayerChunk;

        private void Start()
        {
            _currentPlayerChunk = ChunkCoord.FromWorldPosition(_player.position);
            RefreshChunksAroundPlayer();
        }

        private void Update()
        {
            ChunkCoord newPlayerChunk = ChunkCoord.FromWorldPosition(_player.position);

            if (newPlayerChunk.Equals(_currentPlayerChunk))
                return;

            _currentPlayerChunk = newPlayerChunk;
            RefreshChunksAroundPlayer();
        }

        private void RefreshChunksAroundPlayer()
        {
            for (int x = -_viewDistance; x <= _viewDistance; x++)
            {
                for (int z = -_viewDistance; z <= _viewDistance; z++)
                {
                    ChunkCoord coord = new(_currentPlayerChunk.X + x, _currentPlayerChunk.Z + z);

                    if (_loadedChunks.ContainsKey(coord))
                        continue;

                    CreateChunk(coord);
                }
            }
        }

        private void CreateChunk(ChunkCoord coord)
        {
            Chunk chunk = Instantiate(_chunkPrefab, coord.WorldPosition, Quaternion.identity, transform);

            chunk.name = $"Chunk_{coord.X}_{coord.Z}";

            _worldGenerator.GenerateChunk(chunk, coord);

            _loadedChunks.Add(coord, chunk);
        }
    }
}
