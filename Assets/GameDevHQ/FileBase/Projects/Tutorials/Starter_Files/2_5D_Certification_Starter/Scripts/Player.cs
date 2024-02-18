using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField] private float _moveSpeed;
    [SerializeField] private float _climbSpeed;
    [SerializeField] private float _jumpForce;
    [SerializeField] private float _gravityForce;
    [SerializeField] private Animator _anim;
    [SerializeField] private GameObject _playerModel;
    [SerializeField] private Vector3 _climbingOffset;
 
    private float _yVelocity;
    private float _rollSpeed;
    private float _move;
    private bool _nearLadder = false;
    private bool _onLadder = false;
    private bool _ledgeGrabbed = false;
    private bool _canTurn = true;
    private bool _rolling = false;
    private bool _faceRight = true;
    private PlayerInputActions _input;
    
    Vector3 _velocity;
    CharacterController _controller;
    Ladder _ladder;

    // Start is called before the first frame update
    void Start()
    {
        _controller = GetComponent<CharacterController>();
        _input = new PlayerInputActions();
        _input.Player.Enable();
        _input.Player.Jump.performed += Jump;
        _input.Player.Climb.performed += ClimbUp;
        _input.Player.Roll.performed += Roll;
    }

    // Update is called once per frame
    void Update()
    {
        if (_onLadder)
            LadderMovement();
        else
            CalculateMovement();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Elevator") || other.CompareTag("MovingPlatform"))
            transform.parent = other.transform;

        if (other.CompareTag("Ladder"))
        {
            _ladder = other.GetComponent<Ladder>();
            _nearLadder = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Elevator") || other.CompareTag("MovingPlatform"))
            transform.parent = null;

        if (other.CompareTag("Ladder"))
            _nearLadder = false;
    }

    private void CalculateMovement()
    {
        if (!_rolling)
        {
            _move = _input.Player.Movement.ReadValue<float>();
            _velocity = new Vector3(0, 0, _move) * _moveSpeed;
            _anim.SetFloat("Speed", Mathf.Abs(_move));
        }

        if (!_ledgeGrabbed)
        {
            if (!_controller.isGrounded)
            {
                _yVelocity -= _gravityForce * Time.deltaTime;
            }

            _velocity.y = _yVelocity;
            RotateModel(_move);
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
                rotation = 180;
                _playerModel.transform.rotation = Quaternion.Euler(0, rotation, 0);
                _faceRight = false;
            }
            else if (movement > 0)
            {
                rotation = 0;
                _playerModel.transform.rotation = Quaternion.Euler(0, rotation, 0);
                _faceRight = true;
            }
        }
    }

    private void Jump(InputAction.CallbackContext context)
    {
        if (_controller.isGrounded && !_rolling)
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
        else if (_nearLadder && !_onLadder)
        {
            LadderGrab(_ladder);
        }
    }

    private void Roll(InputAction.CallbackContext context)
    {
        if (_controller.isGrounded && !_rolling)
        {
            _canTurn = false;
            _rolling = true;
            _anim.SetTrigger("Roll");

            if (_faceRight)
                _rollSpeed = _moveSpeed;
            else
                _rollSpeed = -_moveSpeed;

            _velocity.z = _rollSpeed;
        }
    }

    private void LadderGrab(Ladder ladder)
    {
        _controller.enabled = false;
        transform.position = ladder.GetAnchor();
        _onLadder = true;
        _anim.SetBool("OnLadder", true);
        _controller.enabled = true;
    }

    private void LadderMovement()
    {
        float move = _input.Player.LadderMovement.ReadValue<float>();
        Vector3 velocity = new Vector3(0, move, 0) * _climbSpeed;
        _anim.SetFloat("LadderSpeed", move);
        _controller.Move(velocity * Time.deltaTime);
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

    public void RollCompleted()
    {
        _canTurn = true;
        _rolling = false;
        _rollSpeed = 0;
    }
}