using MT_MiencraftTask.Voxels;
using MT_MiencraftTask.World;
using UnityEngine;

namespace MT_MiencraftTask.Player
{
    public class BlockPlacementPreview : MonoBehaviour
    {
        [SerializeField] private PlayerVoxelInteractor _interactor;
        [SerializeField] private GameObject _previewObject;

        private void Update()
        {
            if (!_interactor.TryGetPlacementPosition(out Chunk chunk, out Vector3Int placePosition))
            {
                _previewObject.SetActive(false);
                return;
            }

            if (!chunk.IsWithinBuildLimits(placePosition.y))
            {
                _previewObject.SetActive(false);
                return;
            }

            if (chunk.GetBlock(placePosition) != EBlockType.Air)
            {
                _previewObject.SetActive(false);
                return;
            }

            _previewObject.SetActive(true);
            _previewObject.transform.position = chunk.transform.TransformPoint(placePosition) + Vector3.one * 0.5f;
        }
    }
}