using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Elevator : MonoBehaviour
{
    [SerializeField] private GameObject _liftFloor;
    [SerializeField] private Vector3[] _stops;
    [SerializeField] private float _moveSpeed;

    private int _nextStop = 0;
    private bool _moveDown;
    private bool _canMove = true;

    void FixedUpdate()
    {
        if (_canMove)
            LiftMovement(_stops[_nextStop]);
    }

    private void LiftMovement(Vector3 moveTo)
    {
        moveTo = transform.TransformPoint(moveTo);
        _liftFloor.transform.position = Vector3.MoveTowards(_liftFloor.transform.position, moveTo, _moveSpeed * Time.deltaTime);

        if (_liftFloor.transform.position == moveTo)
            CheckFloor();
    }

    private void CheckFloor()
    {
        _canMove = false;
        StartCoroutine(WaitAtFloor());

        if (_nextStop == 0)
            _moveDown = true;
        else if (_nextStop == _stops.Length - 1)
            _moveDown = false;

        if (_moveDown)
            _nextStop++;
        else
            _nextStop--;
    }

    IEnumerator WaitAtFloor()
    {
        yield return new WaitForSeconds(5.0f);
        _canMove = true;
    }
}