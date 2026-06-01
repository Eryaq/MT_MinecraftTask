using UnityEngine;
using UnityEngine.InputSystem;

namespace MT_MinecraftTask.Player
{
    public class FootstepAudio : MonoBehaviour
    {
        [SerializeField] private CharacterController _controller;
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private AudioClip[] _footstepClips;
        [SerializeField] private InputActionReference _moveAction;

        [Header("Timing")]
        [SerializeField] private float _stepInterval = 0.45f;

        private float _stepTimer;

        private void OnEnable()
        {
            _moveAction.action.Enable();
        }

        private void OnDisable()
        {
            _moveAction.action.Disable();
        }

        private void Update()
        {
            Vector2 moveInput = _moveAction.action.ReadValue<Vector2>();

            bool isMoving = moveInput.sqrMagnitude > 0.1f;
            bool isGrounded = _controller.isGrounded;

            if (!isMoving || !isGrounded)
            {
                _stepTimer = 0f;
                return;
            }

            _stepTimer += Time.deltaTime;

            if (_stepTimer >= _stepInterval)
            {
                PlayFootstep();
                _stepTimer = 0f;
            }
        }

        private void PlayFootstep()
        {
            if (_footstepClips == null || _footstepClips.Length == 0)
                return;

            AudioClip clip = _footstepClips[Random.Range(0, _footstepClips.Length)];

            _audioSource.PlayOneShot(clip);
        }
    }
}
