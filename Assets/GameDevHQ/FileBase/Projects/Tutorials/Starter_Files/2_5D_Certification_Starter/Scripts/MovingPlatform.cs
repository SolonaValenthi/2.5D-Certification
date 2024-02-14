using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [SerializeField] private Vector3[] _targetPoint;
    [SerializeField] private float _moveSpeed;
    [SerializeField] private bool _looping;

    private int _lastPoint = 0;
    private int _nextPoint = 1;
    private bool _returning = false;

    // Start is called before the first frame update
    void Start()
    {
        transform.position = _targetPoint[0];
    }

    void FixedUpdate()
    {
        MoveToPoint();
    }

    private void MoveToPoint()
    {
        transform.position = Vector3.MoveTowards(transform.position, _targetPoint[_nextPoint], _moveSpeed * Time.deltaTime);

        if (transform.position == _targetPoint[_nextPoint])
        {
            _lastPoint = _nextPoint;

            if (_looping || _targetPoint.Length == 2)
            {
                if (_lastPoint == _targetPoint.Length - 1)
                    _nextPoint = 0;
                else
                    _nextPoint++;
            }
            else
            {
                if (_lastPoint == _targetPoint.Length - 1)
                    _returning = true;
                else if (_lastPoint == 0)
                    _returning = false;

                if (_returning)
                    _nextPoint--;
                else
                    _nextPoint++;
            }
        }
    }
}
