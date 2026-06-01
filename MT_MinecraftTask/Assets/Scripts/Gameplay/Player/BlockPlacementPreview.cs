using MT_MinecraftTask.Voxels;
using MT_MinecraftTask.World;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MT_MinecraftTask.Player
{
    public class BlockPlacementPreview : MonoBehaviour
    {
        [SerializeField] private PlayerVoxelInteractor _interactor;
        [SerializeField] private GameObject _previewObject;
        [SerializeField] private WorldManager _worldManager;

        [SerializeField] private InputActionReference _togglePreviewAction;
        [SerializeField] private bool _isPreviewEnabled = true;

        private void OnEnable()
        {
            _togglePreviewAction.action.Enable();
        }

        private void OnDisable()
        {
            _togglePreviewAction.action.Disable();
        }

        private void Update()
        {
            if (_togglePreviewAction.action.WasPressedThisFrame())
            {
                _isPreviewEnabled = !_isPreviewEnabled;

                if (!_isPreviewEnabled)
                    _previewObject.SetActive(false);
            }

            if (!_isPreviewEnabled)
                return;

            if (!_interactor.TryGetPlacementWorldPosition(out Vector3Int worldPosition))
            {
                _previewObject.SetActive(false);
                return;
            }

            if (worldPosition.y < Chunk.MinBuildY || worldPosition.y > Chunk.MaxBuildY)
            {
                _previewObject.SetActive(false);
                return;
            }

            if (_worldManager.GetBlockWorld(worldPosition) != EBlockType.Air)
            {
                _previewObject.SetActive(false);
                return;
            }

            _previewObject.SetActive(true);
            _previewObject.transform.position = worldPosition + Vector3.one * 0.5f;
        }
    }
}