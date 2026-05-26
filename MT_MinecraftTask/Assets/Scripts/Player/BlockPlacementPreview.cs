using MT_MiencraftTask.Voxels;
using MT_MiencraftTask.World;
using UnityEngine;

namespace MT_MiencraftTask.Player
{
    public class BlockPlacementPreview : MonoBehaviour
    {
        [SerializeField] private PlayerVoxelInteractor _interactor;
        [SerializeField] private GameObject _previewObject;
        [SerializeField] private WorldManager _worldManager;

        private void Update()
        {
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