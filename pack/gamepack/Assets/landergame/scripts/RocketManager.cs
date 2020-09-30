using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; 

public class RocketManager : MonoBehaviour
{

    public RocketLander player;
    public Button startButton;
    public Button restartButton;

    public GameObject shipExplo;
    public GameObject effectExplo;
    public AudioSource source;


    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<RocketLander>();
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.R))
        {
            RestartScene();

        }
        if (Input.GetKeyUp(KeyCode.D))
        {
            Explosion();

        }
        if (player.crashed)
        {
            
            Explosion();
            player.ResetCrashed(); 
            
        }
        if(player.Touch)
        {
            StartCoroutine(WaitandButton(2));
        }
        
    }


    public void RestartScene()
    {
        Scene scene = SceneManager.GetActiveScene();

        SceneManager.LoadScene(scene.name);

    }

    public void PressButton()
    {
        player.PressButton(); 

    }

    public void ReleaseButton()
    {
        player.ReleaseButton();

    }

    public void StartGame()
    {
        player.StartGame();
        startButton.gameObject.SetActive(false); 

    }


    void Explosion()
    {
        player.gameObject.SetActive(false);
        Instantiate(shipExplo, player.transform.position, Quaternion.identity);
        Instantiate(effectExplo, player.transform.position, Quaternion.identity);
        source.Play(); 
        //shipExplo.GetComponent<ExplodingShip>().Explode(); 

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
}
