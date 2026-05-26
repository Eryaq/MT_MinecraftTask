using MT_MiencraftTask.Voxels;
using MT_MiencraftTask.World;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MT_MiencraftTask.Player
{
    public class PlayerVoxelInteractor : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Camera _camera;

        [Header("Input")]
        [SerializeField] private InputActionReference _mineAction;
        [SerializeField] private InputActionReference _placeAction;

        [Header("Mining")]
        [SerializeField] private float _interactionDistance = 6f;

        [Header("Inventory")]
        [SerializeField] private PlayerBlockInventory _inventory;
        [SerializeField] private EBlockType _selectedPlacementBlock = EBlockType.Grass;

        [SerializeField] private BlockDatabase _blockDatabase;
        [SerializeField] private WorldManager _worldManager;

        private Vector3Int _targetWorldBlockPosition;
        private bool _hasTargetBlock;
        private float _mineProgress;

        private void OnEnable()
        {
            _mineAction.action.Enable();
            _placeAction.action.Enable();
        }

        private void OnDisable()
        {
            _mineAction.action.Disable();
            _placeAction.action.Disable();
        }

        private void Update()
        {
            HandleMining();
            HandlePlacement();
        }

        private void HandleMining()
        {
            if (!_mineAction.action.IsPressed())
            {
                ResetMining();
                return;
            }

            if (!TryGetTargetBlockWorldPosition(out Vector3Int worldBlockPosition))
            {
                ResetMining();
                return;
            }

            EBlockType blockType = _worldManager.GetBlockWorld(worldBlockPosition);

            if (blockType == EBlockType.Air)
            {
                ResetMining();
                return;
            }

            if (worldBlockPosition.y == Chunk.MinBuildY)
            {
                ResetMining();
                return;
            }

            if (!_hasTargetBlock || worldBlockPosition != _targetWorldBlockPosition)
            {
                _targetWorldBlockPosition = worldBlockPosition;
                _hasTargetBlock = true;
                _mineProgress = 0f;
            }

            float miningTime = _blockDatabase.GetMiningTime(blockType);
            _mineProgress += Time.deltaTime;

            if (_mineProgress >= miningTime)
            {
                if (_worldManager.TrySetBlockWorld(worldBlockPosition, EBlockType.Air))
                    _inventory.Add(blockType);

                ResetMining();
            }
        }

        private bool TryGetTargetBlockWorldPosition(out Vector3Int worldBlockPosition)
        {
            worldBlockPosition = default;

            Ray ray = new Ray(_camera.transform.position, _camera.transform.forward);

            if (!Physics.Raycast(ray, out RaycastHit hit, _interactionDistance))
                return false;

            Chunk chunk = hit.collider.GetComponent<Chunk>();

            if (chunk == null)
                return false;

            Vector3 localHitPoint = chunk.transform.InverseTransformPoint(hit.point);
            Vector3 localNormal = chunk.transform.InverseTransformDirection(hit.normal);

            Vector3 insideBlockPoint = localHitPoint - localNormal * 0.01f;
            Vector3Int localBlockPosition = Vector3Int.FloorToInt(insideBlockPoint);

            worldBlockPosition = chunk.LocalToWorldBlockPosition(localBlockPosition);

            return true;
        }

        private void HandlePlacement()
        {
            if (!_placeAction.action.WasPressedThisFrame())
                return;

            if (!TryGetPlacementWorldPosition(out Vector3Int worldPosition))
                return;

            if (worldPosition.y < Chunk.MinBuildY || worldPosition.y > Chunk.MaxBuildY)
                return;

            if (_worldManager.GetBlockWorld(worldPosition) != EBlockType.Air)
                return;

            if (!_inventory.TryConsume(_selectedPlacementBlock))
                return;

            if (!_worldManager.TrySetBlockWorld(worldPosition, _selectedPlacementBlock))
                _inventory.Add(_selectedPlacementBlock);
        }

        public bool TryGetPlacementWorldPosition(out Vector3Int worldBlockPosition)
        {
            worldBlockPosition = default;

            Ray ray = new Ray(_camera.transform.position, _camera.transform.forward);

            if (!Physics.Raycast(ray, out RaycastHit hit, _interactionDistance))
                return false;

            Chunk chunk = hit.collider.GetComponent<Chunk>();

            if (chunk == null)
                return false;

            Vector3 localHitPoint = chunk.transform.InverseTransformPoint(hit.point);
            Vector3 localNormal = chunk.transform.InverseTransformDirection(hit.normal);

            Vector3 outsideBlockPoint = localHitPoint + localNormal * 0.01f;
            Vector3Int localBlockPosition = Vector3Int.FloorToInt(outsideBlockPoint);

            worldBlockPosition = chunk.LocalToWorldBlockPosition(localBlockPosition);

            return true;
        }

        private void ResetMining()
        {
            _targetWorldBlockPosition = default;
            _hasTargetBlock = false;
            _mineProgress = 0f;
        }
    }
}
