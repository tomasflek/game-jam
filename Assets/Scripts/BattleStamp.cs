using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleStamp : MonoBehaviour
{
    private Animation _anim;
    private void OnEnable()
    {
        if (_anim == null)
            _anim = GetComponent<Animation>();
        _anim.Play();
    }
}
