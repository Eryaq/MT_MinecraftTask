using MT_MiencraftTask.World;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MT_MiencraftTask.UI
{
    public class DebugOverlayHUD : MonoBehaviour
    {
        [SerializeField] private WorldManager _worldManager;
        [SerializeField] private TMP_Text _text;
        [SerializeField] private float _refreshInterval = 0.25f;

        [SerializeField] private InputActionReference _toggleDebugAction;
        [SerializeField] private GameObject _debugRoot;
        [SerializeField] private bool _isVisible = true;

        private float _timer;
        private float _deltaTime;

        private void OnEnable()
        {
            _toggleDebugAction.action.Enable();
        }

        private void OnDisable()
        {
            _toggleDebugAction.action.Disable();
        }

        private void Update()
        {
            if (_toggleDebugAction.action.WasPressedThisFrame())
            {
                _isVisible = !_isVisible;
                _debugRoot.SetActive(_isVisible);
            }

            if (!_isVisible)
                return;

            _deltaTime += (Time.unscaledDeltaTime - _deltaTime) * 0.1f;

            _timer += Time.unscaledDeltaTime;

            if (_timer < _refreshInterval)
                return;

            _timer = 0f;
            Refresh();
        }

        private void Refresh()
        {
            float fps = 1f / _deltaTime;

            _text.text =
                $"FPS: {fps:0}\n" +
                $"Player Chunk: {_worldManager.CurrentPlayerChunk}\n" +
                $"Loaded Chunks: {_worldManager.LoadedChunkCount}\n" +
                $"Pooled Chunks: {_worldManager.PooledChunkCount}\n" +
                $"Dirty Chunks: {_worldManager.DirtyChunkCount}\n" +
                $"Total Rebuilds: {_worldManager.TotalChunkRebuilds}";
        }
    }
}
