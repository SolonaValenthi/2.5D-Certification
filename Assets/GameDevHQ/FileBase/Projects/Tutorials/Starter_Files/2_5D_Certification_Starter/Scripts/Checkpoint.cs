using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] private GameObject _spawnAnchor;
    [SerializeField] private MeshRenderer _lightRenderer;

    BoxCollider _collider;

    void Start()
    {
        _collider = GetComponent<BoxCollider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Player player = other.GetComponent<Player>();
            player.SetSpawn(_spawnAnchor.transform.position);
            _lightRenderer.material.SetColor("_EmissionColor", Color.green);
        }
    }
}
