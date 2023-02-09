using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartSentence : MonoBehaviour
{
    [SerializeField] Transform _player;
    [SerializeField] AudioSource _source;

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform == _player) _source.Play();
    }
}
