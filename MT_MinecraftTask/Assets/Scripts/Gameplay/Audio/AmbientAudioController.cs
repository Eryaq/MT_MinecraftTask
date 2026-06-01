using MT_MinecraftTask.Core;
using UnityEngine;

namespace MT_MinecraftTask.Audio
{
    public class AmbientAudioController : MonoBehaviour
    {
        [SerializeField] private GameStateManager _gameStateManager;
        [SerializeField] private AudioSource _audioSource;

        private void OnEnable()
        {
            _gameStateManager.StateChanged += OnStateChanged;
        }

        private void OnDisable()
        {
            _gameStateManager.StateChanged -= OnStateChanged;
        }

        private void OnStateChanged(GameState state)
        {
            switch (state)
            {
                case GameState.Gameplay:
                    if (!_audioSource.isPlaying)
                        _audioSource.Play();
                    break;

                case GameState.MainMenu:
                    _audioSource.Stop();
                    break;

                case GameState.Loading:
                    _audioSource.Stop();
                    break;

                case GameState.Paused:
                    break;
            }
        }
    }
}
