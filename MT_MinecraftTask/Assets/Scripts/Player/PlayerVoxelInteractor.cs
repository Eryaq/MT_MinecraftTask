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

        private Chunk _targetChunk;
        private Vector3Int _targetBlockPosition;
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

            if (!TryGetTargetBlock(out Chunk chunk, out Vector3Int blockPosition))
            {
                ResetMining();
                return;
            }

            if (chunk != _targetChunk || blockPosition != _targetBlockPosition)
            {
                _targetChunk = chunk;
                _targetBlockPosition = blockPosition;
                _mineProgress = 0f;
            }

            EBlockType blockType = chunk.GetBlock(blockPosition);

            float miningTime = GetMiningTime(blockType);

            _mineProgress += Time.deltaTime;

            if (_mineProgress >= miningTime)
            {
                chunk.TrySetBlock(blockPosition, EBlockType.Air);
                ResetMining();
            }
        }

        private bool TryGetTargetBlock(out Chunk chunk, out Vector3Int blockPosition)
        {
            chunk = null;
            blockPosition = default;

            Ray ray = new Ray(_camera.transform.position, _camera.transform.forward);

            if (!Physics.Raycast(ray, out RaycastHit hit, _interactionDistance))
                return false;

            chunk = hit.collider.GetComponent<Chunk>();

            if (chunk == null)
                return false;

            Vector3 localHitPoint = chunk.transform.InverseTransformPoint(hit.point);

            Vector3 insideBlockPoint = localHitPoint - hit.normal * 0.01f;

            blockPosition = Vector3Int.FloorToInt(insideBlockPoint);

            return chunk.IsInside(blockPosition.x, blockPosition.y, blockPosition.z);
        }

        private void HandlePlacement()
        {
            if (!_placeAction.action.WasPressedThisFrame())
                return;

            if (!TryGetPlacementPosition(out Chunk chunk, out Vector3Int placePosition))
                return;

            if (chunk.GetBlock(placePosition) != EBlockType.Air)
                return;

            chunk.TrySetBlock(placePosition, EBlockType.Grass);
        }

        private bool TryGetPlacementPosition(out Chunk chunk, out Vector3Int blockPosition)
        {
            chunk = null;
            blockPosition = default;

            Ray ray = new Ray(_camera.transform.position, _camera.transform.forward);

            if (!Physics.Raycast(ray, out RaycastHit hit, _interactionDistance))
                return false;

            chunk = hit.collider.GetComponent<Chunk>();

            if (chunk == null)
                return false;

            Vector3 localHitPoint = chunk.transform.InverseTransformPoint(hit.point);
            Vector3 localNormal = chunk.transform.InverseTransformDirection(hit.normal);

            Vector3 outsideBlockPoint = localHitPoint + localNormal * 0.01f;

            blockPosition = Vector3Int.FloorToInt(outsideBlockPoint);

            return chunk.IsInside(blockPosition.x, blockPosition.y, blockPosition.z);
        }

        private float GetMiningTime(EBlockType type)
        {
            return type switch
            {
                EBlockType.Stone => 2.0f,
                EBlockType.Grass => 1.0f,
                EBlockType.Snow => 0.5f,
                _ => 1.0f
            };
        }

        private void ResetMining()
        {
            _targetChunk = null;
            _targetBlockPosition = default;
            _mineProgress = 0f;
        }
    }
}
