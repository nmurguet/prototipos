using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; 

public class CannonGameManager : MonoBehaviour
{
    public CanonTargeting player;

    public Button restartButton;


    public int shots;

    private bool canShoot; 


    

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindObjectOfType<CanonTargeting>();
        canShoot = true; 
    }

    // Update is called once per frame
    void Update()
    {
        if(shots==0)
        {
            canShoot = false; 
            StartCoroutine(WaitandButton(2)); 
        }
    }


    public void PressButton()
    {
        if(canShoot)
        { 
        player.PressButton();
        shots -= 1;
        }
    }

    public void RestartScene()
    {
        Scene scene = SceneManager.GetActiveScene();

        SceneManager.LoadScene(scene.name);

    }

    IEnumerator WaitandButton(int seconds)
    {
        yield return new WaitForSeconds(seconds);
        restartButton.gameObject.SetActive(true);

    }

    public void ExitGame()
    {


        SceneManager.LoadScene("menu");

    }
}
