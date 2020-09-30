using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collect : MonoBehaviour
{
    public PlaneGameController gc; 

    public Scroller sc;

    public AudioSource source; 

    public int points;


    // Start is called before the first frame update
    void Start()
    {
        gc = GameObject.FindObjectOfType<PlaneGameController>();
        source = gameObject.GetComponent<AudioSource>(); 
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
         
            sc.ResetandRelocate();
            gc.ScoreToAdd(points);
            source.Play(); 

        }
    }
}
