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
    [SerializeField] private Animator _anim;
    [SerializeField] private GameObject _playerModel;
 
    private float _yVelocity;
    private float _rotationSpeed;
    private bool _ledgeGrabbed = false;
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
        _anim.SetFloat("Speed", Mathf.Abs(move));

        if (!_ledgeGrabbed)
        {
            if (!_controller.isGrounded)
            {
                _yVelocity -= _gravityForce * Time.deltaTime;
            }

            _velocity.y = _yVelocity;
            RotateModel(move);
            _controller.Move(_velocity * Time.deltaTime);
        }

        if (_controller.isGrounded)
            _anim.SetBool("Jump", false);
    }

    private void RotateModel(float movement)
    {
        float rotation;

        if (movement < 0)
        {
            rotation = Mathf.SmoothDampAngle(_playerModel.transform.eulerAngles.y, 180, ref _rotationSpeed, 0.1f);
            _playerModel.transform.rotation = Quaternion.Euler(0, rotation, 0);
        }
        else if (movement > 0)
        {
            rotation = Mathf.SmoothDampAngle(_playerModel.transform.eulerAngles.y, 0, ref _rotationSpeed, 0.1f);
            _playerModel.transform.rotation = Quaternion.Euler(0, rotation, 0);
        }       
    }

    private void Jump(InputAction.CallbackContext context)
    {
        if (_controller.isGrounded)
        {
            _yVelocity = _jumpForce;
            _anim.SetBool("Jump", true);
        }  
    }

    public void GrabLedge(Vector3 ledgeAnchor)
    {
        if (!_controller.isGrounded)
        {
            _anim.SetBool("LedgeGrab", true);
            _ledgeGrabbed = true;
            _yVelocity = 0;
            _controller.enabled = false;
            transform.position = ledgeAnchor;
        }
    }
}
