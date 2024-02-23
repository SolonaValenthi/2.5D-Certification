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

    private int _gemCount = 0;
    private float _yVelocity;
    private float _rollSpeed;
    private float _move;
    private float _standingHeight = 1.8f;
    private float _rollingHeight = 0.8f;
    private bool _nearLadder = false;
    private bool _onLadder = false;
    private bool _atBottom = false;
    private bool _atTop = false;
    private bool _ledgeGrabbed = false;
    private bool _canTurn = true;
    private bool _rolling = false;
    private bool _faceRight = true;
    private PlayerInputActions _input;
    
    Vector3 _velocity;
    Vector3 _spawnPoint;
    Vector3 _standingOffset = new Vector3(0, 0.9f, 0);
    Vector3 _rollingCenter = new Vector3(0, 0.4f, 0);
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
        SetSpawn(transform.position);
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

        if (other.CompareTag("LadderBottom"))
            _atBottom = true;

        if (other.CompareTag("LadderTop"))
            _atTop = true;

        if (other.CompareTag("KillZone"))
            Respawn();
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Elevator") || other.CompareTag("MovingPlatform"))
            transform.parent = null;

        if (other.CompareTag("Ladder"))
            _nearLadder = false;

        if (other.CompareTag("LadderBottom"))
            _atBottom = false;

        if (other.CompareTag("LadderTop"))
            _atTop = false;
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
            _controller.height = _rollingHeight;
            _controller.center = _rollingCenter;

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
        transform.position = ladder.GetLadderAnchor();
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

        if (_atBottom && move < 0)
        {
            _anim.SetBool("OnLadder", false);
            _onLadder = false;
        }

        if (_atTop && move > 0)
        {
            _controller.enabled = false;
            transform.position = _ladder.GetClimbAnchor();
            _anim.SetTrigger("ClimbUp");
            _canTurn = false;
            _onLadder = false;
        }
    }

    private void Respawn()
    {
        _controller.enabled = false;
        transform.position = _spawnPoint;
        _controller.enabled = true;
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
        if (_faceRight)
            transform.position += _climbingOffset;
        else
            transform.position += new Vector3(0, _climbingOffset.y, -_climbingOffset.z);

        _anim.SetBool("OnLadder", false);
        _controller.enabled = true;
        _atTop = false;
        _canTurn = true;
    }

    public void RollCompleted()
    {
        _canTurn = true;
        _rolling = false;
        _rollSpeed = 0;
        _controller.height = _standingHeight;
        _controller.center = _standingOffset;
    }

    public void CollectGem()
    {
        _gemCount++;
    }

    public int GetGemCount()
    {
        return _gemCount;
    }

    public void SetSpawn(Vector3 newSpawn)
    {
        _spawnPoint = newSpawn;
    }

    private void OnDisable()
    {
        _input.Player.Jump.performed -= Jump;
        _input.Player.Climb.performed -= ClimbUp;
        _input.Player.Roll.performed -= Roll;
    }
}