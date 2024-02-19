using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ladder : MonoBehaviour
{
    [SerializeField] private GameObject _ladderAnchor;
    [SerializeField] private GameObject _climbAnchor;

    public Vector3 GetLadderAnchor()
    {
        return _ladderAnchor.transform.position;
    }

    public Vector3 GetClimbAnchor()
    {
        return _climbAnchor.transform.position;
    }
}
