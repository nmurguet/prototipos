using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Manager : MonoBehaviour
{

    public Transform spawn;

    public GameObject player;


    public string levelname;
    public string indexstr;
    public int index;

    public string nextLevel;

    public GameObject winPanel;


    // Start is called before the first frame update
    void Start()
    {

        winPanel = GameObject.FindGameObjectWithTag("winPanel");


        player.transform.position = spawn.position;

        levelname = SceneManager.GetActiveScene().name;
        indexstr = levelname.Substring(levelname.Length - 1);

        int.TryParse(indexstr, out index);

        index = index + 1;

        nextLevel = "level" + index;
        winPanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Respawn()
    {

        player.transform.position = spawn.position;
        player.GetComponent<PlayerMovement>().Respawn();

    }


    public void OnRestart()
    {
        Scene scene = SceneManager.GetActiveScene();

        SceneManager.LoadScene(scene.name);


    }

    public void LevelClear()
    {
        player.GetComponent<PlayerMovement>().SetFreeze(true);
        winPanel.SetActive(true);
        //SceneManager.LoadScene(nextLevel);


    }

    public void OnNextLevel()
    {
        SceneManager.LoadScene(nextLevel);
    }

}
