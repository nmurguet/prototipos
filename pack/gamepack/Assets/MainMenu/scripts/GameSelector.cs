﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSelector : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void CarGame()
    {

        SceneManager.LoadScene("MainMenuCar");
    }

    public void ExitGame()
    {
        Application.Quit();

    }
}
