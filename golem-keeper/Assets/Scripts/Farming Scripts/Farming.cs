using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FARMSTATE
{
    VAZIO,
    PLANTADO,
    COLHEITA,
    SECO
}

public class Farming : MonoBehaviour
{

    public GameObject select;
    public GameObject plant;
    public FARMSTATE state = FARMSTATE.VAZIO;
    public PlantData plantData = null;

    GameManager gameManager;    
    float initialTime = 0f;


    private void Start()
    {
        gameManager = FindFirstObjectByType<GameManager>();
    }

    void Update()
    {
        switch (state) 
        
        { 
        
            case FARMSTATE.VAZIO:
                break;

            case FARMSTATE.PLANTADO:
                Growing();
                break;

            case FARMSTATE.COLHEITA:
                break;

            case FARMSTATE.SECO:
                Destroy(plant);
                state = FARMSTATE.VAZIO;
                break;
        
        }

    }

    public void Select(bool toggle) 
    { 
    
        select.SetActive(toggle);
    
    }

    public void Seeding(string plantName)
    {
        int qtdSeed = gameManager.GetSeeds(plantName);

        if (qtdSeed > 0)
        {
            gameManager.SetInventario(plantName, 0, -1);
            state = FARMSTATE.PLANTADO;
            plantData = gameManager.GetPlantData(plantName);
            plant = Instantiate(plantData.gameModelPlant, plant.transform.position, plant.transform.rotation);
            plant.GetComponent<SkinnedMeshRenderer>().SetBlendShapeWeight(0, 100);
            plant.SetActive(true);
            initialTime = gameManager.gameTimer;

        }
    }

    void Growing()
    {
        float crescimento = (gameManager.gameTimer - initialTime) /  plantData.growTime;
        
        if (crescimento <= 1)
        {
            plant.GetComponent<SkinnedMeshRenderer>().SetBlendShapeWeight(0, 100 - Mathf.Round(crescimento * 100));

        }
        else
        {
            state = FARMSTATE.COLHEITA;
        }

    }

}
