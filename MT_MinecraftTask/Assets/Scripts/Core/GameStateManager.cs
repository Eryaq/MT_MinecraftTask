using System;
using UnityEngine;

namespace MT_MiencraftTask.Core
{
    public class GameStateManager : MonoBehaviour
    {
        public event Action<GameState> StateChanged;

        public GameState CurrentState { get; private set; }

        private void Start()
        {
            SetState(GameState.MainMenu);
        }

        public void StartGame()
        {
            SetState(GameState.Loading);
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