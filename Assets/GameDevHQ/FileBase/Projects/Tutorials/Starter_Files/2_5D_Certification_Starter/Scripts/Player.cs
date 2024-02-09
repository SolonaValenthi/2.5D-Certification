using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField] private float _moveSpeed;
    [SerializeField] private float _jumpForce;
    [SerializeField] private float _gravityForce;

    private float _yVelocity;
    private PlayerInputActions _input;
    
    Vector3 _velocity;
    CharacterController _controller;

    // Start is called before the first frame update
    void Start()
    {
        _controller = GetComponent<CharacterController>();
        _input = new PlayerInputActions();
        _input.Player.Enable();
        _input.Player.Jump.performed += Jump;
    }

    // Update is called once per frame
    void Update()
    {
        CalculateMovement();
    }

    private void CalculateMovement()
    {
        float move = _input.Player.Movement.ReadValue<float>();
        _velocity = new Vector3(0, 0, move) * _moveSpeed;

        if (!_controller.isGrounded)
        {
            _yVelocity -= _gravityForce * Time.deltaTime;
        }

        _velocity.y = _yVelocity;
        _controller.Move(_velocity * Time.deltaTime);
    }

    private void Jump(InputAction.CallbackContext context)
    {
        if (_controller.isGrounded)
            _yVelocity = _jumpForce;
    }
}
