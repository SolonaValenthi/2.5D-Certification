using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LedgeChecker : MonoBehaviour
{
    [SerializeField] private Vector3 _grabAnchor;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("LedgeCheck"))
        {
            Player player = other.transform.GetComponentInParent<Player>();
            player.GrabLedge(_grabAnchor);
        }
    }
}