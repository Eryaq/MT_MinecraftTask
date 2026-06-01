using MT_MiencraftTask.Player;
using MT_MiencraftTask.World;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MT_MiencraftTask.Core
{
    public class GameStateManager : MonoBehaviour
    {
        public event Action<GameState> StateChanged;

        [SerializeField] private WorldManager _worldManager;
        [SerializeField] private InputActionReference _pauseAction;
        [SerializeField] private Transform _player;
        [SerializeField] private PlayerBlockInventory _inventory;
        [SerializeField] private Vector3 _playerStartPosition = new(0f, 80f, 0f);
        [SerializeField] private CharacterController _playerController;

        public GameState CurrentState { get; private set; }

        private void Start()
        {
            SetState(GameState.MainMenu);
        }

        private void Update()
        {
            HandlePauseInput();

            if (CurrentState != GameState.Loading)
                return;

            if (_worldManager.IsInitialLoadingFinished)
            {
                _worldManager.MovePlayerAboveTerrain();
                EnterGameplay();
            }
        }

        private void OnEnable()
        {
            _pauseAction.action.Enable();
        }

        private void OnDisable()
        {
            _pauseAction.action.Disable();
        }

        private void HandlePauseInput()
        {
            if (!_pauseAction.action.WasPressedThisFrame())
                return;

            if (CurrentState == GameState.Gameplay)
                PauseGame();
            else if (CurrentState == GameState.Paused)
                ResumeGame();
        }

        public void StartGame()
        {
            SetState(GameState.Loading);

            _worldManager.BeginInitialLoading();
        }

        public void EnterGameplay()
        {
            SetState(GameState.Gameplay);
        }

        public void PauseGame()
        {
            if (CurrentState != GameState.Gameplay)
                return;

            SetState(GameState.Paused);
        }

        public void ResumeGame()
        {
            if (CurrentState != GameState.Paused)
                return;

            SetState(GameState.Gameplay);
        }

        public void ReturnToMainMenu()
        {
            _worldManager.ClearWorld();

            if (_inventory != null)
                _inventory.Clear();

            if (_playerController != null)
                _playerController.enabled = false;

            if (_player != null)
                _player.position = _playerStartPosition;

            if (_playerController != null)
                _playerController.enabled = true;

            SetState(GameState.MainMenu);
        }

        public void QuitGame()
        {
            Application.Quit();

#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }

        private void SetState(GameState state)
        {
            CurrentState = state;

            Time.timeScale = state == GameState.Paused ? 0f : 1f;

            bool gameplay = state == GameState.Gameplay;

            Cursor.lockState = gameplay ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = !gameplay;

            StateChanged?.Invoke(state);
        }
    }
}