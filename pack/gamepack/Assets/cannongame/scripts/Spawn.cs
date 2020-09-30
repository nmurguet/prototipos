using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawn : MonoBehaviour
{
    public SpriteRenderer sr;
    public Sprite[] sprites;
    public int index; 
    // Start is called before the first frame update
    void Start()
    {
        sr = gameObject.GetComponent<SpriteRenderer>();
        index = Random.Range(0, sprites.Length);
        sr.sprite = sprites[index];
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
