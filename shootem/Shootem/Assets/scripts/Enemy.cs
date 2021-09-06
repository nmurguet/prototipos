using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{

    bool shrink = false;

    float scale = 1f;

    public GameObject explos; 
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (shrink) {
            scale -= 0.55f * Time.deltaTime;
                
        transform.localScale = new Vector2(scale, scale);
        if (scale <= 0.3f) { 
            Destroy(gameObject);
            Instantiate(explos, gameObject.transform.position, Quaternion.Euler(0, 0, 0));
            }
        }
    }

    public void SetShrink(bool value)
    {

        shrink = value; 
    }
}
