using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyController : MonoBehaviour
{

    public GameController gc; 
    // Start is called before the first frame update
    void Start()
    {
        gc = FindObjectOfType<GameController>(); 
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Q))
        {
            gc.Reset();    

        }

        

        if (Input.GetKeyUp(KeyCode.R))
        {
            gc.RestartScene(); 

        }

                              
        if (Input.GetKeyDown(KeyCode.A))
        {
            gc.PressGas(); 
        }
        if (Input.GetKeyUp(KeyCode.A))
        {
            gc.LiftGas(); 
        }
    }
}
