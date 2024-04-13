using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    private GameManager gameManager;
    public float gameTimer;

    [Header("Inventário")]
    public Inventario[] inventario;


    void Awake()
    {
        if (gameManager == null)
        {
            gameManager = this;
        }
        else
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);

    }

    private void Update()
    {
        gameTimer += 1 * Time.deltaTime;
    }

    public void SetInventario(string inventarioName, int qtdFruit, int qtdSeeds)
    {

        for(int i = 0; i < inventario.Length; i++)
        {
            if(inventarioName == inventario[i].nome)
            {
                inventario[i].qtdFrutos += qtdFruit;
                inventario[i].qtdSementes += qtdSeeds;
                return;
            }
        }

    }

    public int GetSeeds(string inventarioName)
    {

        for (int i = 0; i < inventario.Length; i++)
        {
            if (inventarioName == inventario[i].nome)
            {
                return(inventario[i].qtdSementes);
                
            }
        }

        return 0;

    }

    public int GetFruit(string inventarioName)
    {

        for (int i = 0; i < inventario.Length; i++)
        {
            if (inventarioName == inventario[i].nome)
            {
                return (inventario[i].qtdFrutos);

            }
        }

        return 0;

    }

    public PlantData GetPlantData(string inventarioName)
    {

        for (int i = 0; i < inventario.Length; i++)
        {
            if (inventarioName == inventario[i].nome)
            {
                return (inventario[i].plantData);

            }
        }

        return null;

    }

}

[Serializable]
public class Inventario
{

    public string nome;
    public int qtdFrutos;
    public int qtdSementes;
    public PlantData plantData;

}
