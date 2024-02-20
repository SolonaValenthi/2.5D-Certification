using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gem : MonoBehaviour
{
    [SerializeField] private MeshRenderer _mesh;
    [SerializeField] private SphereCollider _collider;
    [SerializeField] private ParticleSystem _emitter;

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.right / 2);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Player player = other.GetComponent<Player>();
            player.CollectGem();
            DestroyGem();
        }
    }

    private void DestroyGem()
    {
        _mesh.enabled = false;
        _collider.enabled = false;
        _emitter.Stop(false, ParticleSystemStopBehavior.StopEmitting);
        Destroy(this.gameObject, 1.0f);
    }
}
