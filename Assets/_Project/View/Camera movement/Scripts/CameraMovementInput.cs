using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace View
{
    public class CameraMovementInput : MonoBehaviour
    {
        [SerializeField] private InputActionReference _moveInputActionReference;
        [SerializeField] private CameraMovement _cameraMovement;
        [SerializeField] private float _movementSpeed = 25f;

        public InputAction MoveInput => _moveInputActionReference.action;
        private Vector2 _inputDirection;

        private void Awake()
        {
            MoveInput.Enable();
        }

        private void OnEnable()
        {
            MoveInput.started += OnMoveInput;
            MoveInput.performed += OnMoveInput;
            MoveInput.canceled += OnMoveInput;
        }

        private void OnDisable()
        {
            MoveInput.started -= OnMoveInput;
            MoveInput.performed -= OnMoveInput;
            MoveInput.canceled -= OnMoveInput;
        }

        private void Update()
        {
            _cameraMovement.Move(_inputDirection * Time.deltaTime * _movementSpeed);
        }

        private void OnMoveInput(InputAction.CallbackContext context)
        {
            _inputDirection = context.action.ReadValue<Vector2>();
        }
    }
}
