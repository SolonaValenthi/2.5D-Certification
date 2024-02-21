using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GemDoor : MonoBehaviour
{
    [SerializeField] private int _reqGems;
    [SerializeField] private float _flashTime;
    [SerializeField] private SpriteRenderer _gemSprite;
    [SerializeField] private MeshRenderer _lightRenderer;
    [SerializeField] private TextMeshPro _countText;

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
        _countText.text = $"{player.GetGemCount()} / {_reqGems}";

        if (player.GetGemCount() >= _reqGems)
            OpenDoor();
        else
            DoorError();
    }

    private void OpenDoor()
    {
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

        _countText.enabled = true;
        _gemSprite.enabled = true;
        _countText.color = Color.green;
        _gemSprite.color = Color.green;

        for (int i = 0; i < 3; i++)
        {
            _lightRenderer.material.SetColor("_EmissionColor", Color.black);
            yield return _flash;
            _lightRenderer.material.SetColor("_EmissionColor", Color.green);
            yield return _flash;
        }

        _countText.enabled = false;
        _gemSprite.enabled = false;
        _anim.SetTrigger("OpenDoor");
    }

    IEnumerator ErrorFlash()
    {
        _countText.enabled = true;
        _gemSprite.enabled = true;
        _countText.color = Color.red;
        _gemSprite.color = Color.red;

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
        _countText.enabled = false;
        _gemSprite.enabled = false;
    }
}
