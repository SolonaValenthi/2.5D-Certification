using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ladder : MonoBehaviour
{
    [SerializeField] private GameObject _ladderAnchor;

    public Vector3 GetAnchor()
    {
        return _ladderAnchor.transform.position;
    }
}
