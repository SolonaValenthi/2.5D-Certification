using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GemDoor : MonoBehaviour
{
    [SerializeField] private int _reqGems;
    [SerializeField] private float _flashTime;
    [SerializeField] private MeshRenderer _lightRenderer;

    private bool _isOpen = false;

    Animator _anim;
    BoxCollider[] _colliders;
    WaitForSeconds _flash;

    // Start is called before the first frame update
    void Start()
    {
        _anim = GetComponent<Animator>();
        _colliders = GetComponents<BoxCollider>();
        _flash = new WaitForSeconds(_flashTime);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !_isOpen)
        {
            Player player = other.GetComponent<Player>();
            CheckGems(player);
        }
    }

    private void CheckGems(Player player)
    {
        if (player.GetGemCount() >= _reqGems)
            OpenDoor();
        else
            DoorError();
    }

    private void OpenDoor()
    {
        _anim.SetTrigger("OpenDoor");

        foreach (var collider in _colliders)
            collider.enabled = false;

        StartCoroutine(SuccessFlash());
    }

    private void DoorError()
    {
        StopCoroutine("ErrorFlash");
        StartCoroutine("ErrorFlash");
    }

    IEnumerator SuccessFlash()
    {
        StopCoroutine("ErrorFlash");

        for (int i = 0; i < 3; i++)
        {
            _lightRenderer.material.SetColor("_EmissionColor", Color.black);
            yield return _flash;
            _lightRenderer.material.SetColor("_EmissionColor", Color.green);
            yield return _flash;
        }
    }

    IEnumerator ErrorFlash()
    {
        for (int i = 0; i < 3; i++)
        {
            _lightRenderer.material.SetColor("_EmissionColor", Color.black);
            yield return _flash;
            _lightRenderer.material.SetColor("_EmissionColor", Color.red);
            yield return _flash;
        }

        _lightRenderer.material.SetColor("_EmissionColor", Color.black);
        yield return _flash;
        _lightRenderer.material.SetColor("_EmissionColor", Color.cyan);
        yield return _flash;
    }
}
