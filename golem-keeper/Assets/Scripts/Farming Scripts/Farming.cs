using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Farming : MonoBehaviour
{
    public GameObject select;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Select(bool toggle) 
    { 
    
        select.SetActive(toggle);
    
    }

}
