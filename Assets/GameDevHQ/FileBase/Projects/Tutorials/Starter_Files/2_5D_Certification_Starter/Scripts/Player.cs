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
    [SerializeField] private Vector3 _climbingOffset;
 
    private float _yVelocity;
    private float _rotationSpeed;
    private bool _ledgeGrabbed = false;
    private bool _canTurn = true;
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
        _input.Player.Climb.performed += ClimbUp;
    }

    // Update is called once per frame
    void Update()
    {
        CalculateMovement();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Elevator") || other.CompareTag("MovingPlatform"))
            transform.parent = other.transform;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Elevator") || other.CompareTag("MovingPlatform"))
            transform.parent = null;
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

        if (_canTurn)
        {
            if (movement < 0)
            {
                rotation = Mathf.SmoothDampAngle(_playerModel.transform.eulerAngles.y, 180, ref _rotationSpeed, 0.05f);
                _playerModel.transform.rotation = Quaternion.Euler(0, rotation, 0);
            }
            else if (movement > 0)
            {
                rotation = Mathf.SmoothDampAngle(_playerModel.transform.eulerAngles.y, 0, ref _rotationSpeed, 0.05f);
                _playerModel.transform.rotation = Quaternion.Euler(0, rotation, 0);
            }
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

    private void ClimbUp(InputAction.CallbackContext context)
    {
        if (_ledgeGrabbed)
        {
            _ledgeGrabbed = false;
            _anim.SetBool("LedgeGrab", false);
            _anim.SetTrigger("ClimbUp");
        }
    }

    public void GrabLedge(Vector3 ledgeAnchor)
    {
        if (!_controller.isGrounded)
        {
            _ledgeGrabbed = true;
            _canTurn = false;
            _anim.SetBool("LedgeGrab", _ledgeGrabbed);
            _anim.SetBool("Jump", false);
            _anim.SetFloat("Speed", 0.0f);
            _yVelocity = 0;
            _controller.enabled = false;
            transform.position = ledgeAnchor;
        }
    }

    public void ClimbUpCompleted()
    {
        transform.position += _climbingOffset;
        _controller.enabled = true;
        _canTurn = true;
    }
}