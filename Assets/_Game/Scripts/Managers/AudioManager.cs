using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _fire, _metalHit, _eggHit, _stoneHit, _merge;

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    public void Play(AudioClip audioClip)
    {
        _audioSource.clip = audioClip;
        _audioSource.Play();
    }

    public void PlayFire()
    {
        _audioSource.clip = _fire;
        _audioSource.Play();
    }
    public void PlayMerge()
    {
        _audioSource.clip = _merge;
        _audioSource.Play();
    }
    public void PlayEggHit()
    {
        _audioSource.clip = _eggHit;
        _audioSource.Play();
    }
    public void PlayStoneHit()
    {
        _audioSource.clip = _stoneHit;
        _audioSource.Play();
    }
    public void PlayMetalHit()
    {
        _audioSource.clip = _metalHit;
        _audioSource.Play();
    }
}