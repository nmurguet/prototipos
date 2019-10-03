using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{

    public Transform spawn;

    public GameObject player; 


    // Start is called before the first frame update
    void Start()
    {
        
        
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

}
