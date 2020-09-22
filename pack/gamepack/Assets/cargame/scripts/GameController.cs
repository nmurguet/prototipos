using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cinemachine;

public class GameController : MonoBehaviour
{

    public car player;
    public KeyController keyboard;


    public int activePlayer;

    public List<GameObject> players; 

    public CinemachineVirtualCamera vc; 

    private void Awake()
    {
        if (!PlayerPrefs.HasKey("activePlayer"))
        {
            PlayerPrefs.SetInt("activePlayer", 0);

        }

        

        activePlayer = PlayerPrefs.GetInt("activePlayer");
        
        


    }

    // Start is called before the first frame update
    void Start()
    {

        players[activePlayer].SetActive(true);
        player = FindObjectOfType<car>();

        keyboard = FindObjectOfType<KeyController>();
        vc.Follow = player.transform;
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void PressGas()
    {

        player.PressGas();
    

    }
    public void LiftGas()
    {
        player.LiftGas();
    }

    public void Reset()
    {
        player.Reset();
       
    }

    public void RestartScene()
    {
        Scene scene = SceneManager.GetActiveScene();

        SceneManager.LoadScene(scene.name);

    }

    public void ExitGame()
    {


        SceneManager.LoadScene("MainMenuCar");

    }
}
