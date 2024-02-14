using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LedgeChecker : MonoBehaviour
{
    [SerializeField] private GameObject _grabAnchor;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("LedgeCheck"))
        {
            Player player = other.transform.GetComponentInParent<Player>();
            player.GrabLedge(_grabAnchor.transform.position);
        }
    }
}