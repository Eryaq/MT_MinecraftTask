using MT_MinecraftTask.Core;
using UnityEngine;

namespace MT_MinecraftTask.UI
{
    public class GameUIController : MonoBehaviour
    {
        [SerializeField] private GameStateManager _gameStateManager;

        [Header("UI Roots")]
        [SerializeField] private GameObject _mainMenuRoot;
        [SerializeField] private GameObject _loadingRoot;
        [SerializeField] private GameObject _gameHudRoot;
        [SerializeField] private GameObject _pauseMenuRoot;

        private void OnEnable()
        {
            _gameStateManager.StateChanged += OnStateChanged;
        }

        private void OnDisable()
        {
            _gameStateManager.StateChanged -= OnStateChanged;
        }

        private void Start()
        {
            OnStateChanged(_gameStateManager.CurrentState);
        }

        private void OnStateChanged(GameState state)
        {
            _mainMenuRoot.SetActive(state == GameState.MainMenu);
            _loadingRoot.SetActive(state == GameState.Loading);
            _gameHudRoot.SetActive(state == GameState.Gameplay);
            _pauseMenuRoot.SetActive(state == GameState.Paused);
        }
    }
}
