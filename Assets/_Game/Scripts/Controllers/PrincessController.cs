using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrincessController : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    private string _cheeerAnim = "Cheer", _sadAnim = "Sad";

    private void Start()
    {
        PlaySadAnim();
    }

    public void MoveTo(Vector3 pos)
    {
        transform.position = pos;
    }

    private void PlaySadAnim()
    {
        _animator.CrossFade(_sadAnim, 0, 0);
    }

    public void PlayCheerAnim()
    {
        _animator.CrossFade(_cheeerAnim, 0, 0);
    }
}