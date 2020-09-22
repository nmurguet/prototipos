using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelSelect : MonoBehaviour
{

    public Image sr;

    public int activePlayer;
    public int max_player;
    public int index;

    public List<GameObject> players;

    // Start is called before the first frame update
    void Start()
    {
        if (!PlayerPrefs.HasKey("activePlayer"))
        {
            PlayerPrefs.SetInt("activePlayer", 0);

        }

        activePlayer = PlayerPrefs.GetInt("activePlayer");
        index = activePlayer;
    }

    // Update is called once per frame
    void Update()
    {
        if (index == 0)
        {
            
            sr.sprite = players[index].GetComponent<SpriteRenderer>().sprite;
        }
        else if (index == 1)
        {
            sr.sprite = players[index].GetComponent<SpriteRenderer>().sprite;
        }
        else if (index == 2)
        {
            sr.sprite = players[index].GetComponent<SpriteRenderer>().sprite;
        }
        else
        {
            sr.sprite = players[index].GetComponent<SpriteRenderer>().sprite;
        }
    } 

    public void PressLeft()
    {
        index = (index + players.Count - 1) % players.Count;
        PlayerPrefs.SetInt("activePlayer", index);

    }

    public void PressRight()
    {

        index = (index + 1) % players.Count;
        PlayerPrefs.SetInt("activePlayer", index);
    }

    public void Select3()
    {

       PlayerPrefs.SetInt("activePlayer", 2);
    }


    public void GameOn()
    {
        SceneManager.LoadScene("proto1");

    }

    public void ExitMenu()
    {
        SceneManager.LoadScene("menu");

    }
}
