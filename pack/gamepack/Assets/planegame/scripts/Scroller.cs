using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scroller : MonoBehaviour
{
    

    public float speed = 3.0f;
    private Vector2 target;
    private Vector2 position;

    public SpriteRenderer sr; 
    

    // Start is called before the first frame update
    void Start()
    {
        position = new Vector3(Random.Range(10f, 45f), Random.Range(-5f, 5f), 0f);
        transform.position = position; 
        target = new Vector2(-10.72f, position.y);
        sr = gameObject.GetComponent<SpriteRenderer>();
        sr.sortingOrder = Random.Range(-1, 1);
        speed = Random.Range(2f, 4f); 
    }

    // Update is called once per frame
    void Update()
    {
        float step = speed * Time.deltaTime;

        // move sprite towards the target location
        transform.position = Vector2.MoveTowards(transform.position, target, step);

        if (transform.position.x - 1f < target.x)
        {

            ResetandRelocate();



        }


    }


    public void ResetandRelocate()
    {
        position = new Vector3(Random.Range(10f, 30f), Random.Range(-5f, 5f), 0f);
        target = new Vector2(-10.72f, position.y);
        transform.position = position;
        speed = Random.Range(2f, 4f);
    }
}
