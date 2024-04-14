using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class Combo
{

    public Hit[] hits;

}

[Serializable]
public class Hit
{
    public string animation;
    public string inputButton;
    public float animationTime;
    public float resetTime;

    public int damage;
    public AudioClip hitSound;
}