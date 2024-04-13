using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName ="Plant")]
public class PlantData : ScriptableObject
{

    [Header("Plant Settings")]
    public string namePlant;
    [TextArea(2, 10)]
    public string plantDescription;
    public Sprite thumbPlant;
    public GameObject gameModelPlant;

    [Header("Seed Settings")]
    public Sprite thumbSeed;
    public GameObject gameModelSeed;
    public float growTime;

}