using MT_MiencraftTask.Voxels;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

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

        [Header("Input")]
        [SerializeField] private InputActionReference _saveWorldAction;
        [SerializeField] private InputActionReference _loadWorldAction;

        private const string SaveKey = "MinecraftDemo_WorldSave";

        private readonly Dictionary<ChunkCoord, Chunk> _loadedChunks = new();
        private readonly Dictionary<Vector3Int, EBlockType> _worldModifications = new();
        private readonly Queue<Chunk> _chunkPool = new();

        private ChunkCoord _currentPlayerChunk;

        private static readonly Vector2Int[] NeighborDirections =
        {
            new(0, 1),
            new(0, -1),
            new(-1, 0),
            new(1, 0)
        };

        private void OnEnable()
        {
            _saveWorldAction.action.Enable();
            _loadWorldAction.action.Enable();
        }

        private void OnDisable()
        {
            _saveWorldAction.action.Disable();
            _loadWorldAction.action.Disable();
        }

        private void Start()
        {
            _currentPlayerChunk = ChunkCoord.FromWorldPosition(_player.position);
            RefreshChunksAroundPlayer();
        }

        private void Update()
        {
            HandleSaveLoadInput();

            ChunkCoord newPlayerChunk = ChunkCoord.FromWorldPosition(_player.position);

            if (newPlayerChunk.Equals(_currentPlayerChunk))
                return;

            _currentPlayerChunk = newPlayerChunk;
            RefreshChunksAroundPlayer();
        }

        private void HandleSaveLoadInput()
        {
            if (_saveWorldAction.action.WasPressedThisFrame())
                SaveWorld();

            if (_loadWorldAction.action.WasPressedThisFrame())
                LoadWorld();
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
                ReleaseChunk(_loadedChunks[coord]);
                _loadedChunks.Remove(coord);
            }
        }

        private void CreateChunk(ChunkCoord coord)
        {
            Chunk chunk = GetChunkFromPool();

            chunk.transform.SetParent(transform);
            chunk.transform.position = coord.WorldPosition;
            chunk.transform.rotation = Quaternion.identity;
            chunk.name = $"Chunk_{coord.X}_{coord.Z}";
            chunk.gameObject.SetActive(true);

            chunk.Initialize(coord, this);
            chunk.ClearBlocks();

            _loadedChunks.Add(coord, chunk);

            _worldGenerator.GenerateChunk(chunk, coord);
            ApplyModificationsToChunk(chunk);
            chunk.RebuildMesh();

            RebuildChunkAndNeighbors(coord);

        }

        private void ReleaseChunk(Chunk chunk)
        {
            chunk.MeshFilter.sharedMesh = null;
            chunk.MeshCollider.sharedMesh = null;

            chunk.gameObject.SetActive(false);
            _chunkPool.Enqueue(chunk);
        }

        private Chunk GetChunkFromPool()
        {
            if (_chunkPool.Count > 0)
                return _chunkPool.Dequeue();

            return Instantiate(_chunkPrefab);
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

        public bool TrySetBlockWorld(Vector3Int worldPosition, EBlockType type)
        {
            ChunkCoord coord = ChunkCoord.FromWorldPosition(worldPosition);

            if (!_loadedChunks.TryGetValue(coord, out Chunk chunk))
                return false;

            Vector3Int localPosition = WorldToLocalBlockPosition(worldPosition);

            if (!chunk.TrySetBlock(localPosition, type))
                return false;

            _worldModifications[worldPosition] = type;

            return true;
        }

        private void ApplyModificationsToChunk(Chunk chunk)
        {
            foreach (var modification in _worldModifications)
            {
                Vector3Int worldPosition = modification.Key;

                ChunkCoord coord = ChunkCoord.FromWorldPosition(worldPosition);

                if (!coord.Equals(chunk.Coord))
                    continue;

                Vector3Int localPosition = WorldToLocalBlockPosition(worldPosition);
                chunk.SetBlock(localPosition.x, localPosition.y, localPosition.z, modification.Value);
            }
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

        #region Save/Load

        public void SaveWorld()
        {
            WorldSaveData saveData = new();

            foreach (var modification in _worldModifications)
            {
                saveData.Modifications.Add(
                    new WorldModification(modification.Key, modification.Value)
                );
            }

            string json = JsonUtility.ToJson(saveData);
            PlayerPrefs.SetString(SaveKey, json);
            PlayerPrefs.Save();

            Debug.Log($"World saved. Modifications: {saveData.Modifications.Count}");
        }

        public void LoadWorld()
        {
            if (!PlayerPrefs.HasKey(SaveKey))
            {
                Debug.Log("No world save found.");
                return;
            }

            string json = PlayerPrefs.GetString(SaveKey);
            WorldSaveData saveData = JsonUtility.FromJson<WorldSaveData>(json);

            _worldModifications.Clear();

            foreach (WorldModification modification in saveData.Modifications)
            {
                _worldModifications[modification.WorldPosition] = modification.BlockType;
            }

            foreach (Chunk chunk in _loadedChunks.Values)
            {
                _worldGenerator.GenerateChunk(chunk, chunk.Coord);
                ApplyModificationsToChunk(chunk);
                chunk.RebuildMesh();
            }

            Debug.Log($"World loaded. Modifications: {_worldModifications.Count}");
        }

        #endregion
    }
}
