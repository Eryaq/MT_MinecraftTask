using UnityEngine;
using UnityEngine.InputSystem;

namespace MT_MiencraftTask.Player
{
    public class CameraBob : MonoBehaviour
    {
        [SerializeField] private CharacterController _controller;
        [SerializeField] private Transform _cameraRoot;

        [SerializeField] private InputActionReference _moveAction;

        [Header("Bob Settings")]
        [SerializeField] private float _bobFrequency = 8f;
        [SerializeField] private float _bobAmplitude = 0.035f;
        [SerializeField] private float _returnSpeed = 10f;

        private Vector3 _defaultLocalPosition;
        private float _bobTimer;

        private void Awake()
        {
            _defaultLocalPosition = _cameraRoot.localPosition;
        }

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
            if (_controller == null || _cameraRoot == null)
                return;

            Vector2 moveInput = _moveAction.action.ReadValue<Vector2>();

            bool isMoving = moveInput.sqrMagnitude > 0.1f;
            bool isGrounded = _controller.isGrounded;

            if (isMoving && isGrounded)
            {
                _bobTimer += Time.deltaTime * _bobFrequency;

                float bobOffset = Mathf.Sin(_bobTimer) * _bobAmplitude;

                Vector3 targetPosition = _defaultLocalPosition + new Vector3(0f, bobOffset, 0f);
                _cameraRoot.localPosition = Vector3.Lerp(_cameraRoot.localPosition, targetPosition, Time.deltaTime * _returnSpeed);
            }
            else
            {
                _bobTimer = 0f;

                _cameraRoot.localPosition = Vector3.Lerp(_cameraRoot.localPosition, _defaultLocalPosition, Time.deltaTime * _returnSpeed);
            }
        }
    }
}
