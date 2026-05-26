using MT_MiencraftTask.Voxels;
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

        private static readonly Vector2Int[] NeighborDirections =
        {
            new(0, 1),
            new(0, -1),
            new(-1, 0),
            new(1, 0)
        };

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
            HashSet<ChunkCoord> neededChunks = new();

            for (int x = -_viewDistance; x <= _viewDistance; x++)
            {
                for (int z = -_viewDistance; z <= _viewDistance; z++)
                {
                    ChunkCoord coord = new(_currentPlayerChunk.X + x, _currentPlayerChunk.Z + z);

                    neededChunks.Add(coord);

                    if (!_loadedChunks.ContainsKey(coord))
                        CreateChunk(coord);
                }
            }

            List<ChunkCoord> chunksToUnload = new();

            foreach (var loadedChunk in _loadedChunks)
            {
                if (!neededChunks.Contains(loadedChunk.Key))
                    chunksToUnload.Add(loadedChunk.Key);
            }

            foreach (ChunkCoord coord in chunksToUnload)
            {
                Destroy(_loadedChunks[coord].gameObject);
                _loadedChunks.Remove(coord);
            }
        }

        private void CreateChunk(ChunkCoord coord)
        {
            Chunk chunk = Instantiate(_chunkPrefab, coord.WorldPosition, Quaternion.identity, transform);

            chunk.name = $"Chunk_{coord.X}_{coord.Z}";

            chunk.Initialize(coord, this);
            _loadedChunks.Add(coord, chunk);
            _worldGenerator.GenerateChunk(chunk, coord);
            RebuildChunkAndNeighbors(coord);

        }

        private void RebuildChunkAndNeighbors(ChunkCoord centerCoord)
        {
            if (_loadedChunks.TryGetValue(centerCoord, out Chunk centerChunk))
                centerChunk.RebuildMesh();

            foreach (Vector2Int direction in NeighborDirections)
            {
                ChunkCoord neighborCoord = new(centerCoord.X + direction.x, centerCoord.Z + direction.y);

                if (_loadedChunks.TryGetValue(neighborCoord, out Chunk neighborChunk))
                    neighborChunk.RebuildMesh();
            }
        }

        public void RebuildChunkAndAffectedNeighbors(Chunk chunk, Vector3Int changedLocalPosition)
        {
            chunk.RebuildMesh();

            if (changedLocalPosition.x == 0)
                RebuildChunkIfLoaded(new ChunkCoord(chunk.Coord.X - 1, chunk.Coord.Z));

            if (changedLocalPosition.x == Chunk.SizeX - 1)
                RebuildChunkIfLoaded(new ChunkCoord(chunk.Coord.X + 1, chunk.Coord.Z));

            if (changedLocalPosition.z == 0)
                RebuildChunkIfLoaded(new ChunkCoord(chunk.Coord.X, chunk.Coord.Z - 1));

            if (changedLocalPosition.z == Chunk.SizeZ - 1)
                RebuildChunkIfLoaded(new ChunkCoord(chunk.Coord.X, chunk.Coord.Z + 1));
        }

        private void RebuildChunkIfLoaded(ChunkCoord coord)
        {
            if (_loadedChunks.TryGetValue(coord, out Chunk chunk))
                chunk.RebuildMesh();
        }

        #region LookUpMethods

        public bool TryGetChunk(ChunkCoord coord, out Chunk chunk)
        {
            return _loadedChunks.TryGetValue(coord, out chunk);
        }

        public EBlockType GetBlockWorld(Vector3Int worldPosition)
        {
            ChunkCoord coord = ChunkCoord.FromWorldPosition(worldPosition);

            if (!_loadedChunks.TryGetValue(coord, out Chunk chunk))
                return EBlockType.Air;

            Vector3Int localPosition = WorldToLocalBlockPosition(worldPosition);

            return chunk.GetBlock(localPosition);
        }

        public static Vector3Int WorldToLocalBlockPosition(Vector3Int worldPosition)
        {
            int localX = Mod(worldPosition.x, Chunk.SizeX);
            int localZ = Mod(worldPosition.z, Chunk.SizeZ);

            return new Vector3Int(localX, worldPosition.y, localZ);
        }

        private static int Mod(int value, int modulo)
        {
            return (value % modulo + modulo) % modulo;
        }

        #endregion
    }
}
