using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; 

public class PlaneGameController : MonoBehaviour
{
    public PlaneController planeController;
    public Button startButton;
    public Button restartButton;

    public GameObject finger;

    public Text highscore;
    public Text tiempo;

    public int score;


    public float timer;


    private bool canPlay; 

    // Start is called before the first frame update
    void Start()
    {
        planeController = FindObjectOfType<PlaneController>();
        score = 0;
        highscore.text = "Puntos : 0";
        tiempo.text = "Tiempo : " + Mathf.CeilToInt(timer); 
        canPlay = false; 
    }

    // Update is called once per frame
    void Update()
    {
        if(canPlay)
        { 
        timer -= Time.deltaTime;
        tiempo.text = "Tiempo : " + Mathf.CeilToInt(timer);
               
        }
        if (timer < 0)
        {
            restartButton.gameObject.SetActive(true);
            canPlay = false;
            planeController.StopGame(); 

        }

        if (Input.GetKeyUp(KeyCode.R))
        {
            RestartScene();

        }
        
    }

    public void RestartScene()
    {
        Scene scene = SceneManager.GetActiveScene();

        SceneManager.LoadScene(scene.name);

    }

    public void PressButton()
    {
        planeController.PressButton();

    }

    public void ReleaseButton()
    {
        planeController.ReleaseButton();

    }

    public void StartGame()
    {
        planeController.StartGame();
        startButton.gameObject.SetActive(false);
        finger.gameObject.SetActive(false);
        canPlay = true; 

    }

    public void ExitGame()
    {


        SceneManager.LoadScene("menu");

    }

    IEnumerator WaitandButton(int seconds)
    {
        yield return new WaitForSeconds(seconds);
        restartButton.gameObject.SetActive(true);

    }

    public void ScoreToAdd(int value)
    {
        if (canPlay) { 
        score += value;
        highscore.text = "Puntos : " + score;
        }
    }
}
