using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonSelectSeed : MonoBehaviour
{

    public void SetSeed()
    {

        Player player = FindFirstObjectByType<Player>();

        player.seedSelect = gameObject.name;

        Time.timeScale = 1;

        player.InteractFarm();

    }

}
