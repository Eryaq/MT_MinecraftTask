using UnityEngine;
using UnityEngine.InputSystem;

namespace MT_MiencraftTask.Player
{
    [RequireComponent(typeof(CharacterController))]
    public class FirstPersonController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform _cameraRoot;

        [Header("Input")]
        [SerializeField] private InputActionReference _moveAction;
        [SerializeField] private InputActionReference _lookAction;
        [SerializeField] private InputActionReference _jumpAction;

        [Header("Movement")]
        [SerializeField] private float _moveSpeed = 6f;
        [SerializeField] private float _gravity = -20f;
        [SerializeField] private float _jumpHeight = 1.25f;

        [Header("Look")]
        [SerializeField] private float _mouseSensitivity = 0.08f;
        [SerializeField] private float _minPitch = -85f;
        [SerializeField] private float _maxPitch = 85f;

        private CharacterController _controller;
        private Vector3 _verticalVelocity;
        private float _pitch;

        private void Awake()
        {
            _controller = GetComponent<CharacterController>();

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void OnEnable()
        {
            _moveAction.action.Enable();
            _lookAction.action.Enable();
            _jumpAction.action.Enable();
        }

        private void OnDisable()
        {
            _moveAction.action.Disable();
            _lookAction.action.Disable();
            _jumpAction.action.Disable();
        }

        private void Update()
        {
            HandleLook();
            HandleMovement();
        }

        private void HandleLook()
        {
            Vector2 lookInput = _lookAction.action.ReadValue<Vector2>();

            float mouseX = lookInput.x * _mouseSensitivity;
            float mouseY = lookInput.y * _mouseSensitivity;

            transform.Rotate(Vector3.up * mouseX);

            _pitch -= mouseY;
            _pitch = Mathf.Clamp(_pitch, _minPitch, _maxPitch);

            _cameraRoot.localRotation = Quaternion.Euler(_pitch, 0f, 0f);
        }

        private void HandleMovement()
        {
            bool isGrounded = _controller.isGrounded;

            if (isGrounded && _verticalVelocity.y < 0f)
                _verticalVelocity.y = -2f;

            Vector2 moveInput = _moveAction.action.ReadValue<Vector2>();

            Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;

            move = Vector3.ClampMagnitude(move, 1f);

            _controller.Move(move * (_moveSpeed * Time.deltaTime));

            if (isGrounded && _jumpAction.action.WasPressedThisFrame())
            {
                _verticalVelocity.y = Mathf.Sqrt(_jumpHeight * -2f * _gravity);
            }

            _verticalVelocity.y += _gravity * Time.deltaTime;
            _controller.Move(_verticalVelocity * Time.deltaTime);
        }
    }
}
