using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodingShip : MonoBehaviour
{
    public List<GameObject> parts;
    public Vector2 randomForce;
    public float randomTorque;
    public GameObject midPart;
    public float power;
    public int index; 

    // Start is called before the first frame update
    void Start()
    {
        Explode(); 
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            Explode(); 
        }
        
    }

    public void Explode()
    {

        for (int i = 0; i < parts.Count; i++)
        {
            randomForce = new Vector2(Random.Range(-4.0f, 5.0f), Random.Range(-4.0f, 5.0f));
            randomTorque = Random.Range(-4.0f, 5.0f); 
            parts[i].GetComponent<Rigidbody2D>().AddRelativeForce(randomForce * Time.deltaTime * power);
            parts[i].GetComponent<Rigidbody2D>().AddTorque(randomTorque * Time.deltaTime * power/2);
        }
        

        

    }
}
