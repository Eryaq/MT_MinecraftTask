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